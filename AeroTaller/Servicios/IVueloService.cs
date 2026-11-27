public interface IVueloService
{
    // Devuelve el Id generado. Lanza ReglaDeNegocioException (número repetido, aeropuerto inexistente).
    int Crear(Vuelo vuelo);
}
