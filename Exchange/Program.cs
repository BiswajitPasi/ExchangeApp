using Exchange.Interfaces;
using Exchange.Services;
using Exchange.Utilities;
using Microsoft.Extensions.DependencyInjection;
// <summary>
// Represents an application for retrieving and rendering exchange rates.
// </summary>
// <summary>
// Represents the entry point of the program.
// </summary>
public class Program
    {
    private static readonly ILogger _logger = new Logger();
    // <summary>
    // The main method that is called when the program starts.
    // </summary>
    public static async Task Main()
        {
            try
            {

            var serviceProvider = new ServiceCollection()
            .AddHttpClient().AddTransient<ILogger, Logger>()
            .AddTransient<IExchangeRateApi, ExchangeRateApi>()
            .AddTransient<IExchangeAPI, ExchangeAPI>()
            .BuildServiceProvider();

         

            var exchangeService = serviceProvider.GetService<IExchangeAPI>();
            
            var currencies = await exchangeService.GetExchangeRates("EUR");
             exchangeService.RenderCurrencies(currencies);

            
           
            }
            catch (HttpRequestException e)
            {
            // Print the error message if an HttpRequestException occurs.
           
            _logger.LogError($"Error: {e.Message}");
            }
        }
    }
