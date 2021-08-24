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
                EventTypes.TenureCreatedEvent
            };
    }
}
