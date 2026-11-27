// Detalle del vuelo + los asientos ocupados (reservas confirmadas) para dibujar el mapa
public record VueloDetalleViewModel(Vuelo Vuelo, IReadOnlySet<string> Ocupados)
{
    public int Libres => Vuelo.Capacidad - Ocupados.Count;

    public bool EstaOcupado(string asiento) => Ocupados.Contains(asiento);
}
