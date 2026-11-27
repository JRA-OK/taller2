public class UsuarioRepositoryFalso : IUsuarioRepository
{
    public Usuario? ObtenerPorNombreUsuario(string nombreUsuario)
    {
        lock (DatosFalsos.Candado) return DatosFalsos.Usuarios.FirstOrDefault(u => u.NombreUsuario == nombreUsuario);
    }
}
