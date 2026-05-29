using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Meat.Application.Shared;
using Meat.Application.Shared.Settings;
using Meat.Infrastructure;
using Meat.Queries;
using Meat.Queries.Articulos;
using Meat.Queries.Informes;
using Meat.Repositories;
using Meat.Services;
using System.Data;
using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseDefaultServiceProvider(options => options.ValidateScopes = false);

builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

builder.Services.AddDbContext<MeatContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("Default"),
        sql => sql.MigrationsAssembly("Meat.Repositories"))
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

builder.Services.AddIdentityServices(builder.Configuration);
builder.Services.AddApplicationInsightsTelemetry();

if (!isApiLocal)
    builder.Services.AddHostedService<MeatService>();
else
    builder.Services.AddApplicationInsightsTelemetryProcessor<AppInsightsTelemetryProcessor>();

builder.Services.AddScoped<IUsuariosQueries, UsuariosQueries>();
builder.Services.AddScoped<IVentasEmpleadosQueries, VentasEmpleadosQueries>();
builder.Services.AddScoped<IArticulosMarcasPorSucursalQueries, ArticulosMarcasPorSucursalQueries>();

builder.Services.AddScoped<IDbConnection>(_ =>
{
    var connection = new SqlConnection { ConnectionString = builder.Configuration.GetConnectionString("Default") };
    connection.Open();
    return connection;
});

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
