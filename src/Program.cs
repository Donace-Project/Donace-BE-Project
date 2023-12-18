using Donace_BE_Project.Extensions;
using Donace_BE_Project.Services;
using Donace_BE_Project.Services.RabbitMQ;
using Hangfire;

var builder = WebApplication.CreateBuilder(args);

var _configuration = builder.Configuration;

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureSwagger();
builder.Services.RegisterSettings(_configuration);
builder.Services.ConfigureCustomerSqlContext(_configuration);
builder.Services.ConfigureIdentity();
builder.Services.ConfigureJwt(_configuration);
builder.Services.RegisterAppServices(_configuration);
builder.Services.AddHostedService<RabbitMQService>();
builder.Services.AddHostedService<RabbitMQJoinEventService>();

var app = builder.Build();
app.UseCors(builder => builder
         .AllowAnyHeader()
         .AllowAnyMethod()
         .SetIsOriginAllowed((host) => true)
         .AllowCredentials()
     );

app.ConfigureExceptionHandler();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

// Doashboard management queue Hangfire
app.UseHangfireDashboard();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
