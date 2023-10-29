using Exchange.Interfaces;
using Exchange.Models;
using Exchange.Utilities;
using Newtonsoft.Json;

namespace Exchange.Services
{
    // <summary>
    // Represents a service that interacts with the ExchangeRate API to retrieve exchange rates and related information.
    // Implements the IExchangeRateApi interface.
    // </summary>
    public class ExchangeRateApi : IExchangeRateApi
    {
        private HttpClient _httpClient;
        private readonly ILogger _logger;
        private const string ApiBaseUrl = "http://api.exchangerate.host";
        private const string ApiAccessKey = "8e61e410d65eebf5a7ec8f246125a4bf";

        // <summary>
        // Constructs a new instance of the ExchangeRateApi class.
        //
        // Parameters:
        // - httpClient: An instance of the HttpClient class used to make HTTP requests to the API.
        // - logger: An instance of the ILogger class used for logging errors and messages.
        // </summary>
        public ExchangeRateApi(HttpClient httpClient, ILogger logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        // <summary>
        // Retrieves the exchange rates for the specified source currency from the ExchangeRate API.
        //
        // Parameters:
        // - sourceCurrency: The source currency for which the exchange rates should be retrieved.
        //
        // Returns:
        // - An instance of the APIResponseCurrencyList class containing the exchange rates.
        // </summary>
        public async Task<APIResponseCurrencyList> GetExchangeRates(string sourceCurrency)
        {
            var data = new APIResponseCurrencyList();
            try
            {
                var response = await _httpClient.GetAsync($"{ApiBaseUrl}/live?access_key={ApiAccessKey}&source={sourceCurrency}").ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                data = JsonConvert.DeserializeObject<APIResponseCurrencyList>(responseContent);
            }
            catch (ExchangeRateApiException ex)
            {
                _logger.LogError($"Error Code: {ex.ErrorCode}");
                _logger.LogError($"Error Message: {ex.Message}");
            }
            catch (Exception e)
            {
                _logger.LogError("Error while retrieving exchange rates: " + e.Message);
            }
            return data;
        }

        // <summary>
        // Retrieves the exchange rate for the specified currency code relative to USD from the ExchangeRate API.
        //
        // Parameters:
        // - currencyCode: The currency code for which the exchange rate should be retrieved.
        //
        // Returns:
        // - The exchange rate as a decimal value.
        // </summary>
        public async Task<decimal> GetExchangeRateToUSD(string currencyCode)
        {
            decimal rate = 0;
            try
            {
                currencyCode = currencyCode.Substring(currencyCode.Length - 3);
                var response = await _httpClient.GetAsync($"{ApiBaseUrl}/convert?access_key={ApiAccessKey}&from=USD&to={currencyCode}&amount=1").ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var responseData = JsonConvert.DeserializeObject<APIResponseForRateToUSD>(responseContent);

                if (responseData != null && responseData.Success)
                {
                    rate = responseData.Result;
                }
                return rate;
            }
            catch (ExchangeRateApiException ex)
            {
                _logger.LogError($"Error Code: {ex.ErrorCode}");
                _logger.LogError($"Error Message: {ex.Message}");
            }

            return rate;
        }

        // <summary>
        // Retrieves the exchange rate change for the specified currency code relative to the base currency from the ExchangeRate API.
        //
        // Parameters:
        // - currencyCode: The currency code for which the exchange rate change should be retrieved.
        // - baseCurrency: The base currency against which the exchange rate change is calculated.
        //
        // Returns:
        // - The exchange rate change as a decimal value.
        // </summary>
        public async Task<decimal> GetExchangeRateChange(string currencyCode, string baseCurrency)
        {
            decimal rate = 0;
            try
            {
                var startDate = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                var endDate = DateTime.Now.ToString("yyyy-MM-dd");
                var response = await _httpClient.GetAsync($"{ApiBaseUrl}/change?access_key={ApiAccessKey}&currencies={currencyCode}&start_date={startDate}&end_date={endDate}&source={baseCurrency}").ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var currencyCodeWithBase = $"{baseCurrency}{currencyCode}";
                dynamic responseData = JsonConvert.DeserializeObject(responseContent);
                dynamic rates = responseData.quotes[currencyCodeWithBase].change_pct;
                if (responseData != null)
                {
                    rate = rates;
                }
            }
            catch (ExchangeRateApiException ex)
            {
                _logger.LogError($"Error Code: {ex.ErrorCode}");
                _logger.LogError($"Error Message: {ex.Message}");
            }
            return rate;
        }
    }
}