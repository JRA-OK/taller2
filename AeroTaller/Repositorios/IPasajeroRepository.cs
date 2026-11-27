public interface IPasajeroRepository
{
    // buscar: coincidencia parcial por nombre o DNI; null o vacío = todos
    List<Pasajero> ObtenerTodos(string? buscar = null);
    Pasajero? ObtenerPorId(int id);
    bool ExisteDni(string dni);
    int Crear(Pasajero pasajero);   // devuelve el Id generado
    bool Eliminar(int id);          // false si no existe
}
