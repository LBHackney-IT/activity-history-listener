using ActivityListener.Boundary;
using ActivityListener.Domain;
using ActivityListener.Infrastructure;
using System;
using System.Collections.Generic;

namespace ActivityListener.Factories
{
    public static class EntityFactory
    {
        public static ActivityHistoryDB ToDatabase(this ActivityHistoryEntity entity)
        {
            return new ActivityHistoryDB
            {
                Id = entity.Id,
                CreatedAt = entity.CreatedAt,
                Type = entity.Type,
                SourceDomain = entity.SourceDomain,
                OldData = entity.OldData,
                TimetoLiveForRecord = entity.TimetoLiveForRecord,
                TargetType = entity.TargetType,
                TargetId = entity.TargetId,
                NewData = entity.NewData,
                AuthorDetails = entity.AuthorDetails
            };
        }

        public static ActivityType GetActivityType(this EntityEventSns eventSns)
        {
            switch (eventSns.EventType)
            {
                case EventTypes.PersonCreatedEvent:
                case EventTypes.ContactDetailAddedEvent:
                    return ActivityType.create;
                case EventTypes.PersonUpdatedEvent:
                    return ActivityType.update;
                case EventTypes.ContactDetailDeletedEvent:
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
                case EventTypes.ContactDetailAddedEvent:
                case EventTypes.ContactDetailDeletedEvent:
                    return TargetType.person;

                default:
                    throw new ArgumentException($"Unknown event type: {eventSns.EventType}");
            }
        }

        public static AuthorDetails GetAuthorDetails(this EntityEventSns eventSns)
        {
            return new AuthorDetails
            {
                Id = eventSns.User.Id.ToString(),
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
                OldData = /*(Dictionary<string, object>)*/eventSns.EventData.OldData,
                NewData = /*(Dictionary<string, object>)*/ eventSns.EventData.NewData,
                AuthorDetails = eventSns.GetAuthorDetails()
            };
        }
    }
}
