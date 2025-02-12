using System.ComponentModel.DataAnnotations;
using Client.Identity.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Client.Components.Pages.Identity;

public partial class Register
{
    private bool _registerProcessing;
    private EditForm _form = null!;
    private FormResult _formResult = new();
    [SupplyParameterFromForm] private InputModel Input { get; set; } = new();

    public async Task RegisterUserAsync()
    {
        if (_form.EditContext?.Validate() != true)
        {
            return;
        }

        _registerProcessing = true;
        StateHasChanged();

        await Task.Delay(1000); // to show loader
        _formResult = await AuthService.RegisterAsync(Input.Name, Input.Email, Input.Password);

        try
        {
            if (_formResult.Succeeded)
            {
                Navigation.NavigateTo("/");
            }
        }
        finally
        {
            _registerProcessing = false;
        }
    }

    public sealed class InputModel
    {
        [Required]
        [RegularExpression(
            "^(?=.{2,100}$)(?!.*[.,]{2,})(?=.*\\p{L})[\\p{L}\\p{M}.,]+(?: [\\p{L}\\p{M}.,]+)*$",
            ErrorMessage =
                "The provided name is invalid. It must consist solely of letters (from any language), may include single commas or periods (without consecutive punctuation), and spaces. Please ensure the name is between 2 and 100 characters long.")]
        [Display(Name = "Name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}