using Microsoft.Data.Sqlite;

public class AeropuertoRepository(ConexionDb conexionDb) : IAeropuertoRepository
{
    public List<Aeropuerto> ObtenerTodos()
    {
        using var conexion = new SqliteConnection(conexionDb.Cadena);
        conexion.Open();
        using var comando = conexion.CreateCommand();
        comando.CommandText = "SELECT Id, Codigo, Ciudad, Nombre FROM Aeropuerto ORDER BY Ciudad";

        var aeropuertos = new List<Aeropuerto>();
        using var lector = comando.ExecuteReader();
        while (lector.Read()) aeropuertos.Add(ConversorSQL.AAeropuerto(lector));
        return aeropuertos;
    }

    public Aeropuerto? ObtenerPorId(int id)
    {
        using var conexion = new SqliteConnection(conexionDb.Cadena);
        conexion.Open();
        using var comando = conexion.CreateCommand();
        comando.CommandText = "SELECT Id, Codigo, Ciudad, Nombre FROM Aeropuerto WHERE Id = @id";
        comando.Parameters.AddWithValue("@id", id);

        using var lector = comando.ExecuteReader();
        return lector.Read() ? ConversorSQL.AAeropuerto(lector) : null;
    }
}
