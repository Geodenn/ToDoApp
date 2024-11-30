using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Data;
using ToDoApp.Models;

var builder = WebApplication.CreateBuilder(args);

// Pobieranie connection string z konfiguracji aplikacji
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    // Wyrzucenie wyjątku, jeśli connection string nie został skonfigurowany
    throw new InvalidOperationException("Connection string 'DefaultConnection' nie jest skonfigurowany.");
}

builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));



// Konfiguracja Swaggera (narzędzie do dokumentowania i testowania API)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(config =>
{
    config.DocumentName = "ToDoAPP";
    config.Title = "ToDoAPP v1";
    config.Version = "v1";
});

var app = builder.Build();

// konfiguracja Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi(config =>
    {
        config.DocumentTitle = "ToDoAPP";
        config.Path = "/swagger";
        config.DocumentPath = "/swagger/{documentName}/swagger.json";
        config.DocExpansion = "list";
    });
}

// Endpoint GET - Pobieranie wszystkich zadań
app.MapGet("/todos", async (ToDoDbContext db) =>
{
    return await db.ToDos.ToListAsync(); // Zwracanie listy wszystkich rekordów z tabeli ToDos
});

// Endpoint GET - Pobieranie pojedynczego zadania po jego ID
app.MapGet("/todos/{id}", async (int id, ToDoDbContext db) =>
{
    var todo = await db.ToDos.FindAsync(id); // Szukanie zadania po ID w bazie
    if (todo is null)
        return Results.NotFound();

    return Results.Ok(todo);
});

// Endpoint GET - Pobieranie zadań na dzisiaj
app.MapGet("/todos/today", async (ToDoDbContext db) =>
{
    var today = DateTime.Today; // Dzisiejsza data (bez godziny)

    // Filtrowanie zadań, które mają termin realizacji dzisiaj
    var todayTodos = await db.ToDos
        .Where(todo => todo.ExpiryDate.Date == today) // Porównanie tylko daty
        .ToListAsync();

    return Results.Ok(todayTodos);
});

// Endpoint GET - Pobieranie zadań na jutro
app.MapGet("/todos/tomorrow", async (ToDoDbContext db) =>
{
    var tomorrow = DateTime.Today.AddDays(1); // Jutro

    // Filtrowanie zadań, które mają termin realizacji jutro
    var tomorrowTodos = await db.ToDos
        .Where(todo => todo.ExpiryDate.Date == tomorrow) // Porównanie tylko daty
        .ToListAsync();

    return Results.Ok(tomorrowTodos);
});

// Endpoint GET - Pobieranie zadań w nadchodzącym tygodniu
app.MapGet("/todos/nextweek", async (ToDoDbContext db) =>
{
    var today = DateTime.Today; // Dzisiejsza data (bez godziny)
    var nextWeek = today.AddDays(7); // Przyszły tydzień

    // Filtrowanie zadań, które mają termin realizacji w przyszłym tygodniu
    var nextWeekTodos = await db.ToDos
        .Where(todo => todo.ExpiryDate.Date > today && todo.ExpiryDate.Date <= nextWeek) // Zadania na nadchodzący tydzień
        .ToListAsync();

    return Results.Ok(nextWeekTodos);
});

// Endpoint POST - Dodawanie nowego zadania
app.MapPost("/todos", async (ToDo todo, ToDoDbContext db) =>
{
    var validationResults = new List<ValidationResult>();
    var validationContext = new ValidationContext(todo);

    // Walidacja obiektu
    if (!Validator.TryValidateObject(todo, validationContext, validationResults, true))
    {
        // Jeśli walidacja się nie powiedzie, zwróć błędy
        return Results.BadRequest(validationResults);
    }

    db.ToDos.Add(todo); // Dodanie nowego zadania do kontekstu bazy
    await db.SaveChangesAsync(); // Zapisanie zmian do bazy danych
    return Results.Created($"/todos/{todo.Id}", todo);
});

// Endpoint PUT - Aktualizowanie istniejącego zadania
app.MapPut("/todos/{id}", async (int id, ToDo updatedToDo, ToDoDbContext db) =>
{
    var todo = await db.ToDos.FindAsync(id); // Szukanie zadania po ID w bazie
    if (todo is null)
        return Results.NotFound();


    // Walidacja
    var validationResults = new List<ValidationResult>();
    var validationContext = new ValidationContext(updatedToDo);

    if (!Validator.TryValidateObject(updatedToDo, validationContext, validationResults, true))
    {
        return Results.BadRequest(validationResults);
    }


    // Aktualizowanie właściwości zadania
    todo.Title = updatedToDo.Title;
    todo.Description = updatedToDo.Description;
    todo.ExpiryDate = updatedToDo.ExpiryDate;
    todo.PercentComplete = updatedToDo.PercentComplete;
    todo.IsDone = updatedToDo.IsDone;

    await db.SaveChangesAsync(); // Zapisanie zmian do bazy danych
    return Results.NoContent();
});

// Endpoint DELETE - Usuwanie zadania po ID
app.MapDelete("/todos/{id}", async (int id, ToDoDbContext db) =>
{
    var todo = await db.ToDos.FindAsync(id); // Szukanie zadania po ID w bazie
    if (todo is null)
        return Results.NotFound();

    db.ToDos.Remove(todo); // Usunięcie zadania z kontekstu bazy
    await db.SaveChangesAsync(); // Zapisanie zmian do bazy danych
    return Results.NoContent();
});

app.Run();
