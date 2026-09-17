using System.ComponentModel.DataAnnotations;

namespace KernelAbstractions.Patterns.Results;

/// <summary>
/// Defines the type of an error, indicating its nature and severity.
/// </summary>
/// <remarks>
/// Values are grouped by category and spaced numerically to allow future
/// additions without renumbering existing members:
/// <list type="bullet">
/// <item><description>0–9: None (no error)</description></item>
/// <item><description>10–19: Warnings</description></item>
/// <item><description>20–29: Validation errors</description></item>
/// <item><description>30–39: Entity errors</description></item>
/// <item><description>40–49: Security errors</description></item>
/// <item><description>50–59: Business rule violations</description></item>
/// <item><description>60–69: Infrastructure errors</description></item>
/// <item><description>70–79: Timeout and concurrency</description></item>
/// <item><description>80–99: General errors</description></item>
/// </list>
/// </remarks>
/// <seealso cref="IError"/>
/// <seealso cref="Error"/>
public enum ErrorType
{
    /// <summary>No error; the operation completed successfully.</summary>
    [Display(Name = "None")]
    None = 0,

    /// <summary>Operation completed with a warning.</summary>
    [Display(Name = "Warning")]
    Warning = 10,

    /// <summary>Validation warning; the input is acceptable but may need attention.</summary>
    [Display(Name = "Validation Warning")]
    ValidationWarning = 11,

    /// <summary>Validation error; the input is not valid.</summary>
    [Display(Name = "Validation Error")]
    ValidationError = 20,

    /// <summary>Invalid input provided.</summary>
    [Display(Name = "Invalid Input")]
    InvalidInput = 21,

    /// <summary>A required field is missing.</summary>
    [Display(Name = "Required Field")]
    RequiredField = 22,

    /// <summary>The format of the provided value is invalid.</summary>
    [Display(Name = "Invalid Format")]
    InvalidFormat = 23,

    /// <summary>The requested resource was not found.</summary>
    [Display(Name = "Not Found")]
    NotFound = 30,

    /// <summary>The resource already exists (duplicate).</summary>
    [Display(Name = "Duplicate")]
    Duplicate = 31,

    /// <summary>The operation conflicts with the current state.</summary>
    [Display(Name = "Conflict")]
    Conflict = 32,

    /// <summary>The resource is invalid.</summary>
    [Display(Name = "Invalid")]
    Invalid = 33,

    /// <summary>The user is not authenticated.</summary>
    [Display(Name = "Unauthorized")]
    Unauthorized = 40,

    /// <summary>The user does not have permission to perform the operation.</summary>
    [Display(Name = "Forbidden")]
    Forbidden = 41,

    /// <summary>The authentication token is invalid or expired.</summary>
    [Display(Name = "Invalid Token")]
    InvalidToken = 42,

    /// <summary>Too many failed attempts.</summary>
    [Display(Name = "Too Many Attempts")]
    TooManyAttempts = 43,

    /// <summary>A business rule was violated.</summary>
    [Display(Name = "Business Rule Violation")]
    BusinessRuleViolation = 50,

    /// <summary>Insufficient stock for the requested operation.</summary>
    [Display(Name = "Insufficient Stock")]
    InsufficientStock = 51,

    /// <summary>The current status does not allow the requested operation.</summary>
    [Display(Name = "Invalid Status")]
    InvalidStatus = 52,

    /// <summary>The amount is below the minimum allowed.</summary>
    [Display(Name = "Minimum Amount")]
    MinimumAmount = 53,

    /// <summary>The amount exceeds the maximum allowed.</summary>
    [Display(Name = "Maximum Amount")]
    MaximumAmount = 54,

    /// <summary>An infrastructure error occurred.</summary>
    [Display(Name = "Infrastructure Error")]
    Infrastructure = 60,

    /// <summary>A database error occurred.</summary>
    [Display(Name = "Database Error")]
    DatabaseError = 61,

    /// <summary>A network error occurred.</summary>
    [Display(Name = "Network Error")]
    NetworkError = 62,

    /// <summary>An external service error occurred.</summary>
    [Display(Name = "External Service Error")]
    ExternalServiceError = 63,

    /// <summary>A file-related error occurred.</summary>
    [Display(Name = "File Error")]
    FileError = 64,

    /// <summary>The operation timed out.</summary>
    [Display(Name = "Timeout")]
    Timeout = 70,

    /// <summary>A concurrency conflict occurred.</summary>
    [Display(Name = "Concurrency")]
    Concurrency = 71,

    /// <summary>A general error occurred.</summary>
    [Display(Name = "General Error")]
    GeneralError = 80,

    /// <summary>An unknown error occurred.</summary>
    [Display(Name = "Unknown Error")]
    UnknownError = 99
}