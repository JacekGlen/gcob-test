namespace Rabobank.TechnicalTest.GCOB.Enums
{
    /// <summary>
    /// An enum value indicates what kind of error has occurred during a service call.
    /// It can be expand with other type of errors such as Authentication error.
    /// </summary>
    public enum ErrorType
    {
        ValidationError,
        Exception
    }
}
