using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager.Models;
using TaskManager.Services;

namespace TaskManager.Controllers;

public class UsersController: Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly ApplicationDbContext _context;

    public UsersController(UserManager<IdentityUser> userManager, 
                            SignInManager<IdentityUser> signInManager,
                            ApplicationDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
    }
    
    [AllowAnonymous]
    public IActionResult SignUp()
    {
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> SignUp(SignUpViewModel model)
    {
        if(!ModelState.IsValid)
        {
            return View(model);
        }
        var user = new IdentityUser
        {
            UserName = model.Email,
            Email = model.Email
        };
        var result = await _userManager.CreateAsync(user, model.Password);
        if(result.Succeeded)
        {
            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Index", "Home");
        }
        else
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty,error.Description);
            }
            return View(model);
        }
    }
    
    [AllowAnonymous]
    public IActionResult Login(string message =null)
    {
        if(message is not null)
            ViewData["Message"] = message;
        return View();
    }
    
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if(!ModelState.IsValid)
        {
            return View(model);
        }

        var result =
            await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe,
                lockoutOnFailure: false);
        if (result.Succeeded)
        {
            return RedirectToAction("Index", "Home");
        }
        else
        {
            ModelState.AddModelError(string.Empty,"Nombre de usuario o password incorrectos");
            return View(model);       
        }
        
    }
    
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
        return RedirectToAction("Index", "Home");
    }

    [AllowAnonymous]
    [HttpGet]
    public ChallengeResult ExternalLogin(string provider, string urlReturn)
    {
        var urlRedirect = Url.Action("ExternalLoginCallback", values: new { urlReturn });
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, urlRedirect);
        return new ChallengeResult(provider, properties);
    }

    [AllowAnonymous]
    public async Task<IActionResult> ExternalLoginCallback(string returnUrl, string remoteError = null)
    {
        returnUrl = returnUrl ?? Url.Content("~/");
        var message = "";
        if (remoteError is not null)
        {
            message = $"Error del proveedor externo: {remoteError}";
            return RedirectToAction("Login",routeValues: new { message });
        }
        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info is null)
        {
            message = "Error cargando la data del login externo";
            return RedirectToAction("Login",routeValues: new { message });
        }

        var resultExternalLogin = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey,
            isPersistent: true, bypassTwoFactor: true);

        if (resultExternalLogin.Succeeded) // User have account
        {
            return LocalRedirect(returnUrl);
        }

        string email = "";

        if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
        {
            email = info.Principal.FindFirstValue(ClaimTypes.Email);
        }
        else
        {
            message = "Error leyendo el email del usuario del proveedor";
            return RedirectToAction("Login", routeValues: new { message });
        }

        var user = new IdentityUser { Email = email, UserName = email };
        
        var resultCreateUser = await _userManager.CreateAsync(user);

        if (!resultCreateUser.Succeeded)
        {
            message = resultCreateUser.Errors.First().Description;
            return RedirectToAction("Login", routeValues: new { message });
        }
        
        var resultAddLogin = await _userManager.AddLoginAsync(user, info);

        if (resultAddLogin.Succeeded)
        {
            await _signInManager.SignInAsync(user,isPersistent:true,info.LoginProvider);
            return LocalRedirect(returnUrl);
        }

        message = "Ha ocurrido un error agregando el login";
        return RedirectToAction("Login",routeValues: new { message });
    }

    [HttpGet]
    public async Task<IActionResult> ListUsers(string message = null)
    {
        var users = await _context.Users.Select(u=> new UserViewModel
        {
            Email = u.Email
        }).ToListAsync();

        var model = new UserListViewModel();
        model.Users = users;
        model.Message = message;
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> PromoteToAdmin(string email)
    {
        var user = await _context.Users.Where(u=> u.Email==email).FirstOrDefaultAsync();
        
        if(user is null)
            return NotFound();

        await _userManager.AddToRoleAsync(user, GlobalConstants.AdminRole);
        
        return RedirectToAction("ListUsers",routeValues: new { message = "Se agrego el rol Admin a " + email });
    }
    
    [HttpPost]
    public async Task<IActionResult> RemoveAdminRole(string email)
    {
        var user = await _context.Users.Where(u=> u.Email==email).FirstOrDefaultAsync();
        
        if(user is null)
            return NotFound();

        await _userManager.RemoveFromRoleAsync(user, GlobalConstants.AdminRole);
        
        return RedirectToAction("ListUsers",routeValues: new { message = "Se retiro el rol Admin a " + email });
    }
}