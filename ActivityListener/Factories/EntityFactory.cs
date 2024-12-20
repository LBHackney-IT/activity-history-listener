using ActivityListener.Boundary;
using Hackney.Shared.ActivityHistory.Domain;
using System;

namespace ActivityListener.Factories
{
    public static class EntityFactory
    {
        public static ActivityType? GetActivityType(this EntityEventSns eventSns)
        {
            switch (eventSns.EventType)
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
                case EventTypes.ProcessStartedAgainstTenureEvent:
                case EventTypes.ProcessStartedAgainstPersonEvent:
                case EventTypes.CautionaryAlertCreatedEvent:
                    return ActivityType.create;
                case EventTypes.ContractUpdatedEvent:
                case EventTypes.AssetUpdatedEvent:
                case EventTypes.PersonUpdatedEvent:
                case EventTypes.TenureUpdatedEvent:
                case EventTypes.HousingApplicationUpdatedEvent:
                case EventTypes.EqualityInformationUpdatedEvent:
                case EventTypes.ProcessUpdatedEvent:
                case EventTypes.ProcessClosedEvent:
                case EventTypes.ProcessCompletedEvent:
                case EventTypes.PatchOrAreaResEntityEditedEvent:
                    return ActivityType.update;
                case EventTypes.ContactDetailDeletedEvent:
                case EventTypes.PersonRemovedFromTenureEvent:
                    return ActivityType.delete;
                case EventTypes.CautionaryAlertEndedEvent:
                    return ActivityType.end;

                // Do not create an activity for these events
                case EventTypes.ContactDetailEditedEvent:
                case EventTypes.NoteCreatedAgainstAssetEvent:
                case EventTypes.NoteCreatedAgainstTenureEvent:
                case EventTypes.NoteCreatedAgainstPersonEvent:
                    return null;

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
                case EventTypes.NoteCreatedAgainstPersonEvent:
                    return TargetType.person;
                case EventTypes.ContactDetailAddedEvent:
                case EventTypes.ContactDetailEditedEvent:
                case EventTypes.ContactDetailDeletedEvent:
                    return TargetType.contactDetails;
                case EventTypes.TenureCreatedEvent:
                case EventTypes.TenureUpdatedEvent:
                case EventTypes.NoteCreatedAgainstTenureEvent:
                    return TargetType.tenure;
                case EventTypes.AssetCreatedEvent:
                case EventTypes.AssetUpdatedEvent:
                case EventTypes.NoteCreatedAgainstAssetEvent:
                    return TargetType.asset;
                case EventTypes.ContractCreatedEvent:
                case EventTypes.ContractUpdatedEvent:
                    return TargetType.contract;
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
                case EventTypes.ProcessStartedAgainstTenureEvent:
                    return TargetType.tenure;
                case EventTypes.ProcessStartedAgainstPersonEvent:
                    return TargetType.person;
                case EventTypes.CautionaryAlertCreatedEvent:
                case EventTypes.CautionaryAlertEndedEvent:
                    return TargetType.cautionaryAlert;
                case EventTypes.PatchOrAreaResEntityEditedEvent:
                    return TargetType.patchesAndAreas;

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
            var activityType = eventSns.GetActivityType();
            if (activityType == null) return null;
            return new ActivityHistoryEntity
            {
                Id = Guid.NewGuid(),
                Type = (ActivityType) activityType,
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
