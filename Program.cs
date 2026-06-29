<<<<<<< HEAD
﻿using System.Text.Json;
using Microsoft.EntityFrameworkCore;
=======
using System.Text.Json;
>>>>>>> parent of 07b22e8 (add api query by ID)
using System.Text.Json.Serialization;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        // เพิ่ม CORS policy
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAllOrigins", policy =>
            {
                policy.AllowAnyOrigin()    // อนุญาตทุกโดเมน
                      .AllowAnyMethod()    // อนุญาตทุก HTTP method (GET, POST, PUT, DELETE)
                      .AllowAnyHeader();   // อนุญาตทุก HTTP header
            });
        });

        // เพิ่มบริการ JSON options
        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new ThaiDateTimeConverter());
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            });
        
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddHttpClient<AuthService>();
        builder.Services.AddDbContext<Backend.Models.AppDbContext>(options =>
        {
            var connStr = builder.Configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(connStr))
        {
        throw new InvalidOperationException("Connection string 'DefaultConnection' is missing or empty in appsettings.json. Add a valid PostgreSQL connection string under \"ConnectionStrings\".");
        }
        options.UseNpgsql(connStr);
        });

        var app = builder.Build();

        // ใช้ CORS policy ที่อนุญาตทุกโดเมน
        app.UseCors("AllowAllOrigins");

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}
