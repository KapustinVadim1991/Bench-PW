using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Client.Components.Pages.Identity;

namespace Tests;

public class UserTests
{
    private bool ValidateModel(object model, out List<ValidationResult> results)
    {
        var context = new ValidationContext(model, null, null);
        results = new List<ValidationResult>();
        return Validator.TryValidateObject(model, context, results, true);
    }

    [Theory]
    [InlineData("John Doe", true)]
    [InlineData("Мария Иванова", true)]
    [InlineData("李四", true)]
    [InlineData("John..Doe", false)]
    [InlineData("John,,Doe", false)]
    [InlineData("J", false)]
    [InlineData("", false)]
    public void TestRegex(string name, bool expectedIsValid)
    {
        var pattern = "^(?=.{2,100}$)(?!.*[.,]{2,})(?=.*\\p{L})[\\p{L}\\p{M}.,]+(?: [\\p{L}\\p{M}.,]+)*$";
        var reg = new Regex(pattern);

        // Assert
        Assert.Equal(expectedIsValid, reg.IsMatch(name));
    }

    [Theory]
    [InlineData("John Doe", true)]
    [InlineData("Мария Иванова", true)]
    [InlineData("李四", true)]
    [InlineData("John..Doe", false)]
    [InlineData("John,,Doe", false)]
    [InlineData("J", false)]
    [InlineData("", false)]
    public void TestNameValidation(string name, bool expectedIsValid)
    {
        // Arrange
        var user = new User
        {
            Name = name
        };

        // Act
        var isValid = ValidateModel(user, out var results);

        Assert.Equal(expectedIsValid, isValid);
    }

    public sealed class User
    {
        [Required]
        [RegularExpression(
            "^(?=.{2,100}$)(?!.*[.,]{2,})(?=.*\\p{L})[\\p{L}\\p{M}.,]+(?: [\\p{L}\\p{M}.,]+)*$",
            ErrorMessage = "The provided name is invalid. It must consist solely of letters (from any language), may include single commas or periods (without consecutive punctuation), and spaces. Please ensure the name is between 2 and 100 characters long.")]
        [Display(Name = "Name")]
        public string Name { get; set; } = string.Empty;
    }
}