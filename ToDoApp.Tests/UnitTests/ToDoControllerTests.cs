using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ToDoApp.Models;
using Xunit;

public class ToDoControllerTests
{
    [Fact]
    public void Test_TodoValidation()
    {
        // Przygotowanie obiektu
        var todo = new ToDo
        {
            Title = "Test Task",
            Description = "A sample task description.",
            ExpiryDate = DateTime.Now.AddDays(1), // Jutro
            PercentComplete = 101, // Błędny procent
            IsDone = false
        };

        // Walidacja
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(todo);

        bool isValid = Validator.TryValidateObject(todo, validationContext, validationResults, true);

        // Test: czy walidacja nie przepuszcza nieprawidłowego procentu
        Assert.False(isValid);
        Assert.Contains(validationResults, v => v.ErrorMessage.Contains("Percent Complete must be between 0 and 100"));
    }
}
