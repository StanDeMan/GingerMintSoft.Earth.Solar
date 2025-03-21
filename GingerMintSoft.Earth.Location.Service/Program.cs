
using DataBase;

namespace GingerMintSoft.Earth.Location.Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            const string solarCorsPolicy = "SolarCorsPolicy";
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddSingleton<IFileStore, FileStore>();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: solarCorsPolicy,
                    policy =>
                    {
                        policy.AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials()
                            .SetIsOriginAllowed(_ => true); // allow any origin
                    });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseCors(solarCorsPolicy);           // use core policy for localhost development
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
