using ActivityListener.Boundary;
using ActivityListener.Factories;
using AutoFixture;
using FluentAssertions;
using Hackney.Shared.ActivityHistory.Domain;
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
                case EventTypes.ContractCreatedEvent:
                case EventTypes.AssetCreatedEvent:
                case EventTypes.PersonCreatedEvent:
                case EventTypes.ContactDetailAddedEvent:
                case EventTypes.TenureCreatedEvent:
                case EventTypes.PersonAddedToTenureEvent:
                case EventTypes.HousingApplicationCreatedEvent:
                case EventTypes.EqualityInformationCreatedEvent:
                case EventTypes.ProcessStartedEvent:
                case EventTypes.NoteCreatedAgainstProcessEvent:
                case EventTypes.ProcessStartedAgainstPersonEvent:
                case EventTypes.ProcessStartedAgainstTenureEvent:
                    eventSns.GetActivityType().Should().Be(ActivityType.create);
                    break;
                case EventTypes.ContractUpdatedEvent:
                case EventTypes.AssetUpdatedEvent:
                case EventTypes.PersonUpdatedEvent:
                case EventTypes.TenureUpdatedEvent:
                case EventTypes.HousingApplicationUpdatedEvent:
                case EventTypes.EqualityInformationUpdatedEvent:
                case EventTypes.ProcessUpdatedEvent:
                case EventTypes.ProcessClosedEvent:
                case EventTypes.ProcessCompletedEvent:
                    eventSns.GetActivityType().Should().Be(ActivityType.update);
                    break;
                case EventTypes.ContactDetailDeletedEvent:
                case EventTypes.PersonRemovedFromTenureEvent:
                    eventSns.GetActivityType().Should().Be(ActivityType.delete);
                    break;
                case EventTypes.CautionaryAlertCreatedEvent:
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
                case EventTypes.AssetCreatedEvent:
                case EventTypes.AssetUpdatedEvent:
                    eventSns.GetTargetType().Should().Be(TargetType.asset);
                    break;
                case EventTypes.ContractCreatedEvent:
                case EventTypes.ContractUpdatedEvent:
                    eventSns.GetTargetType().Should().Be(TargetType.contract);
                    break;
                case EventTypes.PersonCreatedEvent:
                case EventTypes.PersonUpdatedEvent:
                case EventTypes.ProcessStartedAgainstPersonEvent:
                    eventSns.GetTargetType().Should().Be(TargetType.person);
                    break;
                case EventTypes.ContactDetailAddedEvent:
                case EventTypes.ContactDetailDeletedEvent:
                    eventSns.GetTargetType().Should().Be(TargetType.contactDetails);
                    break;
                case EventTypes.TenureCreatedEvent:
                case EventTypes.TenureUpdatedEvent:
                case EventTypes.ProcessStartedAgainstTenureEvent:
                    eventSns.GetTargetType().Should().Be(TargetType.tenure);
                    break;
                case EventTypes.PersonAddedToTenureEvent:
                case EventTypes.PersonRemovedFromTenureEvent:
                    eventSns.GetTargetType().Should().Be(TargetType.tenurePerson);
                    break;
                case EventTypes.HousingApplicationCreatedEvent:
                case EventTypes.HousingApplicationUpdatedEvent:
                    eventSns.GetTargetType().Should().Be(TargetType.housingApplication);
                    break;
                case EventTypes.EqualityInformationCreatedEvent:
                case EventTypes.EqualityInformationUpdatedEvent:
                    eventSns.GetTargetType().Should().Be(TargetType.personEqualityInformation);
                    break;
                case EventTypes.ProcessStartedEvent:
                case EventTypes.ProcessUpdatedEvent:
                case EventTypes.ProcessClosedEvent:
                case EventTypes.ProcessCompletedEvent:
                case EventTypes.NoteCreatedAgainstProcessEvent:
                    eventSns.GetTargetType().Should().Be(TargetType.process);
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
