using Microsoft.Data.Sqlite;

// Crea la base SQLite a partir de aerotaller.sql la primera vez que arranca la aplicación.
public static class InicializadorBase
{
    public static void CrearBaseSiNoExiste(string cadena)
    {
        // La ruta del .db sale de la cadena (puede venir de ConnectionStrings__AeroTaller con una ruta temporal).
        // Una ruta relativa se resuelve contra el directorio de trabajo, igual que lo hace SqliteConnection.
        string rutaBase = Path.GetFullPath(new SqliteConnectionStringBuilder(cadena).DataSource);
        if (File.Exists(rutaBase)) return;

        Directory.CreateDirectory(Path.GetDirectoryName(rutaBase)!);

        // El script se copia a la salida (CopyToOutputDirectory), así no depende del directorio de trabajo
        string script = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "aerotaller.sql"));

        // Abrir la conexión crea el archivo; ExecuteNonQuery ejecuta todas las sentencias del script en orden
        using var conexion = new SqliteConnection(cadena);
        conexion.Open();
        using var comando = conexion.CreateCommand();
        comando.CommandText = script;
        comando.ExecuteNonQuery();
    }
}
