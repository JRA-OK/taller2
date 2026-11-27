public class Vuelo
{
    public const string LetrasAsiento = "ABCDEF";   // A B C | pasillo | D E F
    public const int AsientosPorFila = 6;

    public int Id { get; set; }
    public string Numero { get; set; } = "";   // 'AT1001'

    // Composición: el vuelo tiene sus aeropuertos, no solo los Id (OrigenId/DestinoId viven en la tabla)
    public Aeropuerto Origen { get; set; } = new();
    public Aeropuerto Destino { get; set; } = new();

    public DateTime Salida { get; set; }
    public int DuracionMinutos { get; set; }
    public int Filas { get; set; }
    public decimal Precio { get; set; }

    public int Capacidad => Filas * AsientosPorFila;
    public DateTime Llegada => Salida.AddMinutes(DuracionMinutos);
}
