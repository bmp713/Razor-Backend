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

// GET all users
app.MapGet("/users", () => {
        var users = ReadUsersFromFile();
        return Results.Ok(users);
});

// POST new user
app.MapPost("/users", (User user) => {
        var users = ReadUsersFromFile();
        
        // Check if email already exists
        if (users.Any(u => u.email == user.email)){
            return Results.Conflict($"User with email {user.email} already exists");
        }
        
        // Ensure user has an ID
        if (user.id == 0){
            user.id = new Random().Next(1000000000, int.MaxValue);
        }
        
        // Add the new user
        users.Add(user);
        WriteUsersToFile(users);
        
        return Results.Created($"/users/{user.id}", user);
});

// GET user by ID
app.MapGet("/users/{id:int}", (int id) => {
        var users = ReadUsersFromFile();
        var user = users.FirstOrDefault(u => u.id == id);
        
        if (user == null){
            return Results.NotFound($"User with ID {id} not found");
        }
        
        return Results.Ok(user);
});

// PUT (update) user
app.MapPut("/users/{id:int}", (int id, User updatedUser) => {
        var users = ReadUsersFromFile();
        var userIndex = users.FindIndex(u => u.id == id);
        
        if (userIndex == -1){
            return Results.NotFound($"User with ID {id} not found");
        }
        
        // Ensure ID remains the same
        updatedUser.id = id;
        
        // Update the user
        users[userIndex] = updatedUser;
        WriteUsersToFile(users);
        
        return Results.Ok(updatedUser);
});

// DELETE user
app.MapDelete("/users/{id:int}", (int id) => {
        var users = ReadUsersFromFile();
        var user = users.FirstOrDefault(u => u.id == id);
        
        if (user == null){
            return Results.NotFound($"User with ID {id} not found");
        }
        users.Remove(user);
        WriteUsersToFile(users);
        
        return Results.Ok($"User with ID {id} deleted successfully");
});

// Helper function to read users from JSON file
List<User> ReadUsersFromFile(){    if (!File.Exists("Users.json")){
        
        // Create the file if it doesn't exist with an empty array
        File.WriteAllText("Users.json", "[]");
        return new List<User>();
    }
    string json = File.ReadAllText("Users.json");
    return JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
}

// Helper function to write users to JSON file
void WriteUsersToFile(List<User> users){
    File.WriteAllText("Users.json", JsonSerializer.Serialize(users, new JsonSerializerOptions { 
        WriteIndented = true 
    }));
}

app.Run();
