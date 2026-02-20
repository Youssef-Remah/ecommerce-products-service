using BusinessLogic;
using DataAccess;
using FluentValidation.AspNetCore;
using ProductsMicroservice.API.Middlewares;
using ProductsMicroservice.API.ProductsEndpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddControllers();

builder.Services.AddScoped<ExceptionHandlingMiddleware>();

builder.Services.AddDataAccess(builder.Configuration);

builder.Services.AddBusinessLogic();

var app = builder.Build();

app.UseExceptionHandlingMiddleware();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapProductAPIEndpoints();

app.Run();
