using NotesApp.Api.Data;
using NotesApp.Api.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Register services
builder.Services.AddSingleton<DapperContext>();
builder.Services.AddScoped<INoteRepository, NoteRepository>();

// Configure CORS (allow frontend on localhost:5173)
// builder.Services.AddCors(options =>
// {
//     options.AddPolicy("AllowFrontend", policy =>
//     {
//         policy.WithOrigins("http://localhost:5173")
//               .AllowAnyHeader()
//               .AllowAnyMethod();
//     });
// });
// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// JWT config
var jwtSettings = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["Key"]!)
            )
        };
    });

// Authorization
builder.Services.AddAuthorization();

// Controllers
builder.Services.AddControllers();


// --- Dapper migration: create tables if not exist ---
using (var scope = builder.Services.BuildServiceProvider().CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DapperContext>();
    using var conn = context.CreateConnection();
    conn.Open();
    var createUsers = @"
        IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Users' and xtype='U')
        CREATE TABLE Users (
            Id INT IDENTITY(1,1) PRIMARY KEY,
            Username NVARCHAR(100) NOT NULL UNIQUE,
            Email NVARCHAR(200) NOT NULL UNIQUE,
            PasswordHash NVARCHAR(200) NOT NULL,
            CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
        );
    ";
    var createNotes = @"
        IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Notes' and xtype='U')
        CREATE TABLE Notes (
            Id INT IDENTITY(1,1) PRIMARY KEY,
            UserId INT NOT NULL,
            Title NVARCHAR(200) NOT NULL,
            Content NVARCHAR(MAX) NULL,
            CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
            UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
            FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
        );
    ";
    using var cmd1 = conn.CreateCommand();
    cmd1.CommandText = createUsers;
    cmd1.ExecuteNonQuery();
    using var cmd2 = conn.CreateCommand();
    cmd2.CommandText = createNotes;
    cmd2.ExecuteNonQuery();
    conn.Close();
}

var app = builder.Build();

// Middleware pipeline
app.UseHttpsRedirection();

// app.UseCors("AllowFrontend");
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
