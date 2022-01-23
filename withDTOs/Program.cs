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

app.MapGet("/todo", (appDbContext context) => {
    return Results.Ok(context.Todo);
});


app.MapGet("/todo/{Id}", (int Id, appDbContext context) => {
    if(Id < 0)
    {
        return Results.BadRequest();
    }

    var fromdb = context.Todo.Find(Id);
    if(fromdb == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(fromdb);
});

app.MapPost("/todo", (TodoDTOs todo, appDbContext context) => {
    var newtodo = new Todo()
    {
        Name = todo.Name,
        IsComplete = false
    };
    context.Todo.Add(newtodo);
    context.SaveChanges();
    todo.Id = newtodo.Id;
    
    return Results.Created($"/todo/{todo.Id}", todo);
});


app.MapPut("/todo/{Id}", (int Id, TodoDTOs updateTodo, appDbContext context) => {
    
    if(updateTodo.Id != Id)
    {
        return Results.BadRequest();
    }
    
    var fromdb = context.Todo.FirstOrDefault(t => t.Id == Id);
    if(fromdb == null)
    {
        return Results.NotFound();
    }
    
    fromdb.Name = updateTodo.Name;
    fromdb.IsComplete = updateTodo.IsComplete;

    context.SaveChanges();
    return Results.Ok(fromdb);
    
});


app.MapDelete("/todo/{Id}", (int Id, appDbContext context) => {
    var fromdb = context.Todo.FirstOrDefault(t => t.Id == Id);

    if(fromdb == null)
    {
        return Results.NotFound();
    }

    context.Todo.Remove(fromdb);
    context.SaveChanges();

    return Results.NoContent();

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

    public string? Name { get; set; }
    public bool IsComplete { get; set; }

}

public class appDbContext : DbContext
{
    public appDbContext(DbContextOptions<appDbContext> options) 
        : base(options)
        {}

    public DbSet<Todo> Todo {get; set;}
}
