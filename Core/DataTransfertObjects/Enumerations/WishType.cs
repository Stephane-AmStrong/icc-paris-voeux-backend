using System.Text.Json.Serialization;

namespace DataTransfertObjects.Enumerations;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum WishType
{
    Academic,
    Family,
    Financial,
    Material,
    Other,
    Professional,
    Relationship,
    Spiritual,
}
