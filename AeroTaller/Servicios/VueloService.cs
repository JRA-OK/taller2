// Formato del número, salida futura y origen ≠ destino los valida el ViewModel (no necesitan la base);
// aquí van las reglas que dependen de los datos guardados.
public class VueloService(IVueloRepository vuelos, IAeropuertoRepository aeropuertos, ILogger<VueloService> logger) : IVueloService
{
    public int Crear(Vuelo vuelo)
    {
        if (vuelos.ExisteNumero(vuelo.Numero))
            throw Rechazo("Numero", $"Ya existe el vuelo {vuelo.Numero}.", vuelo);
        if (aeropuertos.ObtenerPorId(vuelo.Origen.Id) is null)
            throw Rechazo("OrigenId", "El aeropuerto de origen no existe.", vuelo);
        if (aeropuertos.ObtenerPorId(vuelo.Destino.Id) is null)
            throw Rechazo("DestinoId", "El aeropuerto de destino no existe.", vuelo);

        int id = vuelos.Crear(vuelo);
        logger.LogInformation("Vuelo {VueloId} creado: {Numero}, salida {Salida:yyyy-MM-dd HH:mm}, {Filas} filas",
            id, vuelo.Numero, vuelo.Salida, vuelo.Filas);
        return id;
    }

    // Registra el rechazo y devuelve la excepción para lanzarla en el lugar de la regla
    private ReglaDeNegocioException Rechazo(string campo, string mensaje, Vuelo vuelo)
    {
        logger.LogWarning("Alta del vuelo {Numero} rechazada en {Campo}: {Motivo}", vuelo.Numero, campo, mensaje);
        return new ReglaDeNegocioException(campo, mensaje);
    }
}
