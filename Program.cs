using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using Backend.Models; 
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// CORS for frontend
builder.Services.AddCors(
        o => o.AddDefaultPolicy(p => 
        p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();
app.UseCors();

// var users = new List<User>();
// users.Add(new User
// {
//     id = new Random().Next(1000000000, int.MaxValue),
//     firstName = "Steve",
//     lastName = "Jobs",
//     email = "steve@apple.com",
//     password = "AAA"  
// });

app.MapGet("/users", () => {
    string json = File.ReadAllText("Users.json");

    var users = JsonSerializer.Deserialize<List<User>>(json);
    foreach (var user in users){
        Console.WriteLine(user);
    }
    
    return users;   
});

app.MapPost("/users", (User user) => {
    Console.WriteLine($"/users/{user.id}", user);

    // Write JSON to file
    string existingJson = File.ReadAllText("Users.json");
    List<User> users = JsonSerializer.Deserialize<List<User>>(existingJson);

    // Add the new user
    users.Add(user);

    // Write the entire updated array back to the file
    File.WriteAllText("Users.json", JsonSerializer.Serialize(users, new JsonSerializerOptions { 
        WriteIndented = true 
    }));

    //users.Add(user);
    return Results.Created($"/users/{user.id}", user);
});

// app.MapGet("/users/{id:int}", (int id) =>
// {
//     var user = users.FirstOrDefault(u => u.Id == id);
//     return user != null ? Results.Ok(user) : Results.NotFound();
// });

app.Run();
