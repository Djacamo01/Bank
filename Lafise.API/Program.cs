using System.Reflection;
using System.Text.Json.Serialization;
using AutoMapper;
using Lafise.API.data;
using Lafise.API.services.Accounts;
using Lafise.API.services.Accounts.Factories;
using Lafise.API.services.Accounts.Mappers;
using Lafise.API.services.Accounts.Repositories;
using Lafise.API.services.Accounts.Services;
using Lafise.API.services.Accounts.Validators;
using Lafise.API.services.Auth;
using Lafise.API.services.Auth.JWT;
using Lafise.API.services.clients;
using Lafise.API.services.Transactions;
using Lafise.API.services.Transactions.Factories;
using Lafise.API.services.Transactions.Repositories;
using Lafise.API.services.Transactions.Validators;
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
                // Configuración de seguridad JWT
                c.OperationFilter<SecurityRequirementsOperationFilter>();
                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Description = "Autenticación JWT Bearer Token. Incluir el token en el header Authorization como: \"Bearer {token}\"",
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT"
                });

                // Incluir comentarios XML con soporte para controladores
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
                }

                
                c.SwaggerDoc("v1", new OpenApiInfo 
                { 
                    Title = "Lafise Banking System API - Prueba Técnica",
                    Version = "v1",
                    Description = "Esta API forma parte de una prueba técnica que implementa funcionalidades típicas de un sistema bancario, como la gestión de cuentas, saldos y otros recursos relacionados. No es un producto oficial de Banco Lafise. El código y las implementaciones son de autoría individual exclusivamente con fines evaluativos y demostrativos.",
                    TermsOfService = new Uri("https://www.djacamo01.dev/privacy"),
                    Contact = new OpenApiContact
                    {
                        Name = "Donald Miguel Jacamo - Desarrollador (Prueba Técnica)",
                        Email = "jacamodonalddmiguel@gmail.com",
                        Url = new Uri("https://www.djacamo01.dev/")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Código abierto bajo licencia MIT",
                        Url = new Uri("https://opensource.org/licenses/MIT")
                    }
                });

                c.AddServer(new OpenApiServer
                {
                    Url = "https://localhost:7233",
                    Description = "Servidor de desarrollo local (HTTPS)"
                });
                c.AddServer(new OpenApiServer
                {
                    Url = "http://localhost:5135",
                    Description = "Servidor de desarrollo local (HTTP)"
                });

                
                c.TagActionsBy(apiDesc =>
                {
                    var controllerName = apiDesc.GroupName ?? apiDesc.ActionDescriptor.RouteValues["controller"];
                    return new[] { controllerName ?? "Default" };
                });

               
                c.OrderActionsBy(apiDesc =>
                {
                    var method = apiDesc.HttpMethod ?? "";
                    var path = apiDesc.RelativePath ?? "";
                    return $"{method}_{path}";
                });

                
                c.CustomOperationIds(apiDesc =>
                {
                    var controllerName = apiDesc.ActionDescriptor.RouteValues["controller"];
                    var actionName = apiDesc.ActionDescriptor.RouteValues["action"];
                    return $"{controllerName}_{actionName}";
                });

                
                c.DescribeAllParametersInCamelCase();

            
                c.IgnoreObsoleteActions();
                c.IgnoreObsoleteProperties();

                
                c.SupportNonNullableReferenceTypes();
                c.NonNullableReferenceTypesAsRequired();

                
                c.UseAllOfForInheritance();

               
                c.UseOneOfForPolymorphism();

                c.UseAllOfToExtendReferenceSchemas();

                
                c.UseInlineDefinitionsForEnums();

                c.CustomSchemaIds(type => type.FullName?.Replace("+", ".") ?? type.Name);
            });

            var app = builder.Build();
                        
           

            
            string? env = builder.Configuration.GetValue<string>("env");
            if (app.Environment.IsDevelopment() || env == "DEV")
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Lafise Banking System API v1");
                    c.DocumentTitle = "Lafise Banking System API - Documentación";
                    c.DefaultModelsExpandDepth(-1); 
                    c.DefaultModelExpandDepth(2); 
                    c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List); 
                    c.EnableDeepLinking(); 
                    c.EnableFilter(); 
                    c.EnableValidator(); 
                    c.ShowExtensions(); 
                    c.ShowCommonExtensions(); 
                    c.RoutePrefix = "swagger"; 
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
            
          
            builder.Services.AddScoped<IAccountCreationValidator, AccountCreationValidator>();
            builder.Services.AddScoped<Lafise.API.services.Accounts.Repositories.IAccountRepository, Lafise.API.services.Accounts.Repositories.AccountRepository>();
            builder.Services.AddScoped<IAccountFactory, AccountFactory>();
            builder.Services.AddScoped<IAccountNumberGenerator, AccountNumberGenerator>();
            builder.Services.AddScoped<IAccountBalanceMapper, AccountBalanceMapper>();
            
           
            builder.Services.AddScoped<ITransactionValidator, TransactionValidator>();
            builder.Services.AddScoped<IAccountValidator, AccountValidator>();
            builder.Services.AddScoped<Lafise.API.services.Transactions.Repositories.IAccountRepository, Lafise.API.services.Transactions.Repositories.AccountRepository>();
            builder.Services.AddScoped<ITransactionFactory, TransactionFactory>();
            builder.Services.AddScoped<ITransactionService, TransactionService>();
            
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