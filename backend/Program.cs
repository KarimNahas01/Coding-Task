using Microsoft.EntityFrameworkCore;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.WebHost.ConfigureKestrel(options =>
{
    options.Listen(IPAddress.Any, 5000);
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Books API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseCors();
app.UseHttpsRedirection();

// GET
app.MapGet("/api/books", async (AppDbContext context) =>
{
    var books = await context.Books.ToListAsync();
    return Results.Ok(books);
});

// GET (by id)
app.MapGet("/api/books/{id:int}", async (int id, AppDbContext context) =>
{
    var book = await context.Books.FindAsync(id);
    return book == null ? Results.NotFound() : Results.Ok(book);
});

// PUT
app.MapPut("/api/books/{id:int}", async (int id, UpdateBook updatedBook, AppDbContext context) =>
{
    var book = await context.Books.FindAsync(id);
    if (book == null)
    {
        return Results.NotFound();
    }

    book.Title = updatedBook.Title;
    await context.SaveChangesAsync();

    return Results.NoContent();
});

// POST
app.MapPost("/api/books", async (Book book, AppDbContext context) =>
{
    context.Books.Add(book);
    await context.SaveChangesAsync();
    return Results.Created($"/api/books/{book.Id}", book);
});

// DELETE
app.MapDelete("/api/books/{id:int}", async (int id, AppDbContext context) =>
{
    var book = await context.Books.FindAsync(id);
    if (book == null)
    {
        return Results.NotFound();
    }

    context.Books.Remove(book);
    await context.SaveChangesAsync();

    return Results.NoContent();
});

app.Run();

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; }
}

public class UpdateBook
{
    public string Title { get; set; }
}

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Book> Books { get; set; }
}
