using Microsoft.EntityFrameworkCore;
using Np.UsersService.Core.Authentication.Keycloak;
using Np.UsersService.Core.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(
    opt=> opt.UseNpgsql(builder.Configuration.GetConnectionString("UsersDb")));

builder.Services.AddKeycloakAuthentication(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
