using System.ComponentModel.DataAnnotations;

// [FechaFutura]: la fecha debe ser posterior al momento actual.
// Un valor null se considera válido: que el campo sea obligatorio lo decide [Required].
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class FechaFuturaAttribute : ValidationAttribute
{
    public FechaFuturaAttribute() : base("{0} debe ser una fecha futura.") { }

    public override bool IsValid(object? value) => value switch
    {
        null => true,
        DateTime fecha => fecha > DateTime.Now,
        _ => false
    };
}
