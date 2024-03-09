using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using OnlineAuctionWeb.Application;
using OnlineAuctionWeb.Domain;
using OnlineAuctionWeb.Domain.Mappings;
using OnlineAuctionWeb.Infrastructure.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Thêm dòng sau để thêm token JWT
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter the token into the field",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
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
            new string[] { }
        }
    });
});

builder.Services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);
builder.Services.AddApplication(builder.Configuration);

var app = builder.Build();

app.UseCors(builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();
});

app.MapGet("/", async context =>
{
    await context.Response.WriteAsync("Hello World!");
});

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    c.InjectJavascript("/custom.js");
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseMiddleware<CustomExceptionMiddleware>();
app.MapControllers();
app.Run();
