using Exchange.Models;


namespace Exchange.Interfaces
{
    public interface IExchangeRateApi
    {
        Task<APIResponseCurrencyList> GetExchangeRates(string sourceCurrency);
        Task<decimal> GetExchangeRateToUSD(string currencyCode);
        Task<decimal> GetExchangeRateChange(string currencyCode, string baseCurrency);
    }
}
