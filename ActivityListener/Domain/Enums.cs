using System.Text.Json.Serialization;

namespace ActivityListener.Domain
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TargetType
    {
        person,
        asset,
        tenure,
        contactDetails,
        tenurePerson
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ActivityType
    {
        create,
        update,
        delete,
        migrate
    }
}
