using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MojePszczoly.Infrastructure;
using MojePszczoly.Interfaces;
using MojePszczoly.Services;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);
var googleAuth = builder.Configuration.GetSection("Authentication:Google");
var adminEmails = builder.Configuration.GetSection("AdminEmails").Get<string[]>();

builder.Services.AddMemoryCache();
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = googleAuth["Authority"];
        options.UseSecurityTokenValidators = true;
        options.IncludeErrorDetails = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = googleAuth["Authority"],
            ValidAudience = googleAuth["ClientId"]
        }; 
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AllowedEmailsOnly", policy =>
        policy.RequireClaim(ClaimTypes.Email, adminEmails!));
});

builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IOrderReportService, OrderReportService>();
builder.Services.AddScoped<IDateService, DateService>();
builder.Services.AddScoped<IBreadService, BreadService>();
builder.Services.AddScoped<IDateOverrideService, DateOverrideService>();
builder.Services.AddSingleton<IClock, SystemClock>();
builder.Services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular",
        policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAngular");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
