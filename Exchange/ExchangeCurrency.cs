
using Exchange.Models;
using Newtonsoft.Json;

namespace Exchange
{
    // <summary>
    // Represents an application for retrieving and rendering exchange rates.
    // </summary>
    public class ExchangeCurrency
    {
        private const string ApiBaseUrl = "http://api.exchangerate.host";
        private const string ApiAccessKey = "bdb0ccec2d3194d5bd5dbe32f2bd8fe9";
        private static readonly ILogger _logger = new Logger();

        // <summary>
        // Retrieves the exchange rates for various currencies.
        //
        // Returns:
        // - A list of Currency objects representing the exchange rates.
        // </summary>
        public async Task<List<Currency>> GetExchangeRates()
        {
            var currencies = new List<Currency>();

            using (var client = new HttpClient())
            {
                try
                {

                    var response = await client.GetAsync($"{ApiBaseUrl}/live?access_key={ApiAccessKey}&source=EUR").ConfigureAwait(false);
                    response.EnsureSuccessStatusCode();


                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                        dynamic responseData = JsonConvert.DeserializeObject(responseContent);


                        //  await ExchangeRateErrorHandler.HandleCustomErrors(responseData);

                        dynamic rates = responseData.quotes;
                        if (rates != null)
                        {

                            var tasks = new List<Task>();

                            foreach (var rate in rates)
                            {
                                var currencyCode = rate.Name;
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
                        else
                        {
                            int errorCode = responseData.error.code;
                            ErrorHandlingUtils.HandleApiError(errorCode);
                        }
                    }

                }
                catch (ExchangeRateApiException ex)
                {
                    _logger.LogError($"Error Code: {ex.ErrorCode}");
                    _logger.LogError($"Error Message: {ex.Message}");
                }
                catch (HttpRequestException e)
                {
                    _logger.LogError("Error while retrieving exchange rates: " + e.Message);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error: " + ex.Message);
                }

                return currencies;
            }
        }

        // <summary>
        // Retrieves the exchange rate of a currency to USD.
        //
        // Parameters:
        // - currencyCode: The code of the currency.
        // - currency: The Currency object to update with the exchange rate.
        // </summary>
        private async Task GetExchangeRateToUSD(string currencyCode, Currency currency)
        {
            currencyCode = currencyCode.Substring(currencyCode.Length - 3);

            using (var client = new HttpClient())
            {
                try
                {
                    var response = await client.GetAsync($"{ApiBaseUrl}/convert?access_key={ApiAccessKey}&from=USD&to={currencyCode}&amount=1").ConfigureAwait(false);
                    response.EnsureSuccessStatusCode();

                    var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    dynamic responseData = JsonConvert.DeserializeObject(responseContent);

                    if (!string.IsNullOrEmpty(Convert.ToString(responseData.result)))
                    {
                        currency.RateToUSD = responseData.result;
                    }
                }
                catch (ExchangeRateApiException ex)
                {
                    _logger.LogError($"Error Code: {ex.ErrorCode}");
                    _logger.LogError($"Error Message: {ex.Message}");
                }
                catch (HttpRequestException e)
                {
                    _logger.LogError($"Error while retrieving exchange rate for {currencyCode} to USD: " + e.Message);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error while retrieving exchange rate: " + ex.Message);
                }
            }
        }

        // <summary>
        // Retrieves the change in exchange rate of a currency to a specified base currency.
        //
        // Parameters:
        // - currencyCode: The code of the currency.
        // - currency: The Currency object to update with the exchange rate change.
        // - baseCurrency: The base currency for the exchange rate change.
        // </summary>
        private async Task GetExchangeRateChange(string currencyCode, Currency currency, string baseCurrency)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    currencyCode = currencyCode.Substring(currencyCode.Length - 3);
                    var startDate = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                    var endDate = DateTime.Now.ToString("yyyy-MM-dd");

                    var response = await client.GetAsync($"{ApiBaseUrl}/change?access_key={ApiAccessKey}&currencies={currencyCode}&start_date={startDate}&end_date={endDate}&source={baseCurrency}").ConfigureAwait(false);
                    response.EnsureSuccessStatusCode();

                    var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    dynamic responseData = JsonConvert.DeserializeObject(responseContent);



                    currencyCode = $"{baseCurrency}{currencyCode}";
                    dynamic rates = responseData.quotes[currencyCode].change_pct;

                    if (rates != null)
                    {
                        if (baseCurrency == "EUR")
                        {
                            currency.ChangeToEUR = rates;
                        }
                        else if (baseCurrency == "USD")
                        {
                            currency.ChangeToUSD = rates;
                        }
                    }
                }
                catch (ExchangeRateApiException ex)
                {
                    _logger.LogError($"Error Code: {ex.ErrorCode}");
                    _logger.LogError($"Error Message: {ex.Message}");
                }
                catch (HttpRequestException e)
                {
                    _logger.LogError($"Error while retrieving exchange rate change for {currencyCode} to {baseCurrency}: " + e.Message);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error: " + ex.Message);
                }
            }
        }

        // <summary>
        // Renders the currencies and their exchange rates.
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
            catch (ExchangeRateApiException ex)
            {
                _logger.LogError($"Error Code: {ex.ErrorCode}");
                _logger.LogError($"Error Message: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error: " + ex.Message);
            }
        }
    }
}
