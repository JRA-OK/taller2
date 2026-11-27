// Inicio de sesión del personal (tabla Usuario). El estado vive en HttpContext.Session.
public interface IAutenticacionService
{
    // Devuelve el usuario y abre la sesión si las credenciales son correctas; si no, null
    Usuario? IniciarSesion(string nombreUsuario, string clave);
    void CerrarSesion();

    bool EstaAutenticado { get; }
    string? Nombre { get; }
    string? Rol { get; }
    bool TieneRol(params string[] roles);
}
