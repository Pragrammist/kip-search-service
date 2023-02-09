using System.Reflection;
using Prometheus;
using Serilog;
using Serilog.Events;
using Web.GrpcServices;
using Infrastructure.Configuration;

public class Program
{
    public static void Main(string[] args)
    {
        try
        {
            var logstashUrl = Environment.GetEnvironmentVariable("LOGSTASH_URL") ?? "http://localhost:8080";
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .WriteTo.Console()
                .WriteTo.Http(logstashUrl, queueLimitBytes: null)
                .CreateLogger();


            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseSerilog();

            var configuration = builder.Configuration;


            
            

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddElastic();
            builder.Services.AddSearchers();
            
            
            
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer()
                            .AddSwaggerGen(options => {
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(System.IO.Path.Combine(AppContext.BaseDirectory, xmlFilename), includeControllerXmlComments: true);
            });
            

            var app = builder.Build();
            app.UseSerilogRequestLogging();
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                
            }


            app.UseMetricServer();
            app.UseHttpMetrics(options => options.ReduceStatusCodeCardinality());
            
            

            app.UseSwagger();
            app.UseSwaggerUI(options => {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                options.RoutePrefix = string.Empty;
            });

            app.UseAuthorization();

            app.MapMetrics();
            
            app.MapControllers();
            
            app.Run();
        }
        catch(Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
