using FluentValidation;
using KernelAbstractions.Patterns.Results;

namespace KernelAbstractions.Application.Extensions;

/// <summary>
/// Provides extension methods for integrating <see cref="IError"/> with FluentValidation.
/// </summary>
public static partial class Extensions
{
    /// <summary>
    /// Sets the error message, code, and type from the specified <see cref="IError"/>.
    /// </summary>
    /// <typeparam name="T">The type being validated.</typeparam>
    /// <typeparam name="TProperty">The type of the property being validated.</typeparam>
    /// <param name="rule">The validation rule.</param>
    /// <param name="error">The error to use for message, code, and type.</param>
    /// <returns>The same rule builder for chaining.</returns>
    public static IRuleBuilderOptions<T, TProperty> WithError<T, TProperty>(
        this IRuleBuilderOptions<T, TProperty> rule,
        IError error)
    {
        return rule
            .WithMessage(error.Text)
            .WithErrorCode(error.Code)
            .WithState(_ => error.ErrorType);
    }
}