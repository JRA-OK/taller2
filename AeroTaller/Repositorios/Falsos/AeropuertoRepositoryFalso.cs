public class AeropuertoRepositoryFalso : IAeropuertoRepository
{
    public List<Aeropuerto> ObtenerTodos()
    {
        lock (DatosFalsos.Candado) return DatosFalsos.Aeropuertos.OrderBy(a => a.Ciudad).ToList();
    }

    public Aeropuerto? ObtenerPorId(int id)
    {
        lock (DatosFalsos.Candado) return DatosFalsos.Aeropuertos.FirstOrDefault(a => a.Id == id);
    }
}
