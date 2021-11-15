namespace ActivityListener.Tests
{
    public static class EventTypeHelper
    {
        public static string[] AllEventTypes =>
            new[]
            {
                EventTypes.PersonCreatedEvent,
                EventTypes.PersonUpdatedEvent,
                EventTypes.ContactDetailAddedEvent,
                EventTypes.ContactDetailDeletedEvent,
                EventTypes.TenureCreatedEvent,
                EventTypes.TenureUpdatedEvent,
                EventTypes.PersonAddedToTenureEvent,
                EventTypes.PersonRemovedFromTenureEvent,
                EventTypes.HousingApplicationCreatedEvent,
                EventTypes.HousingApplicationUpdatedEvent,
                EventTypes.EqualityInformationCreatedEvent,
                EventTypes.EqualityInformationUpdatedEvent
            };
    }
}
