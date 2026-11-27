// Usuario del sistema (personal de la aerolínea); no confundir con Pasajero
public class Usuario
{
    public int Id { get; set; }
    public string NombreUsuario { get; set; } = "";
    public string Nombre { get; set; } = "";
    public string Clave { get; set; } = "";   // texto plano solo con fines didácticos (ver aerotaller.sql)
    public string Rol { get; set; } = "";     // Roles.Admin o Roles.Mostrador
}

// Valores de la columna Usuario.Rol (CHECK en aerotaller.sql): constantes para no repetir strings sueltos
public static class Roles
{
    public const string Admin = "Admin";
    public const string Mostrador = "Mostrador";
}
