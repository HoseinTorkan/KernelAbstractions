using System.ComponentModel.DataAnnotations;

namespace KernelAbstractions.Application.Services.OtpService.Models;

/// <summary>
/// Defines the purpose of an OTP, used to select the appropriate
/// message template and validation rules.
/// </summary>
public enum OtpPurpose
{
    /// <summary>User login/authentication.</summary>
    [Display(Name = "Login")]
    Login = 1,

    /// <summary>SuperAdmin registering a new Admin.</summary>
    [Display(Name = "RegisterAdmin")]
    RegisterAdmin,

    /// <summary>Admin registering a new Seller.</summary>
    [Display(Name = "RegisterSeller")]
    RegisterSeller,

    /// <summary>Changing the user's phone number.</summary>
    [Display(Name = "ChangePhoneNumber")]
    ChangePhoneNumber,

    /// <summary>Resetting a forgotten password.</summary>
    [Display(Name = "ResetPassword")]
    ResetPassword,

    /// <summary>Two-factor authentication.</summary>
    [Display(Name = "TwoFactorAuth")]
    TwoFactorAuth
}