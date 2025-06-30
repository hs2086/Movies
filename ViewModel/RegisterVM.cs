using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace MovieApp.ViewModel
{
    public class RegisterVM
    {
        [Required(ErrorMessage = "First name is required.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Email is required")]
        [Remote(action: "IsEmailInUse", controller: "Account")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number.")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        public string Address { get; set; }

        // For verification step
        public string VerificationCode { get; set; }

        // For password setup step
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords don't match.")]
        public string ConfirmPassword { get; set; }

        public int Step { get; set; } = 1; // 1=Basic info, 2=Verification, 3=Password
    }
}