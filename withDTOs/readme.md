# Minimal API with Data Transfer Objects(DTOs)

## What is Data Transfer Objects?

A Data Transfer Object is an object that is used to encapsulate data, and send it from one subsystem of an application to another.
The motivation for its use is that communication between processes is usually done resorting to remote interfaces (e.g., web services), where each call is an expensive operation. Because the majority of the cost of each call is related to the round-trip time between the client and the server, one way of reducing the number of calls is to use an object (the DTO) that aggregates the data that would have been transferred by the several calls, but that is served by one call only

In simple term, generally application would not send every data recevied from database to UI, so intead of using database model to recevied and send those data to UI, we send those data using DTOs which is subset of the databse model. The main reason for using DTOs can be as follows:
    - Prevent over-posting.
    - Hide properties that clients are not supposed to view.
    - Omit some properties in order to reduce payload size.
    - Flatten object graphs that contain nested objects. Flattened object graphs can be more convenient for clients.

## Ends points of the API

| Http Verb | EndPoint       | Status Codes                           | Descriptions            |
|-----------|----------------|----------------------------------------|-------------------------|
| POST      | /Todo          | 201 Created                            | Create a todo items     |
| GET       | /Todo/{Id:int} | 200 Ok, 404 Not found, 400 Bad Request | get todo item by ID     |
| GET       | /Todo          | 200 Ok                                 | get all todo items      |
| PUT       | /Todo/{Id:int} | 200 Ok, 404 Not found, 400 Bad Request | updates todo item by Id |
| DELETE    | /Todo/{Id:int} | 204 No Content, 404 Not found          | Delete todo item by Id  |

Adding necessary Nuget package

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

In above code by using ``AddDbContext`` we are injecting ``appDbContext`` class to our project which inherits ``DbContext`` and contains information about table need for our projects. After using ``AddDbContext`` now we can get access to our ``appDbcontext`` class around the application either it can be accessed in constructor or in our class method using ``FromService`` attributes. we can also use our `appDbContext` in other places but need to do some over work for that.
And Logger facory is used to get the SQL queries that application used to query that database and `EnableSensitiveDataLogging` is used to get the input information used by EF core.

examples

```sql
Executed DbCommand (1ms) [Parameters=[@__p_0='1'], CommandType='Text', CommandTimeout='30']
      SELECT "t"."Id", "t"."IsComplete", "t"."Name"
      FROM "Todo" AS "t"
      WHERE "t"."Id" = @__p_0
      LIMIT 1
```

EF core Database Migration

```Console
    dotnet ef migrations add <MigrationsName>
    dotnet ef database update
```

final output
![final output](./Screenshot%202022-01-23%20225442.png)
