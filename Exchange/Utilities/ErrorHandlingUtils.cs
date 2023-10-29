

namespace Exchange.Utilities
{
    // <summary>
    // Represents a class that handles custom error handling for the exchangerate.host API.
    // </summary>
    public class ErrorHandlingUtils
    {
        // <summary>
        // Handles custom error handling for the exchangerate.host API response.
        //
        // Parameters:
        // - errorCode: The error code received from the API response.
        //
        // Exceptions:
        // - Throws an ExchangeRateApiException with the corresponding error message based on the error code.
        // </summary>
        public static void HandleApiError(int errorCode)
        {
            switch (errorCode)
            {
                case 404:
                    throw new ExchangeRateApiException(ExchangeRateErrorCode.NotFound, "User requested a resource which does not exist.");
                case 101:
                    throw new ExchangeRateApiException(ExchangeRateErrorCode.InvalidAccessKey, "User did not supply an access key or supplied an invalid access key.");
                case 103:
                    throw new ExchangeRateApiException(ExchangeRateErrorCode.NonExistentFunction, "User requested a non-existent API function.");
                case 104:
                    throw new ExchangeRateApiException(ExchangeRateErrorCode.ExceededRequestAllowance, "User has reached or exceeded his subscription plan's monthly API request allowance.");
                case 105:
                    throw new ExchangeRateApiException(ExchangeRateErrorCode.UnsupportedFunction, "The user's current subscription plan does not support the requested API function.");
                case 106:
                    throw new ExchangeRateApiException(ExchangeRateErrorCode.NoResults, "The user's query did not return any results.");
                case 102:
                    throw new ExchangeRateApiException(ExchangeRateErrorCode.InactiveAccount, "The user's account is not active. User will be prompted to get in touch with Customer Support.");
                case 201:
                    throw new ExchangeRateApiException(ExchangeRateErrorCode.InvalidSourceCurrency, "User entered an invalid Source Currency.");
                case 202:
                    throw new ExchangeRateApiException(ExchangeRateErrorCode.InvalidCurrencyCodes, "User entered one or more invalid currency codes.");
                case 301:
                    throw new ExchangeRateApiException(ExchangeRateErrorCode.NoDateSpecified, "User did not specify a date. [historical]");
                case 302:
                    throw new ExchangeRateApiException(ExchangeRateErrorCode.InvalidDate, "User entered an invalid date. [historical, convert]");
                case 401:
                    throw new ExchangeRateApiException(ExchangeRateErrorCode.InvalidFromProperty, "User entered an invalid \"from\" property. [convert]");
                case 402:
                    throw new ExchangeRateApiException(ExchangeRateErrorCode.InvalidToProperty, "User entered an invalid \"to\" property. [convert]");
                case 403:
                    throw new ExchangeRateApiException(ExchangeRateErrorCode.InvalidAmountProperty, "User entered no or an invalid \"amount\" property. [convert]");
                case 501:
                    throw new ExchangeRateApiException(ExchangeRateErrorCode.NoTimeFrameSpecified, "User did not specify a Time-Frame. [timeframe, convert]");
                case 502:
                    throw new ExchangeRateApiException(ExchangeRateErrorCode.InvalidStartDate, "User entered an invalid \"start_date\" property. [timeframe, convert]");
                case 503:
                    throw new ExchangeRateApiException(ExchangeRateErrorCode.InvalidEndDate, "User entered an invalid \"end_date\" property. [timeframe, convert]");
                case 504:
                    throw new ExchangeRateApiException(ExchangeRateErrorCode.InvalidTimeFrame, "User entered an invalid Time-Frame. [timeframe, convert]");
                case 505:
                    throw new ExchangeRateApiException(ExchangeRateErrorCode.ExceededTimeFrameLimit, "The Time-Frame specified by the user is too long - exceeding 365 days. [timeframe]");
                default:
                    throw new ExchangeRateApiException(ExchangeRateErrorCode.NotFound, "Unknown error occurred.");
            }
        }
    }
}
