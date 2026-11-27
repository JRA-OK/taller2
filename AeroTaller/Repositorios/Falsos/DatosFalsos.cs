// Datos de ejemplo en memoria para OrigenDeDatos = "Falso" (sin base de datos).
// Las fechas se calculan a partir de hoy, como en aerotaller.sql, para que siempre haya vuelos futuros.
// Los repositorios falsos se registran como Singleton: los cambios duran mientras la aplicación esté en ejecución.
public static class DatosFalsos
{
    // Un único candado para todos los repositorios falsos: las listas se comparten entre requests concurrentes
    public static readonly Lock Candado = new();

    public static readonly List<Aeropuerto> Aeropuertos =
    [
        new() { Id = 1, Codigo = "TUC", Ciudad = "San Miguel de Tucumán", Nombre = "Teniente Benjamín Matienzo" },
        new() { Id = 2, Codigo = "AEP", Ciudad = "Buenos Aires", Nombre = "Aeroparque Jorge Newbery" },
        new() { Id = 3, Codigo = "COR", Ciudad = "Córdoba", Nombre = "Ingeniero Ambrosio Taravella" },
        new() { Id = 4, Codigo = "MDZ", Ciudad = "Mendoza", Nombre = "El Plumerillo" },
        new() { Id = 5, Codigo = "BRC", Ciudad = "San Carlos de Bariloche", Nombre = "Teniente Luis Candelaria" }
    ];

    public static readonly List<Vuelo> Vuelos =
    [
        Vuelo(1, "AT0999", 1, 2, -1, 7.5, 125, 20, 98000),    // ya salió
        Vuelo(2, "AT1001", 1, 2, 12, 7.5, 125, 20, 98000),
        Vuelo(3, "AT1002", 2, 1, 12, 18, 125, 20, 98000),
        Vuelo(4, "AT2010", 2, 5, 15, 10.25, 140, 25, 154000),
        Vuelo(5, "AT3005", 3, 4, 14, 21.5, 75, 15, 72000),
        Vuelo(6, "AT0500", 1, 3, 11, 12, 40, 1, 45000)         // avioneta de 6 asientos
    ];

    public static readonly List<Pasajero> Pasajeros =
    [
        new() { Id = 1, Nombre = "Ana Gómez", Dni = "30111222", Email = "ana.gomez@correo.com" },
        new() { Id = 2, Nombre = "Bruno Díaz", Dni = "28999000" },
        new() { Id = 3, Nombre = "Carla Ruiz", Dni = "35444555", Email = "carla.ruiz@correo.com" },
        new() { Id = 4, Nombre = "Diego Herrera", Dni = "33777888" }
    ];

    public static readonly List<Reserva> Reservas =
    [
        Reserva(1, vueloId: 2, pasajeroId: 1, "3A", diasAtras: 6, EstadoReserva.Confirmada),
        Reserva(2, vueloId: 2, pasajeroId: 2, "3B", diasAtras: 6, EstadoReserva.Confirmada),
        Reserva(3, vueloId: 2, pasajeroId: 4, "1C", diasAtras: 3, EstadoReserva.Cancelada),
        Reserva(4, vueloId: 3, pasajeroId: 1, "3A", diasAtras: 6, EstadoReserva.Confirmada),
        Reserva(5, vueloId: 6, pasajeroId: 3, "1A", diasAtras: 8, EstadoReserva.Confirmada)
    ];

    public static readonly List<Usuario> Usuarios =
    [
        new() { Id = 1, NombreUsuario = "admin", Nombre = "Administración", Clave = "aero2026", Rol = "Admin" },
        new() { Id = 2, NombreUsuario = "mostrador", Nombre = "Marta", Clave = "vuelos2026", Rol = "Mostrador" }
    ];

    // Próximo Id de una lista (lo que en SQLite hace AUTOINCREMENT)
    public static int SiguienteId<T>(List<T> lista, Func<T, int> id) => lista.Count == 0 ? 1 : lista.Max(id) + 1;

    private static Vuelo Vuelo(int id, string numero, int origenId, int destinoId, int dias, double hora, int duracion, int filas, decimal precio) => new()
    {
        Id = id,
        Numero = numero,
        Origen = Aeropuertos.First(a => a.Id == origenId),
        Destino = Aeropuertos.First(a => a.Id == destinoId),
        Salida = DateTime.Today.AddDays(dias).AddHours(hora),
        DuracionMinutos = duracion,
        Filas = filas,
        Precio = precio
    };

    private static Reserva Reserva(int id, int vueloId, int pasajeroId, string asiento, int diasAtras, EstadoReserva estado) => new()
    {
        Id = id,
        Vuelo = Vuelos.First(v => v.Id == vueloId),
        Pasajero = Pasajeros.First(p => p.Id == pasajeroId),
        Asiento = asiento,
        Fecha = DateTime.Now.AddDays(-diasAtras),
        Estado = estado
    };
}
