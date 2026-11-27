// Una fila del tablero: el vuelo y cuántos asientos tiene confirmados
public record VueloOcupacionViewModel(Vuelo Vuelo, int Ocupados)
{
    public int Libres => Vuelo.Capacidad - Ocupados;
    public int Porcentaje => Vuelo.Capacidad == 0 ? 0 : Ocupados * 100 / Vuelo.Capacidad;
}
