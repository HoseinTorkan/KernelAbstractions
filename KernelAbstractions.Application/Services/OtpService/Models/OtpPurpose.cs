namespace KernelAbstractions.Application.Services.OtpService;

/// <summary>
/// Defines the purpose of an OTP, used to select the appropriate
/// message template and validation rules.
/// </summary>
public enum OtpPurpose
{
    /// <summary>User login/authentication.</summary>
    Login = 0,

    /// <summary>SuperAdmin registering a new Admin.</summary>
    RegisterAdmin = 1,

    /// <summary>Admin registering a new Seller.</summary>
    RegisterSeller = 2,

    /// <summary>Changing the user's phone number.</summary>
    ChangePhoneNumber = 3,

    /// <summary>Resetting a forgotten password.</summary>
    ResetPassword = 4,

    /// <summary>Two-factor authentication.</summary>
    TwoFactorAuth = 5
}