using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Meat.Application.Shared;
using Meat.Domain.Shared;
using Meat.Application.Shared.Settings;
using Meat.Infrastructure;
using Meat.Repositories;
using Meat.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseDefaultServiceProvider(options => options.ValidateScopes = false);

builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

// Empresa activa de cada request: la lee del claim del JWT y la consumen los query
// filters del MeatContext, que aislan los datos por empresa sin intervencion del handler.
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ITenantContext, HttpTenantContext>();

builder.Services.AddDbContext<MeatContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("Default"),
        // El historial de migraciones tambien va en el schema meat: public lo expone Supabase.
        npgsql => npgsql
            .MigrationsAssembly("Meat.Repositories")
            .MigrationsHistoryTable("__EFMigrationsHistory", "meat"));

    // Vuelca a los logs los valores de las consultas: solo para desarrollo.
    if (builder.Environment.IsDevelopment())
        options.EnableSensitiveDataLogging();
});

// En produccion la API corre detras de nginx, que atiende HTTPS: sin esto no sabe que la request
// original llego por HTTPS. Por defecto solo se aceptan los encabezados que manda localhost.
builder.Services.Configure<ForwardedHeadersOptions>(options =>
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto);

// CORS: en desarrollo cualquier origen (Vite en localhost); en produccion solo los de Cors:Origins
// (variable de entorno Cors__Origins__0 con el dominio de Vercel). Sin origenes configurados,
// ningun navegador puede llamar a la API, pero la API arranca igual.
var corsOrigins = builder.Configuration.GetSection("Cors:Origins").Get<string[]>() ?? Array.Empty<string>();
builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
{
    if (builder.Environment.IsDevelopment())
        policy.AllowAnyOrigin();
    else
        policy.WithOrigins(corsOrigins);

    policy.AllowAnyMethod().AllowAnyHeader();
}));

builder.Services.AddSwagger();

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(System.Reflection.Assembly.Load("Meat.Application")));
builder.Services.AddAutoMapper(cfg => cfg.AddMaps(System.Reflection.Assembly.Load("Meat.Application")));

builder.Services.Configure<Directories>(builder.Configuration.GetSection("Directories"));
builder.Services.Configure<Endpoints>(builder.Configuration.GetSection("Endpoints"));

builder.Services.AddScoped<ImagesMethods>();
builder.Services.AddScoped<PathsMethods>();
builder.Services.AddScoped<EndpointsMethods>();

var isApiLocal = builder.Configuration.GetValue<bool>("IsApiLocal");
builder.Services.AddScoped<IsApiLocal>(_ => new IsApiLocal(isApiLocal));

builder.Services.AddIdentityServices(builder.Configuration, builder.Environment.IsDevelopment());

// Todo endpoint exige usuario autenticado salvo que declare [AllowAnonymous] (hoy solo el login).
// Asi un controller que olvide [Authorize] no queda abierto a internet.
builder.Services.AddAuthorization(options =>
    options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());
builder.Services.AddApplicationInsightsTelemetry();

if (!isApiLocal)
    builder.Services.AddHostedService<MeatService>();
else
    builder.Services.AddApplicationInsightsTelemetryProcessor<AppInsightsTelemetryProcessor>();

var app = builder.Build();

app.UseForwardedHeaders();
app.UseMiddleware<ExceptionHandlerMiddleware>();
app.UseHttpsRedirection();
app.UseRouting();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// La documentacion de la API no se publica en produccion.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MeatNet API V1"));
}

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<MeatContext>();
    context.Database.Migrate();
}

app.Run();
