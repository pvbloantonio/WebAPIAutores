using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Text.Json.Serialization;
using WebAPIAutores;
using WebAPIAutores.Entidades;
using WebAPIAutores.Filtros;
using WebAPIAutores.Middlewares;
using WebAPIAutores.Servicios;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddControllers(opciones =>
{
    opciones.Filters.Add(typeof(FiltroDeExcepcion));
});

builder.Services.AddControllers().AddNewtonsoftJson();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opciones => opciones.TokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuer = false,
    ValidateAudience = false,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(
        Encoding.UTF8.GetBytes(builder.Configuration["llavejwt"])),
        ClockSkew = TimeSpan.Zero
});

// DBContext

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("defaultConnection"));
});

// Cors
builder.Services.AddCors(options => options.AddPolicy("AllowWebapp",
                                    builder => builder.AllowAnyOrigin()
                                                      .AllowAnyHeader()
                                                      .AllowAnyMethod()));


builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
builder.Services.AddAuthorization(opciones =>
{
    opciones.AddPolicy("EsAdmin", politica => politica.RequireClaim("esAdmin"));
    //opciones.AddPolicy("EsVendedor", politica => politica.RequireClaim("esVendedor"));
});

builder.Services.AddDataProtection();

builder.Services.AddTransient<HashService>();

builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
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
            new string[] {}
        }
    }); 
    
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseLoggearRespuestaHTTP();
//app.UseMiddleware<LoggearRespuestaHTTPMiddleware>();



if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    
}



app.UseHttpsRedirection();

app.UseCors("AllowWebapp");

app.UseAuthorization();

app.MapControllers();

app.Run();
