public class Pasajero
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Dni { get; set; } = "";
    public string? Email { get; set; }   // opcional: la columna admite NULL
}
