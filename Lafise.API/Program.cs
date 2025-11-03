using System.Reflection;
using System.Text.Json.Serialization;
using Lafise.API.data;
using Lafise.API.services.clients;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;



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

            // Add services to the container.
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            builder.Services.AddSwaggerGen(c =>
            {
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
                    Description = "API para gestión bancaria - Lafise"
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


            //Agregando servicios            
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            

            builder.Services.AddAuthorization();

            builder.Services.AddScoped<IClientService, ClientService>();
        }

        
}