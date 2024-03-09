using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Server.Models.GraphQl;
using TimetableServer.Database;
using TimetableServer.HelperClasses;
using TimetableServer.Services.Implementations;
using TimetableServer.Services.Interfaces;


var builder = WebApplication.CreateBuilder(args);
string dbConnection = builder.Configuration.GetConnectionString("DbConnection")!;
builder.Services
    .AddControllers()
    .AddNewtonsoftJson(x => 
        x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
    );


builder.Services.AddEndpointsApiExplorer();
builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>();

builder.Services.AddSwaggerGen();
builder.Services.AddCors();
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = AuthOptions.ISSUER,
            ValidateAudience = true,
            ValidAudience = AuthOptions.AUDIENCE,
            ValidateLifetime = true,
            IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
            ValidateIssuerSigningKey = true
        };
    });
builder.Services.AddDbContext<TimetableDBContext>(options => options.UseSqlServer(dbConnection));
builder.Services.AddTransient<IIdentityService, IdentityService>();
builder.Services.AddTransient<ILessonService, LessonService>();

var app = builder.Build();
app.UseCors(builder =>
{
    builder.AllowAnyMethod().AllowAnyHeader().SetIsOriginAllowed(origin => true).AllowCredentials();
});

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.MapControllers();
app.MapGraphQL();

app.Run();
public partial class Program{}