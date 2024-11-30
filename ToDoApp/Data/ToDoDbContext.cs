using Microsoft.EntityFrameworkCore;
using ToDoApp.Models;

namespace ToDoApp.Data
{
    // Implementacja klasy kontekstu bazy danych w celu modyfikacji danych
    public class ToDoDbContext : DbContext
    {
        public ToDoDbContext(DbContextOptions<ToDoDbContext> options) : base(options) { }

        public DbSet<ToDo> ToDos { get; set; } //Tworzenie tabeli w bazie z zadaniami z polami modelu ToDo
    }
}
