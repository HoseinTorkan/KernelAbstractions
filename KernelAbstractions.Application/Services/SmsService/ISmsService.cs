using KernelAbstractions.Patterns.Results;

namespace KernelAbstractions.Application.Services.SmsService;

/// <summary>
/// Contract for sending SMS messages.
/// </summary>
/// <remarks>
/// Implementations are provider-agnostic (Kavenegar, SMS.ir, Twilio, etc.).
/// </remarks>
public interface ISmsService
{
    /// <summary>
    /// Sends an SMS message to the specified phone number.
    /// </summary>
    /// <param name="phoneNumber">Destination phone number.</param>
    /// <param name="message">Message content.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Success, or a failure describing why the message could not be sent.</returns>
    Task<Result> SendAsync(
        string phoneNumber,
        string message,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends an SMS message to multiple recipients.
    /// </summary>
    /// <param name="phoneNumbers">Destination phone numbers.</param>
    /// <param name="message">Message content.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Success, or a failure describing why messages could not be sent.</returns>
    Task<Result> SendBulkAsync(
        IEnumerable<string> phoneNumbers,
        string message,
        CancellationToken cancellationToken = default);
}