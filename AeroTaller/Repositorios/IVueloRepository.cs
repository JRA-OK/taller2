public interface IVueloRepository
{
    // Filtros opcionales: null = sin filtrar. Ordenados por salida.
    List<Vuelo> ObtenerTodos(int? destinoId = null, DateTime? desde = null);
    Vuelo? ObtenerPorId(int id);
    bool ExisteNumero(string numero);
    int Crear(Vuelo vuelo);   // devuelve el Id generado
}
