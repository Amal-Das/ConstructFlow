namespace ConstructFlow.Application.Common.Exceptions;

public class BusinessRuleException : Exception
{
    public string ErrorCode { get; }

    public BusinessRuleException(string errorCode, string message) : base(message)
    {
        ErrorCode = errorCode;
    }
}