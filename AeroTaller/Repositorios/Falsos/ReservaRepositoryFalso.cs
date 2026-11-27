public class ReservaRepositoryFalso : IReservaRepository
{
    public List<Reserva> ObtenerTodas()
    {
        lock (DatosFalsos.Candado) return DatosFalsos.Reservas.OrderByDescending(r => r.Fecha).ToList();
    }

    public List<Reserva> ObtenerPorPasajero(int pasajeroId)
    {
        lock (DatosFalsos.Candado)
            return DatosFalsos.Reservas.Where(r => r.Pasajero.Id == pasajeroId).OrderBy(r => r.Vuelo.Salida).ToList();
    }

    public Reserva? ObtenerPorId(int id)
    {
        lock (DatosFalsos.Candado) return DatosFalsos.Reservas.FirstOrDefault(r => r.Id == id);
    }

    public List<string> ObtenerAsientosOcupados(int vueloId)
    {
        lock (DatosFalsos.Candado)
            return DatosFalsos.Reservas
                .Where(r => r.Vuelo.Id == vueloId && r.Estado == EstadoReserva.Confirmada)
                .Select(r => r.Asiento)
                .ToList();
    }

    public Dictionary<int, int> ContarConfirmadasPorVuelo()
    {
        lock (DatosFalsos.Candado)
            return DatosFalsos.Reservas
                .Where(r => r.Estado == EstadoReserva.Confirmada)
                .GroupBy(r => r.Vuelo.Id)
                .ToDictionary(g => g.Key, g => g.Count());
    }

    public bool ExisteConfirmada(int vueloId, int pasajeroId)
    {
        lock (DatosFalsos.Candado)
            return DatosFalsos.Reservas.Any(r => r.Vuelo.Id == vueloId && r.Pasajero.Id == pasajeroId && r.Estado == EstadoReserva.Confirmada);
    }

    public int Crear(Reserva reserva)
    {
        lock (DatosFalsos.Candado)
        {
            // Mismo resguardo que el índice único UX_Reserva_AsientoConfirmado de la base
            if (DatosFalsos.Reservas.Any(r => r.Vuelo.Id == reserva.Vuelo.Id && r.Asiento == reserva.Asiento && r.Estado == EstadoReserva.Confirmada))
                throw new ReglaDeNegocioException("Asiento", $"El asiento {reserva.Asiento} acaba de ser reservado por otra persona. Elegí otro.");

            reserva.Id = DatosFalsos.SiguienteId(DatosFalsos.Reservas, r => r.Id);
            reserva.Vuelo = DatosFalsos.Vuelos.First(v => v.Id == reserva.Vuelo.Id);
            reserva.Pasajero = DatosFalsos.Pasajeros.First(p => p.Id == reserva.Pasajero.Id);
            DatosFalsos.Reservas.Add(reserva);
            return reserva.Id;
        }
    }

    public bool Cancelar(int id)
    {
        lock (DatosFalsos.Candado)
        {
            Reserva? reserva = DatosFalsos.Reservas.FirstOrDefault(r => r.Id == id);
            if (reserva is null) return false;
            reserva.Estado = EstadoReserva.Cancelada;
            return true;
        }
    }
}
