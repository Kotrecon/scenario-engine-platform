using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ScenarioDesigner.Security;

// ============================================================================
// АВТОРИЗАЦИЯ (Policy-based)
// Определяет, ЧТО может делать пользователь (какие операции).
// Policies инкапсулируют бизнес-правила доступа.
// ============================================================================
public static class AuthorizationExtensions
{
    public static IHostApplicationBuilder AddCustomAuthorization(this IHostApplicationBuilder builder)
    {
        builder.Services.AddAuthorization(options =>
        {
            // ----------------------------------------------------------------
            // AdminOnly — только администраторы.
            // Используется для опасных операций (изменение log levels).
            // ----------------------------------------------------------------
            options.AddPolicy("AdminOnly", policy =>
                policy.RequireRole("Admin"));

            // ----------------------------------------------------------------
            // Operator — операторы и администраторы.
            // Используется для чтения конфигурации.
            // ----------------------------------------------------------------
            options.AddPolicy("Operator", policy =>
                policy.RequireRole("Admin", "Operator"));

            // ----------------------------------------------------------------
            // AuditViewer — все роли, включая аудиторов.
            // Используется для просмотра логов и метрик (только чтение).
            // ----------------------------------------------------------------
            options.AddPolicy("AuditViewer", policy =>
                policy.RequireRole("Admin", "Operator", "Auditor"));
        });

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();

        return builder;
    }
}
