// El formato del DNI (7 u 8 dígitos) lo valida el ViewModel; la unicidad necesita la base y se valida aquí
public class PasajeroService(IPasajeroRepository pasajeros, IReservaRepository reservas, ILogger<PasajeroService> logger) : IPasajeroService
{
    public int Crear(Pasajero pasajero)
    {
        if (pasajeros.ExisteDni(pasajero.Dni))
        {
            logger.LogWarning("Alta de pasajero rechazada: el DNI {Dni} ya está registrado", pasajero.Dni);
            throw new ReglaDeNegocioException("Dni", $"Ya existe un pasajero con el DNI {pasajero.Dni}.");
        }
        int id = pasajeros.Crear(pasajero);
        logger.LogInformation("Pasajero {PasajeroId} registrado: {Nombre}, DNI {Dni}", id, pasajero.Nombre, pasajero.Dni);
        return id;
    }

    public Pasajero? Eliminar(int id)
    {
        Pasajero? pasajero = pasajeros.ObtenerPorId(id);
        if (pasajero is null) return null;

        // También cuentan las canceladas: son historial y la FK de Reserva las referencia
        int cantidad = reservas.ObtenerPorPasajero(id).Count;
        if (cantidad > 0)
        {
            logger.LogWarning("Baja del pasajero {PasajeroId} rechazada: tiene {Cantidad} reservas", id, cantidad);
            throw new ReglaDeNegocioException("Id", $"{pasajero.Nombre} tiene {cantidad} reserva(s) en su historial: no se puede eliminar.");
        }

        pasajeros.Eliminar(id);
        logger.LogInformation("Pasajero {PasajeroId} eliminado: {Nombre}, DNI {Dni}", id, pasajero.Nombre, pasajero.Dni);
        return pasajero;
    }
}
