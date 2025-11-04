using System.Reflection;
using System.Text.Json.Serialization;
using AutoMapper;
using Lafise.API.data;
using Lafise.API.services.Accounts;
using Lafise.API.services.Auth;
using Lafise.API.services.Auth.JWT;
using Lafise.API.services.clients;
using Lafise.API.utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;



namespace Lafise.API;

public class Program
{
   public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddCors(opt =>
            {
                opt.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            });

            //Agregar Servicios
            AddServices(builder);
            builder.Services.AddAutoMapper(cfg => { }, typeof(AutoMapperProfile));

            // Add services to the container.
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

           builder.Services.AddSwaggerGen(c =>
            {
                c.OperationFilter<SecurityRequirementsOperationFilter>();
                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Description = "Standard Authorization header using the Bearer scheme. Example: \"bearer {token}\"",
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }

                c.SwaggerDoc("v1", new OpenApiInfo 
                { 
                    Title = "Lafise API", 
                    Version = "v1",
                    Description = "API para el sistema bancario Lafise"
                });
            });

            var app = builder.Build();
                        
           

            
            string? env = builder.Configuration.GetValue<string>("env");
            if (app.Environment.IsDevelopment() || env == "DEV")
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Lafise API v1");
                });
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseCors("AllowAll");

            
            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }

        private static void AddServices(WebApplicationBuilder builder)
        {
            
            //Agregando EF DataContext             
            builder.Services.AddPooledDbContextFactory<BankDataContext>(
                           options => options.UseSqlite(builder.Configuration.GetValue<string>("db-cnstr-bank")));

            // Configurar AccountSettings desde appsettings.json
            builder.Services.Configure<AccountSettings>(
                builder.Configuration.GetSection(AccountSettings.SettingsName));

            // Registrar AccountSettings como singleton para acceso directo
            builder.Services.AddSingleton<AccountSettings>(sp =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                var settings = new AccountSettings();
                config.GetSection(AccountSettings.SettingsName).Bind(settings);
                return settings;
            });

            //Agregando servicios            
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            

            builder.Services.AddAuthorization();

            // Registrar servicios de utilidad
            builder.Services.AddScoped<ICryptor, Cryptor>();

            // Registrar servicios de autenticación
            builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            builder.Services.AddScoped<IAuthInfo, AuthInfo>();
            builder.Services.AddScoped<IAuthService, AuthService>();

            // Registrar servicios de negocio
            builder.Services.AddScoped<IClientService, ClientService>();
            builder.Services.AddScoped<IAccountService, AccountService>();



            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opt =>
                {
                    opt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = builder.Configuration.GetValue<string>("jwt-issuer"),
                        ValidateAudience = true,
                        ValidAudience = builder.Configuration.GetValue<string>("jwt-audience"),
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            System.Text.Encoding.UTF8.GetBytes(
                                    builder.Configuration.GetValue<string>("jwt-token-secret-key") 
                                    ?? throw new InvalidOperationException("JWT token secret key is not configured")
                            )
                        ),
                        ValidateLifetime = true,
                        LifetimeValidator = CustomLifetimeValidator
                    };
                });
           
        }

        private static bool CustomLifetimeValidator(DateTime? notBefore, DateTime? expires, SecurityToken token, TokenValidationParameters @params)
        {
            if (expires != null)
            {
                return expires > DateTime.UtcNow;
            }
            return false;
        }
}