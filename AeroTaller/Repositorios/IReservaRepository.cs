public interface IReservaRepository
{
    List<Reserva> ObtenerTodas();
    List<Reserva> ObtenerPorPasajero(int pasajeroId);
    Reserva? ObtenerPorId(int id);

    // Solo cuentan las reservas Confirmadas
    List<string> ObtenerAsientosOcupados(int vueloId);
    Dictionary<int, int> ContarConfirmadasPorVuelo();   // VueloId → confirmadas
    bool ExisteConfirmada(int vueloId, int pasajeroId);

    int Crear(Reserva reserva);   // devuelve el Id generado
    bool Cancelar(int id);        // Estado = 'Cancelada'; false si no existe
}
