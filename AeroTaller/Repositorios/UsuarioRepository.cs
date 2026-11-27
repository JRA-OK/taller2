using Microsoft.Data.Sqlite;

public class UsuarioRepository(ConexionDb conexionDb) : IUsuarioRepository
{
    public Usuario? ObtenerPorNombreUsuario(string nombreUsuario)
    {
        using var conexion = new SqliteConnection(conexionDb.Cadena);
        conexion.Open();
        using var comando = conexion.CreateCommand();
        comando.CommandText = "SELECT Id, NombreUsuario, Nombre, Clave, Rol FROM Usuario WHERE NombreUsuario = @nombreUsuario";
        comando.Parameters.AddWithValue("@nombreUsuario", nombreUsuario);

        using var lector = comando.ExecuteReader();
        return lector.Read() ? ConversorSQL.AUsuario(lector) : null;
    }
}
