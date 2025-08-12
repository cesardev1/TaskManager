using System.Security.Claims;

namespace TaskManager.Services;

public interface IUserServices
{
    string GetUserById();
}

public class UserServices : IUserServices
{
    private readonly HttpContext _httpContext;

    public UserServices(IHttpContextAccessor contextAccessor)
    {
         _httpContext = contextAccessor.HttpContext;
    }

    public string GetUserById()
    {
        if (_httpContext.User.Identity!.IsAuthenticated)
        {
            var idClaim = _httpContext.User.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault();
            
            return idClaim!.Value;
        }
        else
        {
            throw new Exception("El usuario no está autenticado");
        }
    }
    
} 