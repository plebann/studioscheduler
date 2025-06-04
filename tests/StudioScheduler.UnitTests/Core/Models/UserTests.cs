using Xunit;
using StudioScheduler.Core.Models;
using StudioScheduler.Core.Enums;
using StudioScheduler.Core.Validators;

namespace StudioScheduler.UnitTests.Core.Models;

public class UserTests
{
    [Fact]
    public void User_Creation_Sets_Default_Values()
    {
        // Arrange & Act
        var user = new User
        {
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            PasswordHash = "hashedpassword123"
        };

        // Assert
        Assert.Equal(UserRole.Student, user.Role);
        Assert.False(string.IsNullOrEmpty(user.Email));
        Assert.False(string.IsNullOrEmpty(user.FirstName));
        Assert.False(string.IsNullOrEmpty(user.LastName));
        Assert.False(string.IsNullOrEmpty(user.PasswordHash));
    }

    [Theory]
    [InlineData("", "Doe", "John", "hashedpassword123", "Email is required")]
    [InlineData("test@example.com", "", "John", "hashedpassword123", "LastName is required")]
    [InlineData("test@example.com", "Doe", "", "hashedpassword123", "FirstName is required")]
    [InlineData("test@example.com", "Doe", "John", "", "PasswordHash is required")]
    public void User_Requires_Essential_Fields(string email, string lastName, string firstName, string passwordHash, string expectedError)
    {
        // Arrange
        var validator = new UserValidator();
        var user = new User
        {
            Email = email,
            LastName = lastName,
            FirstName = firstName,
            PasswordHash = passwordHash
        };

        // Act
        var result = validator.Validate(user);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => 
            error.ErrorMessage.Contains(expectedError));
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("invalid@")]
    [InlineData("@invalid.com")]
    public void User_Email_Must_Be_Valid(string invalidEmail)
    {
        // Arrange
        var validator = new UserValidator();
        var user = new User
        {
            Email = invalidEmail,
            FirstName = "John",
            LastName = "Doe",
            PasswordHash = "hashedpassword123"
        };

        // Act
        var result = validator.Validate(user);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => 
            error.ErrorMessage.Contains("Invalid email format"));
    }
}
