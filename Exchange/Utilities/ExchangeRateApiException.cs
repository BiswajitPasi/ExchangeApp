

namespace Exchange.Utilities
{
    // <summary>
    // Represents custom error codes and their corresponding descriptions for the exchangerate.host API.
    // </summary>
    public enum ExchangeRateErrorCode
    {
        NotFound = 404,
        InvalidAccessKey = 101,
        NonExistentFunction = 103,
        ExceededRequestAllowance = 104,
        UnsupportedFunction = 105,
        NoResults = 106,
        InactiveAccount = 102,
        InvalidSourceCurrency = 201,
        InvalidCurrencyCodes = 202,
        NoDateSpecified = 301,
        InvalidDate = 302,
        InvalidFromProperty = 401,
        InvalidToProperty = 402,
        InvalidAmountProperty = 403,
        NoTimeFrameSpecified = 501,
        InvalidStartDate = 502,
        InvalidEndDate = 503,
        InvalidTimeFrame = 504,
        ExceededTimeFrameLimit = 505
    }
    // <summary>
    // Represents a custom exception for handling errors from the exchangerate.host API.
    //
    // Attributes:
    // - ErrorCode: The error code associated with the exception.
    // </summary>
    public class ExchangeRateApiException : Exception
    {
        // The error code associated with the exception.
        public ExchangeRateErrorCode ErrorCode { get; private set; }

        // <summary>
        // Constructs a new ExchangeRateApiException with the specified error code and message.
        //
        // Parameters:
        // - errorCode: The error code associated with the exception.
        // - message: The error message.
        // </summary>
        public ExchangeRateApiException(ExchangeRateErrorCode errorCode, string message) : base(message)
        {
            ErrorCode = errorCode;
        }
    }


}
