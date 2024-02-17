
using UdemyShared.Configuration;
using UdemyShared.Extensions;

namespace UdemyMiniApp2.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // jwt auth
            builder.Services.Configure<CustomTokenOptions>(builder.Configuration.GetSection("TokenOptions"));
            var tokenOptions = builder.Configuration.GetSection("TokenOptions").Get<CustomTokenOptions>();
            builder.Services.AddCustomTokenAuth(tokenOptions);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}