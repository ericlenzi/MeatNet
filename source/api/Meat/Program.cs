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
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("Default"),
        // El historial de migraciones tambien va en el schema meat: public lo expone Supabase.
        npgsql => npgsql
            .MigrationsAssembly("Meat.Repositories")
            .MigrationsHistoryTable("__EFMigrationsHistory", "meat"))
    .EnableSensitiveDataLogging());

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
builder.Services.AddApplicationInsightsTelemetry();

if (!isApiLocal)
    builder.Services.AddHostedService<MeatService>();
else
    builder.Services.AddApplicationInsightsTelemetryProcessor<AppInsightsTelemetryProcessor>();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlerMiddleware>();
app.UseCors(option => option.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "POS API V1"));

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<MeatContext>();
    context.Database.Migrate();
}

app.Run();
