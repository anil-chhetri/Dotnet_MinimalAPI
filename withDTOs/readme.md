# Minimal API with Data Transfer Objects(DTOs)

## What is Data Transfer Objects?

A Data Transfer Object is an object that is used to encapsulate data, and send it from one subsystem of an application to another.
The motivation for its use is that communication between processes is usually done resorting to remote interfaces (e.g., web services), where each call is an expensive operation. Because the majority of the cost of each call is related to the round-trip time between the client and the server, one way of reducing the number of calls is to use an object (the DTO) that aggregates the data that would have been transferred by the several calls, but that is served by one call only

In simple term, generally application would not send every data recevied from database to UI, so intead of using database model to recevied and send those data to UI, we send those data using DTOs which is subset of the databse model. The main reason for using DTOs can be as follows:
    - Prevent over-posting.
    - Hide properties that clients are not supposed to view.
    - Omit some properties in order to reduce payload size.
    - Flatten object graphs that contain nested objects. Flattened object graphs can be more convenient for clients.

```Console
    dotnet add package Microsoft.EntityFrameworkCore --version 6.0.1
    dotnet add package Microsoft.EntityFrameworkCore.Sqlite --version 6.0.1
    dotnet add package Microsoft.EntityFrameworkCore.Design --version 6.0.1
```

Adding DbContext:

```csharp
    var connetionString = builder.Configuration.GetConnectionString("default");
    builder.Services.AddDbContext<appDbContext>(options => {
        options
        .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()))
        .EnableSensitiveDataLogging()
        .UseSqlite(connetionString);
});
```
