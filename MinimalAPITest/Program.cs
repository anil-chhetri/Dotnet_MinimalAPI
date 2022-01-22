using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// injecting dbcontext.
var connetionString = builder.Configuration.GetConnectionString("default");
builder.Services.AddDbContext<ApplicationDbContext>(options => {
    options
    .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()))
    .EnableSensitiveDataLogging()
    .UseSqlServer(connetionString);

}); 


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//create operations post
app.MapPost("/Gadgets", async(Gadget gadget, ApplicationDbContext context) => { 

    try {
    await context.Gadgets.AddAsync(gadget);
    await context.SaveChangesAsync();
    Results.CreatedAtRoute("getGadget", new {Id = gadget.Id }, gadget);
    }
    catch(Exception)
    {
        Results.BadRequest();
    }
});

// get all the gadgets 

app.MapGet("/Gadgets", (ApplicationDbContext context) => {
    return Results.Ok(context.Gadgets);
}).WithName("GetAllGadgets");


//get by id 
app.MapGet("/Gadgets/{Id}", (int Id, ApplicationDbContext context) => {
    if(Id < 1)
    {
        return Results.BadRequest();
    }

    var fromdb = context.Gadgets.FirstOrDefault(g => g.Id == Id);
    if(fromdb == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(fromdb);
}).WithMetadata(new EndpointNameMetadata("getGadget"));

//update gadets
app.MapPut("/Gadgets", async (Gadget UpdatedGadget, ApplicationDbContext context) => {
    var fromdb = await context.Gadgets.FindAsync(UpdatedGadget.Id);
    if(fromdb == null)
    {
        return Results.NotFound();
    }
    context.Entry<Gadget>(UpdatedGadget).State = EntityState.Modified;
    await context.SaveChangesAsync();

    return Results.NoContent();
});

app.MapDelete("/Gadgets/{Id}", async (int Id, ApplicationDbContext context) => {
    if(Id < 1)
    {
        return Results.BadRequest();
    }

    var fromdb = await context.Gadgets.FindAsync(Id);
    if(fromdb == null)
    {
        return Results.NotFound();
    }

    context.Gadgets.Remove(fromdb);
    await context.SaveChangesAsync();

    return Results.NoContent();
});


app.Run();



#region Dbcontext and model
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
        : base(options)
    { }
    public DbSet<Gadget> Gadgets { get; set; }
}


public record Gadget {

    [Required]
    [Key]
    public int Id { get; init; }

    [Required]
    public string? ProductName { get; init; } = string.Empty;

    public string? Brand { get; init; }

    [DataType("decimal(18,2)")]
    public decimal Cost { get; set; }

    public string? Type { get; set; }

}

#endregion