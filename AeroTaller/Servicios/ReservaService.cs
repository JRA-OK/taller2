using System.Text.RegularExpressions;

public partial class ReservaService(
    IReservaRepository reservas, IVueloRepository vuelos, IPasajeroRepository pasajeros,
    ILogger<ReservaService> logger) : IReservaService
{
    // Fila (número sin ceros a la izquierda) + letra A–F: 1A, 12C, 20F
    [GeneratedRegex("^([1-9][0-9]*)([A-F])$")]
    private static partial Regex FormatoAsiento();

    public Reserva Crear(int vueloId, int pasajeroId, string asiento)
    {
        asiento = (asiento ?? "").Trim().ToUpperInvariant();   // '5b' → '5B'
        try
        {
            (Vuelo vuelo, Pasajero pasajero) = Validar(vueloId, pasajeroId, asiento);
            var reserva = new Reserva
            {
                Vuelo = vuelo,
                Pasajero = pasajero,
                Asiento = asiento,
                Fecha = DateTime.Now,
                Estado = EstadoReserva.Confirmada
            };
            reserva.Id = reservas.Crear(reserva);   // puede rechazarla la base (asiento tomado en simultáneo)

            // Plantilla con propiedades {…}: Serilog guarda los valores por separado (se pueden filtrar y buscar)
            logger.LogInformation("Reserva {ReservaId} confirmada: vuelo {Vuelo}, asiento {Asiento}, pasajero {PasajeroId} ({Pasajero})",
                reserva.Id, vuelo.Numero, asiento, pasajero.Id, pasajero.Nombre);
            return reserva;
        }
        catch (ReglaDeNegocioException ex)
        {
            // Una regla rechazada no es un error del sistema: Warning, sin la traza
            logger.LogWarning("Reserva rechazada (vuelo {VueloId}, pasajero {PasajeroId}, asiento {Asiento}) en {Campo}: {Motivo}",
                vueloId, pasajeroId, asiento, ex.Campo, ex.Message);
            throw;   // la vuelve a lanzar sin perder la información original: el controlador la muestra en el formulario
        }
    }

    public Reserva? Cancelar(int id)
    {
        Reserva? reserva = reservas.ObtenerPorId(id);
        if (reserva is null) return null;
        if (reserva.Estado == EstadoReserva.Cancelada)
            throw new ReglaDeNegocioException("Estado", "La reserva ya estaba cancelada.");

        reservas.Cancelar(id);
        reserva.Estado = EstadoReserva.Cancelada;
        logger.LogInformation("Reserva {ReservaId} cancelada: vuelo {Vuelo}, asiento {Asiento} liberado",
            reserva.Id, reserva.Vuelo.Numero, reserva.Asiento);
        return reserva;
    }

    // Las reglas del negocio, de lo general a lo particular. Cada una informa el campo del formulario.
    private (Vuelo, Pasajero) Validar(int vueloId, int pasajeroId, string asiento)
    {
        // Vuelo → campo VueloId
        Vuelo vuelo = vuelos.ObtenerPorId(vueloId)
            ?? throw new ReglaDeNegocioException("VueloId", "El vuelo no existe.");
        if (vuelo.Salida <= DateTime.Now)
            throw new ReglaDeNegocioException("VueloId", "El vuelo ya salió: no admite reservas.");

        var ocupados = reservas.ObtenerAsientosOcupados(vueloId);
        if (ocupados.Count >= vuelo.Capacidad)
            throw new ReglaDeNegocioException("VueloId", $"El vuelo está completo ({vuelo.Capacidad} asientos).");

        // Asiento → campo Asiento
        Match formato = FormatoAsiento().Match(asiento);
        if (!formato.Success)
            throw new ReglaDeNegocioException("Asiento", "El asiento debe ser fila + letra de la A a la F (por ejemplo 12C).");
        int fila = int.Parse(formato.Groups[1].Value);
        if (fila > vuelo.Filas)
            throw new ReglaDeNegocioException("Asiento", $"El avión tiene {vuelo.Filas} filas: el asiento {asiento} no existe.");
        if (ocupados.Contains(asiento))
            throw new ReglaDeNegocioException("Asiento", $"El asiento {asiento} ya está ocupado.");

        // Pasajero → campo PasajeroId
        Pasajero pasajero = pasajeros.ObtenerPorId(pasajeroId)
            ?? throw new ReglaDeNegocioException("PasajeroId", "El pasajero no existe.");
        if (reservas.ExisteConfirmada(vueloId, pasajeroId))
            throw new ReglaDeNegocioException("PasajeroId", $"{pasajero.Nombre} ya tiene una reserva confirmada en el vuelo {vuelo.Numero}.");

        return (vuelo, pasajero);
    }
}
