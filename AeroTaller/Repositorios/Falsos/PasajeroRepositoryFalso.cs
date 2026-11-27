public class PasajeroRepositoryFalso : IPasajeroRepository
{
    public List<Pasajero> ObtenerTodos(string? buscar = null)
    {
        string? texto = string.IsNullOrWhiteSpace(buscar) ? null : buscar.Trim();
        lock (DatosFalsos.Candado)
            return DatosFalsos.Pasajeros
                .Where(p => texto is null
                    || p.Nombre.Contains(texto, StringComparison.OrdinalIgnoreCase)
                    || p.Dni.Contains(texto))
                .OrderBy(p => p.Nombre)
                .ToList();
    }

    public Pasajero? ObtenerPorId(int id)
    {
        lock (DatosFalsos.Candado) return DatosFalsos.Pasajeros.FirstOrDefault(p => p.Id == id);
    }

    public bool ExisteDni(string dni)
    {
        lock (DatosFalsos.Candado) return DatosFalsos.Pasajeros.Any(p => p.Dni == dni);
    }

    public int Crear(Pasajero pasajero)
    {
        lock (DatosFalsos.Candado)
        {
            pasajero.Id = DatosFalsos.SiguienteId(DatosFalsos.Pasajeros, p => p.Id);
            DatosFalsos.Pasajeros.Add(pasajero);
            return pasajero.Id;
        }
    }

    public bool Eliminar(int id)
    {
        lock (DatosFalsos.Candado) return DatosFalsos.Pasajeros.RemoveAll(p => p.Id == id) > 0;
    }
}
