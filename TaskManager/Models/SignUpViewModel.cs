using System.ComponentModel.DataAnnotations;
namespace TaskManager.Models;


public class SignUpViewModel
{
    [Required(ErrorMessage = "El correo electrónico es requerido")]
    [EmailAddress(ErrorMessage = "Correo electrónico no válido")]
    public string Email { get; set; }
    [Required(ErrorMessage = "La contraseña es requerida")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}