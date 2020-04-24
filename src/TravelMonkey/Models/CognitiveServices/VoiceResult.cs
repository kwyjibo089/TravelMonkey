using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Globalization;

namespace TravelMonkey.Models.CognitiveServices
{
    public static class VoiceResult
    {
        public static Voice[] Voices { get; set; }
    }

    public partial class Voice
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("ShortName")]
        public string ShortName { get; set; }

        [JsonProperty("Gender")]
        public Gender Gender { get; set; }

        [JsonProperty("Locale")]
        public string Locale { get; set; }

        [JsonProperty("SampleRateHertz")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long SampleRateHertz { get; set; }

        [JsonProperty("VoiceType")]
        public VoiceType VoiceType { get; set; }
    }

    public enum Gender { Female, Male };

    public enum VoiceType { Neural, Standard };

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                GenderConverter.Singleton,
                VoiceTypeConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class GenderConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(Gender) || t == typeof(Gender?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "Female":
                    return Gender.Female;
                case "Male":
                    return Gender.Male;
            }
            throw new Exception("Cannot unmarshal type Gender");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (Gender)untypedValue;
            switch (value)
            {
                case Gender.Female:
                    serializer.Serialize(writer, "Female");
                    return;
                case Gender.Male:
                    serializer.Serialize(writer, "Male");
                    return;
            }
            throw new Exception("Cannot marshal type Gender");
        }

        public static readonly GenderConverter Singleton = new GenderConverter();
    }

    internal class ParseStringConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            long l;
            if (Int64.TryParse(value, out l))
            {
                return l;
            }
            throw new Exception("Cannot unmarshal type long");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (long)untypedValue;
            serializer.Serialize(writer, value.ToString());
            return;
        }

        public static readonly ParseStringConverter Singleton = new ParseStringConverter();
    }

    internal class VoiceTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(VoiceType) || t == typeof(VoiceType?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "Neural":
                    return VoiceType.Neural;
                case "Standard":
                    return VoiceType.Standard;
            }
            throw new Exception("Cannot unmarshal type VoiceType");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (VoiceType)untypedValue;
            switch (value)
            {
                case VoiceType.Neural:
                    serializer.Serialize(writer, "Neural");
                    return;
                case VoiceType.Standard:
                    serializer.Serialize(writer, "Standard");
                    return;
            }
            throw new Exception("Cannot marshal type VoiceType");
        }

        public static readonly VoiceTypeConverter Singleton = new VoiceTypeConverter();
    }
}
