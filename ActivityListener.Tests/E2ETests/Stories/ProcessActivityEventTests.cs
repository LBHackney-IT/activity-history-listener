using ActivityListener.Tests.E2ETests.Fixtures;
using ActivityListener.Tests.E2ETests.Steps;
using System;
using System.Collections.Generic;
using TestStack.BDDfy;
using Xunit;

namespace ActivityListener.Tests.E2ETests.Stories
{
    [Story(
        AsA = "SQS Activity History Listener",
        IWant = "a function to process the entity events",
        SoThat = "an entity activity history record is created for each event")]
    [Collection("Aws collection")]
    public class ProcessActivityEventTests : IDisposable
    {
        private readonly AwsIntegrationTests _dbFixture;
        private readonly ActivityHistoryFixture _activityHistoryFixture;

        private readonly ProcessActivityEventSteps _steps;

        public ProcessActivityEventTests(AwsIntegrationTests dbFixture)
        {
            _dbFixture = dbFixture;

            _activityHistoryFixture = new ActivityHistoryFixture(dbFixture.DynamoDbContext);
            _steps = new ProcessActivityEventSteps(_activityHistoryFixture);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool _disposed;
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                _disposed = true;
            }
        }

        public static IEnumerable<object[]> AllEventTypes()
        {
            foreach (var type in EventTypeHelper.AllEventTypes)
            {
                yield return new object[] { type };
            }
        }

        [Fact]
        public void UnknownEventTypeThrows()
        {
            this.Given(g => _steps.GivenAnEntityActivityEvent("unknown-event"))
                .When(w => _steps.WhenTheFunctionIsTriggered(_steps.EventSns))
                .Then(t => _steps.ThenAnUnknownEventTypeExceptionIsThrown(_steps.EventSns))
                .BDDfy();
        }

        [Theory]
        [MemberData(nameof(AllEventTypes))]
        public void EventCreatesActivityHistoryRecord(string eventType)
        {
            this.Given(g => _steps.GivenAnEntityActivityEvent(eventType))
                .When(w => _steps.WhenTheFunctionIsTriggered(_steps.EventSns))
                .Then(t => _steps.ThenAnActivityHistoryRecordIsCreatedAsync(_dbFixture.DynamoDbContext, _steps.EventSns))
                .BDDfy();
        }

    }
}
