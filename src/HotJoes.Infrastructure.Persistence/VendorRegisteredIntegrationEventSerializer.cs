using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using HotJoes.Application.Vendor;

namespace HotJoes.Infrastructure.Persistence;

public sealed class VendorRegisteredIntegrationEventSerializer
{
    private static readonly JsonSerializerOptions SerializerOptions = CreateOptions();

    public SerializedIntegrationEvent Serialize(
        VendorRegisteredIntegrationEvent integrationEvent)
    {
        ArgumentNullException.ThrowIfNull(integrationEvent);

        byte[] serializedEvent = JsonSerializer.SerializeToUtf8Bytes(
            integrationEvent,
            SerializerOptions);

        return new SerializedIntegrationEvent(
            integrationEvent.EventId,
            integrationEvent.EventVersion,
            serializedEvent);
    }

    private static JsonSerializerOptions CreateOptions()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.Never,
            WriteIndented = false
        };

        options.Converters.Add(new UtcRoundTripDateTimeOffsetConverter());
        options.Converters.Add(new InvariantTimeOnlyConverter());

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
                    out DateTimeOffset result))
            {
                throw new JsonException("Invalid UTC round-trip timestamp.");
            }

            return result;
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

    private sealed class InvariantTimeOnlyConverter : JsonConverter<TimeOnly>
    {
        public override TimeOnly Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            string? value = reader.GetString();

            if (!TimeOnly.TryParseExact(
                    value,
                    "HH:mm:ss",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out TimeOnly result))
            {
                throw new JsonException("Invalid invariant time-only value.");
            }

            return result;
        }

        public override void Write(
            Utf8JsonWriter writer,
            TimeOnly value,
            JsonSerializerOptions options)
        {
            writer.WriteStringValue(
                value.ToString("HH:mm:ss", CultureInfo.InvariantCulture));
        }
    }
}
