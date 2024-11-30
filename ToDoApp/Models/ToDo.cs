using System.ComponentModel.DataAnnotations;

namespace ToDoApp.Models;

public class ToDo
{
    //Unikalne ID dla zadania
    public int Id { get; set; }

    //Tytuł zadania, string.Empty w celu uniknięcia Null, ustawienie limitu znaków
    [Required]
    [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
    public string Title { get; set; } = string.Empty;
    //Opis zadania, string.Empty w celu uniknięcia Null, ustawienie limitu znaków

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
    public string Description { get; set; } = string.Empty;

    //Data, do której zadanie powinno zostać zrealizowane
    [Required]
    public DateTime ExpiryDate { get; set; }

    //Aktualny stan zadania, walidacja danych
    [Range(0, 100, ErrorMessage = "Percent Complete must be between 0 and 100.")]
    public int PercentComplete { get; set; }

    //Stan określający, czy zadanie zostało ukończone
    public bool IsDone { get; set; }
}
