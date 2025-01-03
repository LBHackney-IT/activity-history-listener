using ActivityListener.Boundary;
using ActivityListener.Factories;
using ActivityListener.Tests.E2ETests.Fixtures;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Amazon.Lambda.TestUtilities;
using AutoFixture;
using FluentAssertions;
using Hackney.Shared.ActivityHistory.Factories;
using Hackney.Shared.ActivityHistory.Infrastructure;
using Moq;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ActivityListener.Tests.E2ETests.Steps
{
    public class ProcessActivityEventSteps : BaseSteps
    {
        private readonly Fixture _fixture = new Fixture();
        private readonly ActivityHistoryFixture _activityHistoryFixture;
        private Exception _lastException;

        public EntityEventSns EventSns { get; private set; }

        public ProcessActivityEventSteps(ActivityHistoryFixture activityHistoryFixture)
        {
            _activityHistoryFixture = activityHistoryFixture;
        }

        private SQSEvent.SQSMessage CreateMessage(EntityEventSns eventSns)
        {
            var msgBody = System.Text.Json.JsonSerializer.Serialize(eventSns, _jsonOptions);
            return _fixture.Build<SQSEvent.SQSMessage>()
                           .With(x => x.Body, msgBody)
                           .With(x => x.MessageAttributes, new Dictionary<string, SQSEvent.MessageAttribute>())
                           .Create();
        }

        public void GivenAnEntityActivityEvent(string eventType)
        {
            var eventData = new EventData()
            {
                OldData = new Dictionary<string, object>
                {
                    { "prop1", "Some OLD string value" },
                    { "prop2", "2000-01-01" },
                    { "prop3", false },
                    { "prop4", 50 },
                    { "prop5", new
                               {
                                    subProp1 = "some sub old prop string",
                                    subProp2 = false
                               }}
                },
                NewData = new Dictionary<string, object>
                {
                    { "prop1", "Some NEW string value" },
                    { "prop2", "2014-03-11" },
                    { "prop3", true },
                    { "prop4", 100 },
                    { "prop5", new
                               {
                                    subProp1 = "some sub New prop string",
                                    subProp2 = false
                               }}
                }
            };

            EventSns = _fixture.Build<EntityEventSns>()
                               .With(x => x.DateTime, DateTime.UtcNow)
                               .With(x => x.EventData, eventData)
                               .With(x => x.EventType, eventType)
                               .Create();
        }

        public async Task WhenTheFunctionIsTriggered(EntityEventSns eventSns)
        {
            var mockLambdaLogger = new Mock<ILambdaLogger>();
            ILambdaContext lambdaContext = new TestLambdaContext()
            {
                Logger = mockLambdaLogger.Object
            };

            var sqsEvent = _fixture.Build<SQSEvent>()
                                   .With(x => x.Records, new List<SQSEvent.SQSMessage> { CreateMessage(eventSns) })
                                   .Create();

            Func<Task> func = async () =>
            {
                var fn = new SqsFunction();
                await fn.FunctionHandler(sqsEvent, lambdaContext).ConfigureAwait(false);
            };

            _lastException = await Record.ExceptionAsync(func);
        }

        public void ThenAnUnknownEventTypeExceptionIsThrown(EntityEventSns eventSns)
        {
            _lastException.Should().NotBeNull();
            _lastException.Should().BeOfType(typeof(ArgumentException));
            (_lastException as ArgumentException).Message.Should().Be($"Unknown event type: {eventSns.EventType}");
        }

        public async Task ThenAnActivityHistoryRecordIsCreatedAsync(IDynamoDBContext dbContext, EntityEventSns eventSns)
        {
            var dbQuery = dbContext.QueryAsync<ActivityHistoryDB>(eventSns.EntityId);
            var resultsSet = await dbQuery.GetNextSetAsync().ConfigureAwait(false);
            resultsSet.Count.Should().Be(1);

            _activityHistoryFixture.ToDelete.AddRange(resultsSet);

            var expected = eventSns.ToDomain().ToDatabase();
            var actual = resultsSet.First();
            actual.Should().BeEquivalentTo(expected, config => config.Excluding(x => x.Id)
                                                                     .Excluding(x => x.OldData)
                                                                     .Excluding(x => x.NewData));

            actual.Id.Should().NotBeEmpty();
            VerifyDynamicDataObject(expected.OldData, actual.OldData);
            VerifyDynamicDataObject(expected.NewData, actual.NewData);
        }

        public async Task ThenNoActivityHistoryRecordIsCreatedAsync(IDynamoDBContext dbContext, EntityEventSns eventSns)
        {
            var dbQuery = dbContext.QueryAsync<ActivityHistoryDB>(eventSns.EntityId);
            var resultsSet = await dbQuery.GetNextSetAsync().ConfigureAwait(false);
            resultsSet.Count.Should().Be(0);
            _lastException.Should().BeNull();
        }

        private void VerifyDynamicDataObject(object expectedObj, object actualObj)
        {
            JToken expected = JToken.Parse(System.Text.Json.JsonSerializer.Serialize(expectedObj, _jsonOptions));
            JToken actual = JToken.Parse(System.Text.Json.JsonSerializer.Serialize(actualObj, _jsonOptions));

            actual.Should().BeEquivalentTo(expected);
        }

    }
}
