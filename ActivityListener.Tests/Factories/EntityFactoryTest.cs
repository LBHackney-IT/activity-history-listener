using ActivityListener.Boundary;
using ActivityListener.Domain;
using ActivityListener.Factories;
using AutoFixture;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace ActivityListener.Tests.Factories
{
    public class EntityFactoryTest
    {
        private readonly Fixture _fixture = new Fixture();

        public static IEnumerable<object[]> AllEventTypes()
        {
            var allEvents = new[]
            {
                EventTypes.PersonCreatedEvent,
                EventTypes.PersonUpdatedEvent
            };
            foreach (var type in allEvents)
            {
                yield return new object[] { type };
            }
        }

        [Fact]
        public void CanMapADomainEntityToADatabaseObject()
        {
            var entity = _fixture.Create<ActivityHistoryEntity>();
            var databaseEntity = entity.ToDatabase();

            entity.Id.Should().Be(databaseEntity.Id);
            entity.CreatedAt.Should().BeSameDateAs(databaseEntity.CreatedAt);
            entity.AuthorDetails.Should().Be(databaseEntity.AuthorDetails);
            entity.NewData.Should().BeEquivalentTo(databaseEntity.NewData);
            entity.OldData.Should().BeEquivalentTo(databaseEntity.OldData);
            entity.TargetId.Should().Be(databaseEntity.TargetId);
            entity.TargetType.Should().Be(databaseEntity.TargetType);
            entity.TimetoLiveForRecord.Should().Be(databaseEntity.TimetoLiveForRecord);
            entity.Type.Should().Be(databaseEntity.Type);
        }

        [Theory]
        [MemberData(nameof(AllEventTypes))]
        public void CanGetTheActivityTypeFromAnEvent(string eventType)
        {
            var eventSns = new EntityEventSns() { EventType = eventType };
            switch (eventType)
            {
                case EventTypes.PersonCreatedEvent:
                    eventSns.GetActivityType().Should().Be(ActivityType.create);
                    break;
                case EventTypes.PersonUpdatedEvent:
                    eventSns.GetActivityType().Should().Be(ActivityType.update);
                    break;
                default:
                    {
                        Action act = () => eventSns.GetActivityType();
                        act.Should().Throw<ArgumentException>()
                                    .WithMessage($"Unknown event type: {eventSns.EventType}");
                        break;
                    }
            }
        }

        [Theory]
        [MemberData(nameof(AllEventTypes))]
        public void CanGetTheTargetTypeFromAnEvent(string eventType)
        {
            var eventSns = new EntityEventSns() { EventType = eventType };
            switch (eventType)
            {
                case EventTypes.PersonCreatedEvent:
                case EventTypes.PersonUpdatedEvent:
                    eventSns.GetTargetType().Should().Be(TargetType.person);
                    break;
                default:
                    {
                        Action act = () => eventSns.GetTargetType();
                        act.Should().Throw<ArgumentException>()
                                    .WithMessage($"Unknown event type: {eventSns.EventType}");
                        break;
                    }
            }
        }

        [Fact]
        public void CanGetTheAuthorDetailsFromAnEvent()
        {
            var eventSns = _fixture.Create<EntityEventSns>();
            var ad = eventSns.GetAuthorDetails();
            ad.Id.Should().Be(eventSns.User.Id.ToString());
            ad.Email.Should().Be(eventSns.User.Email);
            ad.FullName.Should().Be(eventSns.User.Name);
        }
    }
}
