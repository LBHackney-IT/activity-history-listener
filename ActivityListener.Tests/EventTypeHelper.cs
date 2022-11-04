namespace ActivityListener.Tests
{
    public static class EventTypeHelper
    {
        public static string[] AllEventTypes =>
            new[]
            {
                EventTypes.AssetCreatedEvent,
                EventTypes.AssetUpdatedEvent,
                EventTypes.ContractCreatedEvent,
                EventTypes.ContractUpdatedEvent,
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
                EventTypes.EqualityInformationUpdatedEvent,
                EventTypes.ProcessStartedEvent,
                EventTypes.ProcessUpdatedEvent,
                EventTypes.ProcessClosedEvent,
                EventTypes.ProcessCompletedEvent,
                EventTypes.NoteCreatedAgainstProcessEvent,
                EventTypes.ProcessStartedAgainstTenureEvent,
                EventTypes.ProcessStartedAgainstPersonEvent,
            };
    }
}
