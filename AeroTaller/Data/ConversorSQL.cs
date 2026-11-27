using System.Globalization;
using Microsoft.Data.Sqlite;

// Convierte filas de SqliteDataReader en modelos y fechas DateTime <-> texto 'AAAA-MM-DD HH:MM'.
// Convención de los SELECT con JOIN: las columnas de cada entidad compuesta llevan su nombre como
// prefijo (OrigenCodigo, VueloNumero, VueloOrigenCiudad…); el prefijo se pasa a cada método.
public static class ConversorSQL
{
    public const string FormatoFecha = "yyyy-MM-dd HH:mm";

    public static string ATexto(DateTime fecha) => fecha.ToString(FormatoFecha, CultureInfo.InvariantCulture);

    public static DateTime AFecha(string texto) => DateTime.ParseExact(texto, FormatoFecha, CultureInfo.InvariantCulture);

    public static Aeropuerto AAeropuerto(SqliteDataReader lector, string prefijo = "") => new()
    {
        Id = lector.GetInt32(lector.GetOrdinal(prefijo + "Id")),
        Codigo = lector.GetString(lector.GetOrdinal(prefijo + "Codigo")),
        Ciudad = lector.GetString(lector.GetOrdinal(prefijo + "Ciudad")),
        Nombre = lector.GetString(lector.GetOrdinal(prefijo + "Nombre"))
    };

    public static Vuelo AVuelo(SqliteDataReader lector, string prefijo = "") => new()
    {
        Id = lector.GetInt32(lector.GetOrdinal(prefijo + "Id")),
        Numero = lector.GetString(lector.GetOrdinal(prefijo + "Numero")),
        Origen = AAeropuerto(lector, prefijo + "Origen"),
        Destino = AAeropuerto(lector, prefijo + "Destino"),
        Salida = AFecha(lector.GetString(lector.GetOrdinal(prefijo + "Salida"))),
        DuracionMinutos = lector.GetInt32(lector.GetOrdinal(prefijo + "DuracionMinutos")),
        Filas = lector.GetInt32(lector.GetOrdinal(prefijo + "Filas")),
        Precio = lector.GetDecimal(lector.GetOrdinal(prefijo + "Precio"))   // REAL en SQLite
    };

    public static Pasajero APasajero(SqliteDataReader lector, string prefijo = "")
    {
        int email = lector.GetOrdinal(prefijo + "Email");
        return new()
        {
            Id = lector.GetInt32(lector.GetOrdinal(prefijo + "Id")),
            Nombre = lector.GetString(lector.GetOrdinal(prefijo + "Nombre")),
            Dni = lector.GetString(lector.GetOrdinal(prefijo + "Dni")),
            Email = lector.IsDBNull(email) ? null : lector.GetString(email)
        };
    }

    public static Reserva AReserva(SqliteDataReader lector) => new()
    {
        Id = lector.GetInt32(lector.GetOrdinal("Id")),
        Vuelo = AVuelo(lector, "Vuelo"),
        Pasajero = APasajero(lector, "Pasajero"),
        Asiento = lector.GetString(lector.GetOrdinal("Asiento")),
        Fecha = AFecha(lector.GetString(lector.GetOrdinal("Fecha"))),
        Estado = Enum.Parse<EstadoReserva>(lector.GetString(lector.GetOrdinal("Estado")))
    };

    public static Usuario AUsuario(SqliteDataReader lector) => new()
    {
        Id = lector.GetInt32(lector.GetOrdinal("Id")),
        NombreUsuario = lector.GetString(lector.GetOrdinal("NombreUsuario")),
        Nombre = lector.GetString(lector.GetOrdinal("Nombre")),
        Clave = lector.GetString(lector.GetOrdinal("Clave")),
        Rol = lector.GetString(lector.GetOrdinal("Rol"))
    };

    // AddWithValue no acepta null: los parámetros opcionales viajan como DBNull.Value
    public static object ONulo(object? valor) => valor ?? DBNull.Value;
}
