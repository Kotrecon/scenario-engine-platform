using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ScenarioDesigner.Security;

// ============================================================================
// АУТЕНТИФИКАЦИЯ (JWT Bearer)
// Проверяет, КТО обращается к API (пользователь/сервис).
// Токен должен быть в заголовке: Authorization: Bearer <token>
// ============================================================================
public static class AuthenticationExtensions
{
    public static IHostApplicationBuilder AddCustomAuthentication(this IHostApplicationBuilder builder)
    {
        var jwtKey = builder.Configuration["Jwt:Key"]!;
        var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "ScenarioDesigner";
        var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "ScenarioDesigner";

        // --------------------------------------------------------------------
        // TokenValidationParameters — правила проверки JWT.
        // Все Validate* = true — строгая проверка, без компромиссов.
        // --------------------------------------------------------------------
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // --------------------------------------------------------
                    // Issuer — кто выдал токен. Защита от подделки.
                    // --------------------------------------------------------
                    ValidateIssuer = true,
                    ValidIssuer = jwtIssuer,

                    // --------------------------------------------------------
                    // Audience — для кого токен. Защита от misuse.
                    // --------------------------------------------------------
                    ValidateAudience = true,
                    ValidAudience = jwtAudience,

                    // --------------------------------------------------------
                    // Lifetime — срок действия токена. Защита от replay attacks.
                    // --------------------------------------------------------
                    ValidateLifetime = true,

                    // --------------------------------------------------------
                    // Signing Key — подпись токена. Защита от подделки.
                    // --------------------------------------------------------
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),

                    // --------------------------------------------------------
                    // ClockSkew — допустимое рассинхронизация времени (1 минута).
                    // Защита от проблем с NTP между серверами.
                    // --------------------------------------------------------
                    ClockSkew = TimeSpan.FromMinutes(1)
                };
            });

        return builder;
    }
}
