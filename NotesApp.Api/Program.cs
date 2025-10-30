using NotesApp.Api.Data;
using NotesApp.Api.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Register services
builder.Services.AddSingleton<DapperContext>();
builder.Services.AddScoped<INoteRepository, NoteRepository>();

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

builder.Services.AddAuthorization();
builder.Services.AddControllers();

var app = builder.Build();

// --- Dapper migration: create tables if not exist ---
// WRAPPED IN TRY-CATCH SO STARTUP DOESN'T FAIL
// try
// {
//     using var scope = app.Services.CreateScope();
//     var context = scope.ServiceProvider.GetRequiredService<DapperContext>();
//     using var conn = context.CreateConnection();

//     Console.WriteLine("🔄 Attempting database connection...");
//     conn.Open();

//     var createUsers = @"
//         IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Users' and xtype='U')
//         CREATE TABLE Users (
//             Id INT IDENTITY(1,1) PRIMARY KEY,
//             Username NVARCHAR(100) NOT NULL UNIQUE,
//             Email NVARCHAR(200) NOT NULL UNIQUE,
//             PasswordHash NVARCHAR(200) NOT NULL,
//             CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
//         );
//     ";
//     var createNotes = @"
//         IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Notes' and xtype='U')
//         CREATE TABLE Notes (
//             Id INT IDENTITY(1,1) PRIMARY KEY,
//             UserId INT NOT NULL,
//             Title NVARCHAR(200) NOT NULL,
//             Content NVARCHAR(MAX) NULL,
//             CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
//             UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
//             FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
//         );
//     ";

//     using var cmd1 = conn.CreateCommand();
//     cmd1.CommandText = createUsers;
//     cmd1.ExecuteNonQuery();

//     using var cmd2 = conn.CreateCommand();
//     cmd2.CommandText = createNotes;
//     cmd2.ExecuteNonQuery();

//     conn.Close();
//     Console.WriteLine("✅ Database tables created/verified successfully");
// }
// catch (Exception ex)
// {
//     Console.WriteLine($"⚠️ Database migration failed: {ex.Message}");
//     Console.WriteLine("📝 App will still start, but database operations may fail");
//     // Don't crash - let the app start anyway
// }

// Middleware pipeline
app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

// Add health check endpoints
app.MapGet("/", () => "API is running");
app.MapGet("/health", () => Results.Ok(new
{
    status = "healthy",
    timestamp = DateTime.UtcNow
}));

app.MapControllers();

app.Run();
