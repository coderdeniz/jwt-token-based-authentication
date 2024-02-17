using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using UdemyService.Services;
using UdemyShared.Configuration;

namespace UdemyShared.Extensions
{
    public static class CustomTokenAuth 
    {
        public static void AddCustomTokenAuth(this IServiceCollection serviceCollection, CustomTokenOptions tokenOptions )
        {
            serviceCollection.AddAuthentication(options =>
            {
                // şema: iki ayrı üyelik sistemi olabilir, bayiler ve kullanıcılar için gibi
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }
            // cookie bazlı mı yoksa jwt bazlı mı doğrulama yapcaz onu belirliyoruz. headerden görecek            
            ).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {

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
                    // normalde 1 saat verdiğin token ekstra 5 dk verir 65 dk olur
                    // iki server'daki saat farkını kapatmak için 
                    // zero verince 5 dk vermez hiç vermez
                    ClockSkew = TimeSpan.Zero,
                };
            });
        }
    }
}
