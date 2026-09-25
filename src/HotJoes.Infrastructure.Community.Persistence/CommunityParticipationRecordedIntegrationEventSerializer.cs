using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HotJoes.Infrastructure.Community.Persistence;

public sealed class CommunityParticipationRecordedIntegrationEventSerializer
{
    private static readonly JsonSerializerOptions SerializerOptions =
        CreateOptions();

    public SerializedCommunityIntegrationEvent Serialize(
        CommunityParticipationRecordedIntegrationEvent integrationEvent)
    {
        ArgumentNullException.ThrowIfNull(integrationEvent);

        byte[] bytes = JsonSerializer.SerializeToUtf8Bytes(
            integrationEvent,
            SerializerOptions);

        return new SerializedCommunityIntegrationEvent(
            integrationEvent.EventId,
            integrationEvent.EventVersion,
            bytes);
    }

    private static JsonSerializerOptions CreateOptions()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.Never,
            WriteIndented = false
        };
        options.Converters.Add(new JsonStringEnumConverter(
            JsonNamingPolicy.CamelCase));
        options.Converters.Add(new UtcRoundTripDateTimeOffsetConverter());
        return options;
    }

    private sealed class UtcRoundTripDateTimeOffsetConverter
        : JsonConverter<DateTimeOffset>
    {
        public override DateTimeOffset Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            string? value = reader.GetString();

            if (!DateTimeOffset.TryParseExact(
                    value,
                    "O",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal,
                    out DateTimeOffset parsed))
            {
                throw new JsonException("Invalid UTC round-trip timestamp.");
            }

            return parsed.ToUniversalTime();
        }

        public override void Write(
            Utf8JsonWriter writer,
            DateTimeOffset value,
            JsonSerializerOptions options)
        {
            writer.WriteStringValue(
                value.UtcDateTime.ToString("O", CultureInfo.InvariantCulture));
        }
    }
}
