using ActivityListener.Boundary;
using ActivityListener.Factories;
using AutoFixture;
using FluentAssertions;
using Hackney.Shared.ActivityHistory.Domain;
using Hackney.Shared.ActivityHistory.Factories;
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
            foreach (var type in EventTypeHelper.AllEventTypes)
            {
                yield return new object[] { type };
            }
        }

        [Theory]
        [MemberData(nameof(AllEventTypes))]
        [InlineData("invalid")]
        public void CanGetTheActivityTypeFromAnEvent(string eventType)
        {
            var eventSns = new EntityEventSns() { EventType = eventType };
            switch (eventType)
            {
                case EventTypes.PersonCreatedEvent:
                case EventTypes.ContactDetailAddedEvent:
                case EventTypes.TenureCreatedEvent:
                case EventTypes.PersonAddedToTenureEvent:
                    eventSns.GetActivityType().Should().Be(ActivityType.create);
                    break;
                case EventTypes.PersonUpdatedEvent:
                case EventTypes.TenureUpdatedEvent:
                    eventSns.GetActivityType().Should().Be(ActivityType.update);
                    break;
                case EventTypes.ContactDetailDeletedEvent:
                case EventTypes.PersonRemovedFromTenureEvent:
                    eventSns.GetActivityType().Should().Be(ActivityType.delete);
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
        [InlineData("invalid")]
        public void CanGetTheTargetTypeFromAnEvent(string eventType)
        {
            var eventSns = new EntityEventSns() { EventType = eventType };
            switch (eventType)
            {
                case EventTypes.PersonCreatedEvent:
                case EventTypes.PersonUpdatedEvent:
                    eventSns.GetTargetType().Should().Be(TargetType.person);
                    break;
                case EventTypes.ContactDetailAddedEvent:
                case EventTypes.ContactDetailDeletedEvent:
                    eventSns.GetTargetType().Should().Be(TargetType.contactDetails);
                    break;
                case EventTypes.TenureCreatedEvent:
                case EventTypes.TenureUpdatedEvent:
                    eventSns.GetTargetType().Should().Be(TargetType.tenure);
                    break;
                case EventTypes.PersonAddedToTenureEvent:
                case EventTypes.PersonRemovedFromTenureEvent:
                    eventSns.GetTargetType().Should().Be(TargetType.tenurePerson);
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
            ad.Email.Should().Be(eventSns.User.Email);
            ad.FullName.Should().Be(eventSns.User.Name);
        }
    }
}
