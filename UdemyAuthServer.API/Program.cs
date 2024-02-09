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


            // token geldikten sonra do�rulama k�sm�

            builder.Services.AddAuthentication(options =>
            {
                // �ema: iki ayr� �yelik sistemi olabilir, bayiler ve kullan�c�lar i�in gibi
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;                             
            }
             // cookie bazl� m� yoksa jwt bazl� m� do�rulama yapcaz onu belirliyoruz. headerden g�recek            
            ).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                // app settings'deki bilgiler
                var tokenOptions = builder.Configuration.GetSection("TokenOptions").Get<CustomTokenOptions>();

                // ge�erlilik kontrolleri
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidIssuer = tokenOptions.Issuer,
                    ValidAudience = tokenOptions.Audience[0],
                    
                    IssuerSigningKey = SignService.GetSymmetricSecurityKey(tokenOptions.SecurityKey),

                    ValidateIssuerSigningKey = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuer = true,
                    // normalde 1 saat verdi�in token ekstra 5 dk verir 65 dk olur
                    // iki server'daki saat fark�n� kapatmak i�in 
                    // zero verince 5 dk vermez hi� vermez
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

            // authorize attribute �al��mas� i�in gerekli middleware
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}