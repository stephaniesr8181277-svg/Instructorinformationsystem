using System.ComponentModel.DataAnnotations;

namespace InstructorInformationSystem.Models;

public class LoginViewModel
{
    [Required(ErrorMessage = "E-mail address is required.")]
    [EmailAddress(ErrorMessage = "Enter a valid e-mail address.")]
    [Display(Name = "E-mail Address")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Remember me")]
    public bool RememberMe { get; set; }
}
