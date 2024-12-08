using APIwebmoi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using ApiWebManagement.Service;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost", builder =>
        builder.WithOrigins("https://localhost:44320")  // Địa chỉ frontend
               .AllowAnyHeader()
               .AllowAnyMethod());
});
builder.Services.AddControllersWithViews();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.WithOrigins("https://localhost:7217")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
            .Build();
    });

});

builder.Services.AddControllers();


// Configure JWT Authentication
var config = builder.Configuration;
//var jwtKey = config["Jwt:Key"];
//var jwtIssuer = config["Jwt:Issuer"];
//var jwtAudience = config["Jwt:Audience"];

if (string.IsNullOrEmpty(config["Jwt:Key"]) || string.IsNullOrEmpty(config["Jwt:Issuer"]) || string.IsNullOrEmpty(config["Jwt:Issuer"]))
{
    throw new InvalidOperationException("JWT configuration is missing in appsettings.json.");
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = config["Jwt:Issuer"],
            ValidAudience = config["Jwt:Audience"], // Kiểm tra lại giá trị này
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]))

        };


        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"Authentication Failed: {context.Exception.Message}");
                Console.WriteLine($"Authorization Header: {context.Request.Headers["Authorization"]}");
                return System.Threading.Tasks.Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine("Token successfully validated.");
                return System.Threading.Tasks.Task.CompletedTask;
            }
        };

    }
);

// Configure Swagger with JWT Bearer Authorization
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API Web Management",
        Version = "v1",
        Description = "API documentation for Web Management",
        Contact = new OpenApiContact
        {
            Name = "Your Name",
            Email = "your-email@example.com"
        }
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and your token."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });

    c.DescribeAllParametersInCamelCase();
    c.CustomSchemaIds(type => type.FullName);
});
builder.Services.AddDistributedMemoryCache();
// Configure Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Session timeout
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Configure Database Context
builder.Services.AddDbContext<WebNangCaoQlda2Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("connection")));

// Register Services
builder.Services.AddScoped<UserService>();

var app = builder.Build();
app.UseCors("AllowLocalhost");
// Configure the HTTP request pipeline
//app.UseCors("CorsPolicy");


if (builder.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Web Management v1");
        c.RoutePrefix = string.Empty; // Make Swagger available at root
    });
}

app.UseHttpsRedirection();

app.UseSession(); // Enable Session middleware

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
// Thêm middleware cho MVC
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.Use(async (context, next) =>
{
    if (context.Request.Headers.TryGetValue("Authorization", out var authHeader))
    {
        var token = authHeader.ToString().Replace("Bearer ", "").Trim();
        Console.WriteLine($"Token received: {token}");
    }
    else
    {
        Console.WriteLine("Authorization header missing or invalid.");
    }

    await next();
});




app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers(); 
});

app.Run();
