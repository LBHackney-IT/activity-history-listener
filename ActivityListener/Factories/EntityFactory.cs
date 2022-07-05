using ActivityListener.Boundary;
using Hackney.Shared.ActivityHistory.Domain;
using System;

namespace ActivityListener.Factories
{
    public static class EntityFactory
    {
        public static ActivityType GetActivityType(this EntityEventSns eventSns)
        {
            switch (eventSns.EventType)
            {
                case EventTypes.PersonCreatedEvent:
                case EventTypes.ContactDetailAddedEvent:
                case EventTypes.TenureCreatedEvent:
                case EventTypes.PersonAddedToTenureEvent:
                case EventTypes.HousingApplicationCreatedEvent:
                case EventTypes.EqualityInformationCreatedEvent:
                case EventTypes.ProcessStartedEvent:
                case EventTypes.NoteCreatedAgainstProcessEvent:
                    return ActivityType.create;
                case EventTypes.PersonUpdatedEvent:
                case EventTypes.TenureUpdatedEvent:
                case EventTypes.HousingApplicationUpdatedEvent:
                case EventTypes.EqualityInformationUpdatedEvent:
                case EventTypes.ProcessUpdatedEvent:
                case EventTypes.ProcessClosedEvent:
                case EventTypes.ProcessCompletedEvent:
                case EventTypes.ProcessStartedAgainstTenure:
                case EventTypes.ProcessStartedAgainstPerson:
                    return ActivityType.update;
                case EventTypes.ContactDetailDeletedEvent:
                case EventTypes.PersonRemovedFromTenureEvent:
                    return ActivityType.delete;

                default:
                    throw new ArgumentException($"Unknown event type: {eventSns.EventType}");
            }
        }

        public static TargetType GetTargetType(this EntityEventSns eventSns)
        {
            switch (eventSns.EventType)
            {
                case EventTypes.PersonCreatedEvent:
                case EventTypes.PersonUpdatedEvent:
                    return TargetType.person;
                case EventTypes.ContactDetailAddedEvent:
                case EventTypes.ContactDetailDeletedEvent:
                    return TargetType.contactDetails;
                case EventTypes.TenureCreatedEvent:
                case EventTypes.TenureUpdatedEvent:
                    return TargetType.tenure;
                case EventTypes.PersonAddedToTenureEvent:
                case EventTypes.PersonRemovedFromTenureEvent:
                    return TargetType.tenurePerson;
                case EventTypes.HousingApplicationCreatedEvent:
                case EventTypes.HousingApplicationUpdatedEvent:
                    return TargetType.housingApplication;
                case EventTypes.EqualityInformationCreatedEvent:
                case EventTypes.EqualityInformationUpdatedEvent:
                    return TargetType.personEqualityInformation;
                case EventTypes.ProcessStartedEvent:
                case EventTypes.ProcessUpdatedEvent:
                case EventTypes.ProcessClosedEvent:
                case EventTypes.ProcessCompletedEvent:
                case EventTypes.NoteCreatedAgainstProcessEvent:
                    return TargetType.process;
                case EventTypes.ProcessStartedAgainstTenure:
                    return TargetType.tenure;
                case EventTypes.ProcessStartedAgainstPerson:
                    return TargetType.person;

                default:
                    throw new ArgumentException($"Unknown event type: {eventSns.EventType}");
            }
        }

        public static AuthorDetails GetAuthorDetails(this EntityEventSns eventSns)
        {
            return new AuthorDetails
            {
                Email = eventSns.User.Email,
                FullName = eventSns.User.Name
            };
        }

        public static ActivityHistoryEntity ToDomain(this EntityEventSns eventSns)
        {
            return new ActivityHistoryEntity
            {
                Id = Guid.NewGuid(),
                Type = eventSns.GetActivityType(),
                SourceDomain = eventSns.SourceDomain,
                TargetType = eventSns.GetTargetType(),
                TargetId = eventSns.EntityId,
                CreatedAt = eventSns.DateTime,
                TimetoLiveForRecord = default,
                OldData = eventSns.EventData.OldData,
                NewData = eventSns.EventData.NewData,
                AuthorDetails = eventSns.GetAuthorDetails()
            };
        }
    }
}
