public class Reserva
{
    public int Id { get; set; }
    public Vuelo Vuelo { get; set; } = new();
    public Pasajero Pasajero { get; set; } = new();
    public string Asiento { get; set; } = "";   // fila + letra: '12A'
    public DateTime Fecha { get; set; }         // cuándo se hizo la reserva
    public EstadoReserva Estado { get; set; } = EstadoReserva.Confirmada;
}
