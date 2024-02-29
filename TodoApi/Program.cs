using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TodoApi;

// var builder = WebApplication.CreateBuilder(args);

// // הוספת הגדרות של CORS
// builder.Services.AddCors(options =>
// {
//     options.AddDefaultPolicy(builder =>
//     {
//         builder.AllowAnyOrigin()
//             .AllowAnyHeader()
//             .AllowAnyMethod();
//     });
// });

// // הוספת DbContext כ-Service
// builder.Services.AddDbContext<ToDoDbContext>(options =>
//     options.UseMySql("name=ToDoDB", ServerVersion.Parse("8.0.36-mysql")));

// // הוספת Swagger
// builder.Services.AddSwaggerGen(c =>
// {
//     c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
// });

// var app = builder.Build();

// // הגדרת ה-UseCors middleware
// app.UseCors();

// // הגדרת Swagger UI
// app.UseSwagger();
// app.UseSwaggerUI(c =>
// {
//     c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
// });
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors();
builder.Services.AddDbContext<ToDoDbContext>(options =>
{
    options.UseMySql(builder.Configuration.GetConnectionString("ToDoDB"), ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("ToDoDB")));
});
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// הגדרת ה-Middleware של CORS
app.UseCors(policy =>
{
    policy.AllowAnyOrigin(); 
    policy.AllowAnyMethod(); 
    policy.AllowAnyHeader(); 
});
// הגדרת Routes
app.MapGet("/api/items", async (ToDoDbContext context) =>
{
    return await context.Items.ToListAsync();
});

app.MapPost("/api/items", async (ToDoDbContext context, [FromBody] Item item) =>
{
    context.Items.Add(item);
    await context.SaveChangesAsync();
    return item;
});

app.MapPut("/api/items/{id}", async (ToDoDbContext context, int id, [FromBody] Item updatedItem) =>
{
    var item = await context.Items.FindAsync(id);
    if (item == null) return Results.NotFound();

    item.Name = updatedItem.Name;
    item.IsComplete = updatedItem.IsComplete;

    await context.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/api/items/{id}", async (ToDoDbContext context, int id) =>
{
    var item = await context.Items.FindAsync(id);
    if (item == null) return Results.NotFound();

    context.Items.Remove(item);
    await context.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();
