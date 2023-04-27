using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.Formatters;
using TransactionService.Api;
using TransactionService.Application;
using TransactionService.Infrastructure;

var CorsOrigins = "CorsOriginsAllowed";
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsOrigins, policy => { policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod(); });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// builder.Services.AddControllers().AddJsonOptions(x =>
//                 x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseRouting();
app.UseCors(CorsOrigins);
app.MapControllers();
app.UseMiddleware<ErrorHandlerMiddleware>();

using(var scope = app.Services.CreateScope()){
    var dbContextInitialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();
    await dbContextInitialiser.Migrate();
}

app.Run();