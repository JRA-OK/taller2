public interface IAeropuertoRepository
{
    List<Aeropuerto> ObtenerTodos();
    Aeropuerto? ObtenerPorId(int id);
}
