// La sesión guarda solo lo necesario para mostrar y autorizar (nunca la clave).
// Scoped: cada request lee la sesión de su propio HttpContext (vía IHttpContextAccessor).
public class AutenticacionService(
    IHttpContextAccessor accesor, IUsuarioRepository usuarios, ILogger<AutenticacionService> logger) : IAutenticacionService
{
    private const string ClaveId = "Usuario.Id";
    private const string ClaveNombre = "Usuario.Nombre";
    private const string ClaveRol = "Usuario.Rol";

    private ISession Sesion => accesor.HttpContext?.Session
        ?? throw new InvalidOperationException("No hay un request en curso: la sesión solo existe dentro de un request HTTP.");

    public Usuario? IniciarSesion(string nombreUsuario, string clave)
    {
        Usuario? usuario = usuarios.ObtenerPorNombreUsuario(nombreUsuario.Trim());
        // Clave en texto plano solo con fines didácticos (ver aerotaller.sql): en producción, hash con sal (PBKDF2, Argon2…)
        if (usuario is null || usuario.Clave != clave)
        {
            logger.LogWarning("Inicio de sesión fallido para {NombreUsuario}", nombreUsuario);   // jamás registrar la clave
            return null;
        }

        Sesion.Clear();   // no arrastrar datos de una sesión anterior
        Sesion.SetInt32(ClaveId, usuario.Id);
        Sesion.SetString(ClaveNombre, usuario.Nombre);
        Sesion.SetString(ClaveRol, usuario.Rol);
        logger.LogInformation("Sesión iniciada: {NombreUsuario} ({Rol})", usuario.NombreUsuario, usuario.Rol);
        return usuario;
    }

    public void CerrarSesion()
    {
        logger.LogInformation("Sesión cerrada: {Nombre}", Nombre);
        Sesion.Clear();
    }

    public bool EstaAutenticado => Sesion.GetInt32(ClaveId) is not null;
    public string? Nombre => Sesion.GetString(ClaveNombre);
    public string? Rol => Sesion.GetString(ClaveRol);

    public bool TieneRol(params string[] roles) => Rol is not null && roles.Contains(Rol);
}
