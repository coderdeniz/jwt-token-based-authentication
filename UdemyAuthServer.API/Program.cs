using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UdemyCore.Configuration;
using UdemyCore.Models;
using UdemyCore.Repositories;
using UdemyCore.Services;
using UdemyData;
using UdemyData.Repositories;
using UdemyService.Services;
using UdemyShared.Configuration;

namespace UdemyAuthServer.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            // token geldikten sonra doðrulama kýsmý

            builder.Services.AddAuthentication(options =>
            {
                // þema: iki ayrý üyelik sistemi olabilir, bayiler ve kullanýcýlar için gibi
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;                             
            }
             // cookie bazlý mý yoksa jwt bazlý mý doðrulama yapcaz onu belirliyoruz. headerden görecek            
            ).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                // app settings'deki bilgiler
                var tokenOptions = builder.Configuration.GetSection("TokenOptions").Get<CustomTokenOptions>();

                // geçerlilik kontrolleri
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidIssuer = tokenOptions.Issuer,
                    ValidAudience = tokenOptions.Audience[0],
                    
                    IssuerSigningKey = SignService.GetSymmetricSecurityKey(tokenOptions.SecurityKey),

                    ValidateIssuerSigningKey = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuer = true,
                    // normalde 1 saat verdiðin token ekstra 5 dk verir 65 dk olur
                    // iki server'daki saat farkýný kapatmak için 
                    // zero verince 5 dk vermez hiç vermez
                    ClockSkew = TimeSpan.Zero,
                };
            });

            




            // DI Register
            builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped(typeof(IServiceGeneric<,>), typeof(GenericService<,>));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"),sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly("UdemyData");
                });
            }
            );

            builder.Services.AddIdentity<UserApp,IdentityRole>(opt =>
            {
                opt.User.RequireUniqueEmail= true;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireDigit = false;
                opt.Password.RequireLowercase = false;
                opt.Password.RequireUppercase = false;

            }).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

            builder.Services.Configure<CustomTokenOptions>(builder.Configuration.GetSection("TokenOptions"));
            builder.Services.Configure<List<Client>>(builder.Configuration.GetSection("Clients"));


            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // authorize attribute çalýþmasý için gerekli middleware
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}