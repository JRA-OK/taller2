using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

// [RequiereRol(Roles.Admin)]: sin sesión → login (como RequiereLogin); con sesión pero sin el rol → vista SinPermiso (403).
// Admite varios roles: [RequiereRol(Roles.Admin, Roles.Mostrador)] = cualquiera de ellos.
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class RequiereRolAttribute(params string[] roles) : RequiereLoginAttribute
{
    public string[] RolesPermitidos { get; } = roles;

    public override void OnAuthorization(AuthorizationFilterContext context)
    {
        var autenticacion = context.HttpContext.RequestServices.GetRequiredService<IAutenticacionService>();
        if (!autenticacion.EstaAutenticado)
            context.Result = RedirigirAlLogin(context);
        else if (!autenticacion.TieneRol(RolesPermitidos))
            context.Result = new ViewResult { ViewName = "SinPermiso", StatusCode = StatusCodes.Status403Forbidden };
    }
}
