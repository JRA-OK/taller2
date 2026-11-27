using System.Globalization;
using Serilog;
using Serilog.Events;

// AeroTaller — proyecto base del TP integrador.
// Se completa desafío por desafío siguiendo la Torre de Control (dotnet run --project torre-de-control).

// Fechas y moneda en castellano, en cualquier computadora.
// Decimales: un <input type="number"> SIEMPRE envía "18.5" (con punto), y con es-AR sin modificar
// el model binding lo leería como 185. Por eso los números (no la moneda) usan punto decimal.
var cultura = (CultureInfo)CultureInfo.GetCultureInfo("es-AR").Clone();
cultura.NumberFormat.NumberDecimalSeparator = ".";
cultura.NumberFormat.NumberGroupSeparator = "";
CultureInfo.DefaultThreadCurrentCulture = cultura;

var builder = WebApplication.CreateBuilder(args);

// Desafío 7: el origen de datos se elige por configuración. Controladores y servicios piden interfaces,
// así que no cambian: solo cambia qué implementación entrega el contenedor.
string origenDeDatos = builder.Configuration["OrigenDeDatos"] ?? "Sqlite";
switch (origenDeDatos)
{
    case "Sqlite":
        // Desafío 1: la cadena se lee de la configuración (appsettings.json o variable de entorno), nunca del código
        string cadenaConexion = builder.Configuration.GetConnectionString("AeroTaller")
            ?? throw new InvalidOperationException("Falta ConnectionStrings:AeroTaller en appsettings.json.");
        InicializadorBase.CrearBaseSiNoExiste(cadenaConexion);

        // La cadena es inmutable y compartida (Singleton); los repositorios, uno por request (Scoped)
        builder.Services.AddSingleton(new ConexionDb(cadenaConexion));
        builder.Services.AddScoped<IAeropuertoRepository, AeropuertoRepository>();
        builder.Services.AddScoped<IVueloRepository, VueloRepository>();
        builder.Services.AddScoped<IPasajeroRepository, PasajeroRepository>();
        builder.Services.AddScoped<IReservaRepository, ReservaRepository>();
        builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        break;

    case "Falso":
        // En memoria y sin tocar la base. Singleton: los datos deben sobrevivir entre requests
        builder.Services.AddSingleton<IAeropuertoRepository, AeropuertoRepositoryFalso>();
        builder.Services.AddSingleton<IVueloRepository, VueloRepositoryFalso>();
        builder.Services.AddSingleton<IPasajeroRepository, PasajeroRepositoryFalso>();
        builder.Services.AddSingleton<IReservaRepository, ReservaRepositoryFalso>();
        builder.Services.AddSingleton<IUsuarioRepository, UsuarioRepositoryFalso>();
        break;

    default:
        throw new InvalidOperationException($"OrigenDeDatos = '{origenDeDatos}' no es válido: usar 'Sqlite' o 'Falso'.");
}

// Servicios: las reglas del negocio. Los controladores dependen de la interfaz, no de la clase
builder.Services.AddScoped<IReservaService, ReservaService>();
builder.Services.AddScoped<IPasajeroService, PasajeroService>();
builder.Services.AddScoped<IVueloService, VueloService>();

// Desafío 8: Serilog reemplaza al logger de .NET; los ILogger<T> inyectados escriben aquí.
// Un archivo por día en AeroTaller/logs/ (aerotaller-20261127.txt); {RequestId} agrupa las líneas de un mismo request
// y es el código de referencia que la página de error le muestra al usuario.
string carpetaLogs = Path.Combine(builder.Environment.ContentRootPath, "logs");
builder.Services.AddSerilog(log => log
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(
        Path.Combine(carpetaLogs, "aerotaller-.txt"),
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 31,
        shared: true,   // el archivo se puede leer (verificador, editor) mientras la aplicación escribe
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {SourceContext} {RequestId}{NewLine}    {Message:lj}{NewLine}{Exception}"));

// Desafío 9: sesión en memoria del servidor; el navegador solo guarda una cookie con el identificador
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(opciones =>
{
    opciones.IdleTimeout = TimeSpan.FromMinutes(30);
    opciones.Cookie.HttpOnly = true;      // JavaScript no puede leerla (mitiga robo de sesión por XSS)
    opciones.Cookie.IsEssential = true;   // necesaria para funcionar: no depende del consentimiento de cookies
    opciones.Cookie.SameSite = SameSiteMode.Lax;
});
builder.Services.AddHttpContextAccessor();   // para leer la sesión desde AutenticacionService
builder.Services.AddScoped<IAutenticacionService, AutenticacionService>();

// MVC: controladores que devuelven vistas
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Siempre, también en Development: el usuario nunca ve la traza técnica. El middleware registra la
// excepción en el log y vuelve a ejecutar el request en /Home/Error, conservando el código 500.
app.UseExceptionHandler("/Home/Error");

app.UseStaticFiles();   // sirve lo que está en wwwroot (el CSS)
app.UseSession();       // antes de los controladores: carga la sesión del request para filtros y acciones

// Ruteo convencional: /Vuelos/Detalle/3 → VuelosController.Detalle(3)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
