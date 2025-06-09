using Microsoft.AspNetCore.Builder;
using Scalar.AspNetCore;
using UmbracoBridge.Infraestructure.Exceptions;
using UmbracoBridge.Infrastructure.DependencyInjection;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddUmbracoBridgeServices(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options =>
    {
        options.RouteTemplate = "/openapi/{documentName}.json";
    });
    app.MapScalarApiReference();
}

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.Run();