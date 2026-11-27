using Microsoft.Data.Sqlite;

public class ReservaRepository(ConexionDb conexionDb, ILogger<ReservaRepository> logger) : IReservaRepository
{
    // SQLITE_CONSTRAINT_UNIQUE (código extendido de SQLITE_CONSTRAINT = 19). En el INSERT de Reserva
    // la única restricción UNIQUE es el índice parcial UX_Reserva_AsientoConfirmado.
    private const int ViolacionUnica = 2067;

    // Reserva + vuelo (con sus aeropuertos) + pasajero, con los prefijos que espera ConversorSQL.AReserva
    private const string Select = """
        SELECT r.Id, r.Asiento, r.Fecha, r.Estado,
               v.Id AS VueloId, v.Numero AS VueloNumero, v.Salida AS VueloSalida,
               v.DuracionMinutos AS VueloDuracionMinutos, v.Filas AS VueloFilas, v.Precio AS VueloPrecio,
               o.Id AS VueloOrigenId,  o.Codigo AS VueloOrigenCodigo,  o.Ciudad AS VueloOrigenCiudad,  o.Nombre AS VueloOrigenNombre,
               d.Id AS VueloDestinoId, d.Codigo AS VueloDestinoCodigo, d.Ciudad AS VueloDestinoCiudad, d.Nombre AS VueloDestinoNombre,
               p.Id AS PasajeroId, p.Nombre AS PasajeroNombre, p.Dni AS PasajeroDni, p.Email AS PasajeroEmail
        FROM Reserva r
        JOIN Vuelo v      ON v.Id = r.VueloId
        JOIN Aeropuerto o ON o.Id = v.OrigenId
        JOIN Aeropuerto d ON d.Id = v.DestinoId
        JOIN Pasajero p   ON p.Id = r.PasajeroId
        """;

    public List<Reserva> ObtenerTodas()
    {
        using var conexion = new SqliteConnection(conexionDb.Cadena);
        conexion.Open();
        using var comando = conexion.CreateCommand();
        comando.CommandText = Select + " ORDER BY r.Fecha DESC";
        return Leer(comando);
    }

    public List<Reserva> ObtenerPorPasajero(int pasajeroId)
    {
        using var conexion = new SqliteConnection(conexionDb.Cadena);
        conexion.Open();
        using var comando = conexion.CreateCommand();
        comando.CommandText = Select + " WHERE r.PasajeroId = @pasajeroId ORDER BY v.Salida";
        comando.Parameters.AddWithValue("@pasajeroId", pasajeroId);
        return Leer(comando);
    }

    public Reserva? ObtenerPorId(int id)
    {
        using var conexion = new SqliteConnection(conexionDb.Cadena);
        conexion.Open();
        using var comando = conexion.CreateCommand();
        comando.CommandText = Select + " WHERE r.Id = @id";
        comando.Parameters.AddWithValue("@id", id);
        return Leer(comando).FirstOrDefault();
    }

    // Asientos con reserva confirmada: sirve para el mapa, para "asiento libre" y para contar la ocupación
    public List<string> ObtenerAsientosOcupados(int vueloId)
    {
        using var conexion = new SqliteConnection(conexionDb.Cadena);
        conexion.Open();
        using var comando = conexion.CreateCommand();
        comando.CommandText = "SELECT Asiento FROM Reserva WHERE VueloId = @vueloId AND Estado = 'Confirmada'";
        comando.Parameters.AddWithValue("@vueloId", vueloId);

        var asientos = new List<string>();
        using var lector = comando.ExecuteReader();
        while (lector.Read()) asientos.Add(lector.GetString(0));
        return asientos;
    }

    // Ocupación de todos los vuelos en una sola consulta (evita una consulta por vuelo en el tablero).
    // Los vuelos sin reservas confirmadas no aparecen en el diccionario.
    public Dictionary<int, int> ContarConfirmadasPorVuelo()
    {
        using var conexion = new SqliteConnection(conexionDb.Cadena);
        conexion.Open();
        using var comando = conexion.CreateCommand();
        comando.CommandText = "SELECT VueloId, COUNT(*) FROM Reserva WHERE Estado = 'Confirmada' GROUP BY VueloId";

        var conteo = new Dictionary<int, int>();
        using var lector = comando.ExecuteReader();
        while (lector.Read()) conteo[lector.GetInt32(0)] = lector.GetInt32(1);
        return conteo;
    }

    public bool ExisteConfirmada(int vueloId, int pasajeroId)
    {
        using var conexion = new SqliteConnection(conexionDb.Cadena);
        conexion.Open();
        using var comando = conexion.CreateCommand();
        comando.CommandText = """
            SELECT COUNT(*) FROM Reserva
            WHERE VueloId = @vueloId AND PasajeroId = @pasajeroId AND Estado = 'Confirmada'
            """;
        comando.Parameters.AddWithValue("@vueloId", vueloId);
        comando.Parameters.AddWithValue("@pasajeroId", pasajeroId);
        return Convert.ToInt32(comando.ExecuteScalar()) > 0;
    }

    // Devuelve el Id generado
    public int Crear(Reserva reserva)
    {
        using var conexion = new SqliteConnection(conexionDb.Cadena);
        conexion.Open();
        using var comando = conexion.CreateCommand();
        comando.CommandText = """
            INSERT INTO Reserva (VueloId, PasajeroId, Asiento, Fecha, Estado)
            VALUES (@vueloId, @pasajeroId, @asiento, @fecha, @estado);
            SELECT last_insert_rowid();
            """;
        comando.Parameters.AddWithValue("@vueloId", reserva.Vuelo.Id);
        comando.Parameters.AddWithValue("@pasajeroId", reserva.Pasajero.Id);
        comando.Parameters.AddWithValue("@asiento", reserva.Asiento);
        comando.Parameters.AddWithValue("@fecha", ConversorSQL.ATexto(reserva.Fecha));
        comando.Parameters.AddWithValue("@estado", reserva.Estado.ToString());
        try
        {
            return Convert.ToInt32(comando.ExecuteScalar());
        }
        catch (SqliteException ex) when (ex.SqliteErrorCode == 19 && ex.SqliteExtendedErrorCode == ViolacionUnica)
        {
            // Dos requests pasaron a la vez el control "asiento libre" del servicio: la base rechazó al segundo
            logger.LogWarning(ex, "Conflicto de concurrencia: el asiento {Asiento} del vuelo {VueloId} se reservó en simultáneo",
                reserva.Asiento, reserva.Vuelo.Id);
            throw new ReglaDeNegocioException("Asiento", $"El asiento {reserva.Asiento} acaba de ser reservado por otra persona. Elegí otro.");
        }
    }

    // No borra: la reserva queda en el historial como 'Cancelada'. Devuelve false si el Id no existe.
    public bool Cancelar(int id)
    {
        using var conexion = new SqliteConnection(conexionDb.Cadena);
        conexion.Open();
        using var comando = conexion.CreateCommand();
        comando.CommandText = "UPDATE Reserva SET Estado = 'Cancelada' WHERE Id = @id";
        comando.Parameters.AddWithValue("@id", id);
        return comando.ExecuteNonQuery() > 0;
    }

    private static List<Reserva> Leer(SqliteCommand comando)
    {
        var reservas = new List<Reserva>();
        using var lector = comando.ExecuteReader();
        while (lector.Read()) reservas.Add(ConversorSQL.AReserva(lector));
        return reservas;
    }
}
