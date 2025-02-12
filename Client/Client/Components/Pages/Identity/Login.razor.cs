using System.ComponentModel.DataAnnotations;
using Client.Components.Pages.Base;
using Client.Identity.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Client.Components.Pages.Identity;

public partial class Login : AnonymousPage
{
    private EditForm _form = null!;
    private FormResult _formResult = new();
    private bool _loginProcessing;
    private InputModel Input { get; set; } = new();
    [SupplyParameterFromQuery] private string? ReturnUrl { get; set; }

    public async Task LoginUserAsync()
    {
        if (_form.EditContext?.Validate() != true)
        {
            return;
        }

        _loginProcessing = true;
        StateHasChanged();

        await Task.Delay(1000); // just to show the loader on the button
        _formResult = await AuthService.LoginAsync(Input.Email, Input.Password);

        _loginProcessing = false;
        if (_formResult.Succeeded)
        {
            Navigation.NavigateTo(ReturnUrl ?? "/");
        }
    }

    private void NavigateToRegistration()
    {
        Navigation.NavigateTo("register");
    }

    private sealed class InputModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;
    }
}