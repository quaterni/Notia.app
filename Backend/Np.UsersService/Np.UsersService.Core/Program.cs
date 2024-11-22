using Microsoft.EntityFrameworkCore;
using Np.UsersService.Core.Authentication.Keycloak;
using Np.UsersService.Core.Business;
using Np.UsersService.Core.Data;
using Np.UsersService.Core.Grpc;
using Np.UsersService.Core.Messaging.MessageHandling;
using Np.UsersService.Core.Messaging.Outbox;
using Np.UsersService.Core.Messaging.RabbitMq;
using Np.UsersService.Core.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(
    opt=> opt.UseNpgsql(builder.Configuration.GetConnectionString("UsersDb"))
             .UseSnakeCaseNamingConvention());

builder.Services.AddKeycloakAuthentication(builder.Configuration);

builder.Services.AddRabbitMqMessaging(builder.Configuration);

builder.Services.AddOutboxEntries(builder.Configuration);

builder.Services.AddBusiness();

builder.Services.AddMessageHandling();

builder.Services.AddGrpc();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<CustomExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapGrpcServices();

app.Run();
