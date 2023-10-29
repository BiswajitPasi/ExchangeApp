

namespace Exchange.Models
{

    // <summary>
    // Represents a currency with its exchange rates and changes.
    // </summary>
    public class Currency
    {
        // <summary>
        // Gets or sets the code of the currency.
        // </summary>
        public string Code { get; set; }

        // <summary>
        // Gets or sets the exchange rate of the currency to EUR.
        // </summary>
        public decimal RateToEUR { get; set; }

        // <summary>
        // Gets or sets the exchange rate of the currency to USD.
        // </summary>
        public decimal RateToUSD { get; set; }

        // <summary>
        // Gets or sets the change in exchange rate of the currency to EUR.
        // </summary>
        public decimal ChangeToEUR { get; set; }

        // <summary>
        // Gets or sets the change in exchange rate of the currency to USD.
        // </summary>
        public decimal ChangeToUSD { get; set; }
    }
}
