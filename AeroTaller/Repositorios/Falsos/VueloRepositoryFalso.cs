public class VueloRepositoryFalso : IVueloRepository
{
    public List<Vuelo> ObtenerTodos(int? destinoId = null, DateTime? desde = null)
    {
        lock (DatosFalsos.Candado)
            return DatosFalsos.Vuelos
                .Where(v => destinoId is null || v.Destino.Id == destinoId)
                .Where(v => desde is null || v.Salida >= desde)
                .OrderBy(v => v.Salida)
                .ToList();
    }

    public Vuelo? ObtenerPorId(int id)
    {
        lock (DatosFalsos.Candado) return DatosFalsos.Vuelos.FirstOrDefault(v => v.Id == id);
    }

    public bool ExisteNumero(string numero)
    {
        lock (DatosFalsos.Candado) return DatosFalsos.Vuelos.Any(v => v.Numero == numero);
    }

    public int Crear(Vuelo vuelo)
    {
        lock (DatosFalsos.Candado)
        {
            // Como el JOIN del repositorio real: el vuelo guardado tiene sus aeropuertos completos, no solo el Id
            vuelo.Id = DatosFalsos.SiguienteId(DatosFalsos.Vuelos, v => v.Id);
            vuelo.Origen = DatosFalsos.Aeropuertos.First(a => a.Id == vuelo.Origen.Id);
            vuelo.Destino = DatosFalsos.Aeropuertos.First(a => a.Id == vuelo.Destino.Id);
            DatosFalsos.Vuelos.Add(vuelo);
            return vuelo.Id;
        }
    }
}
