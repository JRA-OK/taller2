using Microsoft.Data.Sqlite;

public class PasajeroRepository(ConexionDb conexionDb) : IPasajeroRepository
{
    // buscar vacío o null: todos; si no, coincidencia parcial por nombre o DNI
    public List<Pasajero> ObtenerTodos(string? buscar = null)
    {
        using var conexion = new SqliteConnection(conexionDb.Cadena);
        conexion.Open();
        using var comando = conexion.CreateCommand();
        comando.CommandText = """
            SELECT Id, Nombre, Dni, Email FROM Pasajero
            WHERE @patron IS NULL OR Nombre LIKE @patron OR Dni LIKE @patron
            ORDER BY Nombre
            """;
        string? patron = string.IsNullOrWhiteSpace(buscar) ? null : $"%{buscar.Trim()}%";
        comando.Parameters.AddWithValue("@patron", ConversorSQL.ONulo(patron));

        var pasajeros = new List<Pasajero>();
        using var lector = comando.ExecuteReader();
        while (lector.Read()) pasajeros.Add(ConversorSQL.APasajero(lector));
        return pasajeros;
    }

    public Pasajero? ObtenerPorId(int id)
    {
        using var conexion = new SqliteConnection(conexionDb.Cadena);
        conexion.Open();
        using var comando = conexion.CreateCommand();
        comando.CommandText = "SELECT Id, Nombre, Dni, Email FROM Pasajero WHERE Id = @id";
        comando.Parameters.AddWithValue("@id", id);

        using var lector = comando.ExecuteReader();
        return lector.Read() ? ConversorSQL.APasajero(lector) : null;
    }

    public bool ExisteDni(string dni)
    {
        using var conexion = new SqliteConnection(conexionDb.Cadena);
        conexion.Open();
        using var comando = conexion.CreateCommand();
        comando.CommandText = "SELECT COUNT(*) FROM Pasajero WHERE Dni = @dni";
        comando.Parameters.AddWithValue("@dni", dni);
        return Convert.ToInt32(comando.ExecuteScalar()) > 0;
    }

    // Devuelve el Id generado
    public int Crear(Pasajero pasajero)
    {
        using var conexion = new SqliteConnection(conexionDb.Cadena);
        conexion.Open();
        using var comando = conexion.CreateCommand();
        comando.CommandText = """
            INSERT INTO Pasajero (Nombre, Dni, Email) VALUES (@nombre, @dni, @email);
            SELECT last_insert_rowid();
            """;
        comando.Parameters.AddWithValue("@nombre", pasajero.Nombre);
        comando.Parameters.AddWithValue("@dni", pasajero.Dni);
        comando.Parameters.AddWithValue("@email", ConversorSQL.ONulo(pasajero.Email));
        return Convert.ToInt32(comando.ExecuteScalar());
    }

    // Con reservas asociadas, la FK (Foreign Keys=True) lo impide: el servicio lo controla antes
    public bool Eliminar(int id)
    {
        using var conexion = new SqliteConnection(conexionDb.Cadena);
        conexion.Open();
        using var comando = conexion.CreateCommand();
        comando.CommandText = "DELETE FROM Pasajero WHERE Id = @id";
        comando.Parameters.AddWithValue("@id", id);
        return comando.ExecuteNonQuery() > 0;
    }
}
