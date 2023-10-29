
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Exchange.Models
{
     
     

        public partial class APIResponseForRateToUSD
    {
            [JsonProperty("success")]
            public bool Success { get; set; }

            [JsonProperty("terms")]
            public Uri Terms { get; set; }

            [JsonProperty("privacy")]
            public Uri Privacy { get; set; }

            [JsonProperty("query")]
            public Query Query { get; set; }

            [JsonProperty("info")]
            public Info Info { get; set; }

            [JsonProperty("result")]
            public decimal Result { get; set; }
        }

        public partial class Info
        {
            [JsonProperty("timestamp")]
            public long Timestamp { get; set; }

            [JsonProperty("quote")]
            public double Quote { get; set; }
        }

        public partial class Query
        {
            [JsonProperty("from")]
            public string From { get; set; }

            [JsonProperty("to")]
            public string To { get; set; }

            [JsonProperty("amount")]
            public long Amount { get; set; }
        }

        public partial class APIResponseForRateToUSD
    {
            public static APIResponseForRateToUSD FromJson(string json) => JsonConvert.DeserializeObject<APIResponseForRateToUSD>(json, Converter.Settings);
        }


    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}


