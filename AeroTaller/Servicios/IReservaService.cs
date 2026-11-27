public interface IReservaService
{
    // Valida las reglas del negocio y guarda la reserva confirmada. Lanza ReglaDeNegocioException.
    Reserva Crear(int vueloId, int pasajeroId, string asiento);

    // Pasa la reserva a 'Cancelada' (no la borra) y libera el asiento.
    // Devuelve null si no existe; lanza ReglaDeNegocioException si ya estaba cancelada.
    Reserva? Cancelar(int id);
}
