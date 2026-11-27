public interface IPasajeroService
{
    // Devuelve el Id generado. Lanza ReglaDeNegocioException si el DNI ya está registrado.
    int Crear(Pasajero pasajero);

    // Devuelve el pasajero eliminado, o null si no existe. Lanza ReglaDeNegocioException si tiene reservas.
    Pasajero? Eliminar(int id);
}
