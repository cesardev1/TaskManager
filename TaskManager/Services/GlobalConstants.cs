using Microsoft.AspNetCore.Mvc.Rendering;

namespace TaskManager.Services;

public class GlobalConstants
{
    public const string AdminRole = "Admin";
    public static readonly SelectListItem[] SupportedCulturesUI = new SelectListItem[]
    {
        new SelectListItem{ Value= "es", Text = "Español"},
        new SelectListItem{ Value= "en", Text = "English"}
    };
}