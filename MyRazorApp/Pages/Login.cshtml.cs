using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using MyRazorApp.Models;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations; // Add this for data annotations

namespace MyRazorApp.Pages
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;

        public LoginModel(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }

        [BindProperty]
        [Required(ErrorMessage = "Username is required")] // Added Required attribute
        public string Username { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Password is required")] // Good practice to also have this for Password
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string ErrorMessage { get; set; }

        public IActionResult OnGet()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToPage("Index");
            
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Check if the model state is valid (i.e., if [Required] attributes are satisfied)
            if (!ModelState.IsValid)
            {
                // If not valid, return the page. Validation messages will be displayed
                // by the <span asp-validation-for="Username"></span> in your .cshtml
                return Page();
            }

            // Proceed with login attempt only if ModelState is valid
            var result = await _signInManager.PasswordSignInAsync(
                Username, Password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return RedirectToPage("Index");
            }
            else
            {
                // If PasswordSignInAsync fails, it's not due to a missing username at this point,
                // but rather an invalid login attempt.
                if (result.IsLockedOut)
                {
                    ErrorMessage = "This account has been locked out. Please try again later.";
                }
                else if (result.IsNotAllowed)
                {
                    ErrorMessage = "Login is not allowed for this user. Please confirm your email if required.";
                }
                else
                {
                    ErrorMessage = "Invalid username or password.";
                }
                // Add the error to ModelState to display it with other validation summaries if you have one,
                // or rely on the ErrorMessage property as you are.
                // ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return Page();
            }
        }
    }
}