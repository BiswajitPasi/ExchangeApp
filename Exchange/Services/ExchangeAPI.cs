using Exchange.Interfaces;
using Exchange.Models;
using Exchange.Utilities;

namespace Exchange.Services
{
    // <summary>
    // Represents an implementation of the IExchangeAPI interface.
    // Provides methods for retrieving exchange rates and rendering currencies.
    // </summary>
    public class ExchangeAPI : IExchangeAPI
    {
        private readonly IExchangeRateApi _exchangeRateApi;
        private readonly ILogger _logger;

        // <summary>
        // Constructs a new instance of the ExchangeAPI class.
        //
        // Parameters:
        // - exchangeRateApi: An instance of the IExchangeRateApi interface used for retrieving exchange rates.
        // - logger: An instance of the ILogger interface used for logging errors.
        // </summary>
        public ExchangeAPI(IExchangeRateApi exchangeRateApi, ILogger logger)
        {
            _exchangeRateApi = exchangeRateApi;
            _logger = logger;
        }

        // <summary>
        // Retrieves the exchange rates for the specified source currency.
        //
        // Parameters:
        // - sourceCurrency: The source currency for which to retrieve exchange rates.
        //
        // Returns:
        // - A list of Currency objects representing the exchange rates.
        // </summary>
        public async Task<List<Currency>> GetExchangeRates(string sourceCurrency)
        {
            var currencies = new List<Currency>();
            try
            {
                var exchangeRateApiResponse = await _exchangeRateApi.GetExchangeRates(sourceCurrency).ConfigureAwait(false);
                if (exchangeRateApiResponse.Error != null)
                {
                    ErrorHandlingUtils.HandleApiError(exchangeRateApiResponse.Error.Code);
                    return currencies;
                }

                var rates = exchangeRateApiResponse.Quotes;
                if (rates != null)
                {
                    var tasks = new List<Task>();
                    foreach (var rate in rates)
                    {
                        var currencyCode = rate.Key;
                        var currency = new Currency
                        {
                            Code = currencyCode,
                            RateToEUR = rate.Value,
                            ChangeToUSD = 0
                        };
                        tasks.Add(GetExchangeRateToUSD(currencyCode, currency));
                        tasks.Add(GetExchangeRateChange(currencyCode, currency, "EUR"));
                        tasks.Add(GetExchangeRateChange(currencyCode, currency, "USD"));
                        currencies.Add(currency);
                    }
                    await Task.WhenAll(tasks).ConfigureAwait(false);
                }
            }
            catch (ExchangeRateApiException ex)
            {
                _logger.LogError($"Error Code: {ex.ErrorCode}");
                _logger.LogError($"Error Message: {ex.Message}");
            }
            catch (Exception e)
            {
                _logger.LogError($"Error Message: {e.Message}");
            }

            return currencies;
        }

        // <summary>
        // Retrieves the exchange rate to USD for the specified currency.
        //
        // Parameters:
        // - currencyCode: The currency code for which to retrieve the exchange rate.
        // - currency: The Currency object to update with the exchange rate.
        // </summary>
        public async Task GetExchangeRateToUSD(string currencyCode, Currency currency)
        {
            try
            {
                currencyCode = currencyCode.Substring(currencyCode.Length - 3);
                currency.RateToUSD = await _exchangeRateApi.GetExchangeRateToUSD(currencyCode).ConfigureAwait(false);
            }
            catch (ExchangeRateApiException ex)
            {
                _logger.LogError($"Error Code: {ex.ErrorCode}");
                _logger.LogError($"Error Message: {ex.Message}");
            }
            catch (Exception e)
            {
                _logger.LogError($"Error Message: {e.Message}");
            }
        }

        // <summary>
        // Retrieves the exchange rate change for the specified currency and base currency.
        //
        // Parameters:
        // - currencyCode: The currency code for which to retrieve the exchange rate change.
        // - currency: The Currency object to update with the exchange rate change.
        // - baseCurrency: The base currency against which to calculate the exchange rate change.
        // </summary>
        public async Task GetExchangeRateChange(string currencyCode, Currency currency, string baseCurrency)
        {
            try
            {
                currencyCode = currencyCode.Substring(currencyCode.Length - 3);
                var change = await _exchangeRateApi.GetExchangeRateChange(currencyCode, baseCurrency).ConfigureAwait(false);
                if (baseCurrency == "EUR")
                {
                    currency.ChangeToEUR = change;
                }
                else if (baseCurrency == "USD")
                {
                    currency.ChangeToUSD = change;
                }
            }
            catch (ExchangeRateApiException ex)
            {
                _logger.LogError($"Error Code: {ex.ErrorCode}");
                _logger.LogError($"Error Message: {ex.Message}");
            }
            catch (Exception e)
            {
                _logger.LogError($"Error Message: {e.Message}");
            }
        }

        // <summary>
        // Renders the currencies and their exchange rates to the console.
        //
        // Parameters:
        // - currencies: The list of Currency objects to render.
        // </summary>
        public void RenderCurrencies(List<Currency> currencies)
        {
            try
            {
                if (currencies == null || currencies.Count == 0)
                {
                    Console.WriteLine("------------------------------------------------------------------------------------");
                    Console.WriteLine("No data found");
                    Console.WriteLine("------------------------------------------------------------------------------------");
                    return;
                }
                Console.WriteLine("Currencies and Exchange Rates:");
                Console.WriteLine("------------------------------------------------------------------------------------");
                Console.WriteLine("Currency\t\tRate to EUR\tRate to USD\tChange to EUR (%)\tChange to USD (%)");
                Console.WriteLine("------------------------------------------------------------------------------------");
                foreach (var currency in currencies)
                {
                    Console.WriteLine($"{currency.Code.Substring(currency.Code.Length - 3)}\t\t{currency.RateToEUR}\t\t{currency.RateToUSD}\t\t{currency.ChangeToEUR}\t\t{currency.ChangeToUSD}");
                    Console.WriteLine("-----------------------------------------------------------------------------------");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error: " + ex.Message);
            }
        }
    }
}