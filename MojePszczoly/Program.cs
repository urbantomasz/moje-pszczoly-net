using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using MojePszczoly.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "https://accounts.google.com";
        options.Audience = "343060699496-jjh4qrfv58g6ddkncqv75ssdm4r7u0ut.apps.googleusercontent.com";
        options.TokenValidationParameters.ValidateIssuer = true;
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AllowedEmailsOnly", policy =>
        policy.Requirements.Add(new AllowedEmailsRequirement()));
});

builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
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

app.UseCors("AllowAngular");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
