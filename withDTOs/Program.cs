using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


//adding dbcontext 
var connetionString = builder.Configuration.GetConnectionString("default");
builder.Services.AddDbContext<appDbContext>(options => {
    options
    .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()))
    .EnableSensitiveDataLogging()
    .UseSqlite(connetionString);
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/todo", () => {

});

app.Run();


public class Todo
{
    [Required]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    public bool IsComplete { get; set; }
}


public class TodoDTOs
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
}

public class appDbContext : DbContext
{
    public appDbContext(DbContextOptions<appDbContext> options) 
        : base(options)
        {}

    public DbSet<Todo> Todo {get; set;}
}
