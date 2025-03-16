
using DataBase;

namespace GingerMintSoft.Earth.Location.Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddSingleton<IFileStore, FileStore>();
            builder.Services.AddCors(options =>  
            {  
      
                options.AddDefaultPolicy(  
                    policy =>  
                    {  
                        policy.WithOrigins("https://localhost:7131", "http://localhost:5083")  
                            .AllowAnyHeader() 
                            .AllowAnyOrigin()
                            .AllowAnyMethod();  
                    });  
            });  

            var app = builder.Build();
            app.UseCors();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
