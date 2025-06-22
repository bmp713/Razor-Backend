using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Backend.Models; 

var builder = WebApplication.CreateBuilder(args);

// CORS for frontend
builder.Services.AddCors(
        o => o.AddDefaultPolicy(p => 
        p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();
app.UseCors();

var users = new List<User>();
users.Add(new User
{
    id = new Random().Next(1000000000, int.MaxValue),
    firstName = "Steve",
    lastName = "Jobs",
    email = "steve@apple.com",
    password = "AAA"  
});

Console.WriteLine("User created: " + users[0]);

app.MapGet("/users", () => {
    Console.WriteLine("Get all users = > " + users);
    return users;
});

app.MapPost("/users", (User user) => {
    users.Add(user);
    Console.WriteLine("User created: " + user);
    return Results.Created($"/users/{user.id}", user);
});

// GET /users/{id} - Get user by ID
// app.MapGet("/users/{id:int}", (int id) =>
// {
//     var user = users.FirstOrDefault(u => u.Id == id);
//     return user != null ? Results.Ok(user) : Results.NotFound();
// });

app.Run();
