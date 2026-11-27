using Microsoft.Data.Sqlite;

public class VueloRepository(ConexionDb conexionDb) : IVueloRepository
{
    // Vuelo + sus dos aeropuertos; los alias siguen la convención de prefijos de ConversorSQL
    private const string Select = """
        SELECT v.Id, v.Numero, v.Salida, v.DuracionMinutos, v.Filas, v.Precio,
               o.Id AS OrigenId,  o.Codigo AS OrigenCodigo,  o.Ciudad AS OrigenCiudad,  o.Nombre AS OrigenNombre,
               d.Id AS DestinoId, d.Codigo AS DestinoCodigo, d.Ciudad AS DestinoCiudad, d.Nombre AS DestinoNombre
        FROM Vuelo v
        JOIN Aeropuerto o ON o.Id = v.OrigenId
        JOIN Aeropuerto d ON d.Id = v.DestinoId
        """;

    // Filtros opcionales: un parámetro NULL desactiva su condición (sin armar el SQL a mano).
    // Salida es texto 'AAAA-MM-DD HH:MM', así que la comparación de texto respeta el orden cronológico.
    public List<Vuelo> ObtenerTodos(int? destinoId = null, DateTime? desde = null)
    {
        using var conexion = new SqliteConnection(conexionDb.Cadena);
        conexion.Open();
        using var comando = conexion.CreateCommand();
        comando.CommandText = Select + """

            WHERE (@destinoId IS NULL OR v.DestinoId = @destinoId)
              AND (@desde IS NULL OR v.Salida >= @desde)
            ORDER BY v.Salida
            """;
        comando.Parameters.AddWithValue("@destinoId", ConversorSQL.ONulo(destinoId));
        comando.Parameters.AddWithValue("@desde", ConversorSQL.ONulo(desde is null ? null : ConversorSQL.ATexto(desde.Value)));

        var vuelos = new List<Vuelo>();
        using var lector = comando.ExecuteReader();
        while (lector.Read()) vuelos.Add(ConversorSQL.AVuelo(lector));
        return vuelos;
    }

    public Vuelo? ObtenerPorId(int id)
    {
        using var conexion = new SqliteConnection(conexionDb.Cadena);
        conexion.Open();
        using var comando = conexion.CreateCommand();
        comando.CommandText = Select + " WHERE v.Id = @id";
        comando.Parameters.AddWithValue("@id", id);

        using var lector = comando.ExecuteReader();
        return lector.Read() ? ConversorSQL.AVuelo(lector) : null;
    }

    public bool ExisteNumero(string numero)
    {
        using var conexion = new SqliteConnection(conexionDb.Cadena);
        conexion.Open();
        using var comando = conexion.CreateCommand();
        comando.CommandText = "SELECT COUNT(*) FROM Vuelo WHERE Numero = @numero";
        comando.Parameters.AddWithValue("@numero", numero);
        return Convert.ToInt32(comando.ExecuteScalar()) > 0;
    }

    // Devuelve el Id generado
    public int Crear(Vuelo vuelo)
    {
        using var conexion = new SqliteConnection(conexionDb.Cadena);
        conexion.Open();
        using var comando = conexion.CreateCommand();
        comando.CommandText = """
            INSERT INTO Vuelo (Numero, OrigenId, DestinoId, Salida, DuracionMinutos, Filas, Precio)
            VALUES (@numero, @origenId, @destinoId, @salida, @duracion, @filas, @precio);
            SELECT last_insert_rowid();
            """;
        comando.Parameters.AddWithValue("@numero", vuelo.Numero);
        comando.Parameters.AddWithValue("@origenId", vuelo.Origen.Id);
        comando.Parameters.AddWithValue("@destinoId", vuelo.Destino.Id);
        comando.Parameters.AddWithValue("@salida", ConversorSQL.ATexto(vuelo.Salida));
        comando.Parameters.AddWithValue("@duracion", vuelo.DuracionMinutos);
        comando.Parameters.AddWithValue("@filas", vuelo.Filas);
        comando.Parameters.AddWithValue("@precio", vuelo.Precio);
        return Convert.ToInt32(comando.ExecuteScalar());
    }
}
