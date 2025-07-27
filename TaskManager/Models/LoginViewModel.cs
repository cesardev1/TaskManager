using System.ComponentModel.DataAnnotations;

namespace TaskManager.Models;

public class LoginViewModel
{
    [Required(ErrorMessage = "El correo electrónico es requerido")]
    [EmailAddress(ErrorMessage = "El correo electrónico no es válido")]
    public string Email { get; set; }
    [Required(ErrorMessage = "La contraseña es requerida")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    [Display(Name = "Recordarme")]
    public bool RememberMe { get; set; }
}