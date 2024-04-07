using IdentityApi.Database;
using IdentityApi.Services.Implementations;
using IdentityApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IIdentityService,IdentityService>();

string dbConnection = builder.Configuration.GetConnectionString("DbConnection")!;
builder.Services.AddDbContext<IdentityDbContext>(options => options.UseSqlServer(dbConnection));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
