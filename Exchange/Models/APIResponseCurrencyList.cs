using Newtonsoft.Json;


namespace Exchange.Models
{
    public class APIResponseCurrencyList
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("terms")]
        public Uri Terms { get; set; }

        [JsonProperty("privacy")]
        public Uri Privacy { get; set; }

        [JsonProperty("timestamp")]
        public long Timestamp { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("quotes")]
        public Dictionary<string, decimal> Quotes { get; set; }
        public ExchangeRateApiError Error { get; set; }
    }
}
