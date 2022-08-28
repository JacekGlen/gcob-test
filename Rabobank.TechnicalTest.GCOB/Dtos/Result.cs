using System;
using Rabobank.TechnicalTest.GCOB.Enums;

namespace Rabobank.TechnicalTest.GCOB.Dtos
{
    /// <summary>
    /// A generic wrapper for service results.
    /// </summary>
    public class Result<T>
    {
        public T Value { get; }

        public bool Succeeded => ErrorType is null;

        public string ErrorMessage { get; }

        public ErrorType? ErrorType { get; }

        protected Result(string errorMessage, ErrorType errorType)
        {
            ErrorType = errorType;
            ErrorMessage = errorMessage;
        }

        public Result(T value)
        {
            Value = value;
        }

        public static Result<T> Success(T value) => new(value);

        public static Result<T> Failure(string errorMessage, ErrorType errorType) => new(errorMessage, errorType);

        public static Result<T> Failure(Exception ex) => new(ex.Message, Enums.ErrorType.Exception);
    }
}
