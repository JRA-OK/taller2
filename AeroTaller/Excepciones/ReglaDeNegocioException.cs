// La lanza la capa de servicios cuando se viola una regla del negocio.
// Campo = nombre de la propiedad del formulario donde se muestra el error (ModelState.AddModelError(Campo, …)).
public class ReglaDeNegocioException(string campo, string mensaje) : Exception(mensaje)
{
    public string Campo { get; } = campo;
}
