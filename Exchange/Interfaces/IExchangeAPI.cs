using Exchange.Models;

namespace Exchange.Interfaces
{
    public interface IExchangeAPI
    {
        Task<List<Currency>> GetExchangeRates(string sourceCurrency);
        Task GetExchangeRateToUSD(string currencyCode, Currency currency);
        Task GetExchangeRateChange(string currencyCode, Currency currency, string baseCurrency);
        void RenderCurrencies(List<Currency> currencies);
    }
}
