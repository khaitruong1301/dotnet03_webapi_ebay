using System.ComponentModel.DataAnnotations;

namespace EbayProject.Api.ViewModel;

public class UserRegisterVM
{
    [Required(ErrorMessage = "Username cannot be blank!")]
    public string? UserName { get; set; } = "";
    [Required(ErrorMessage = "Password cannot be blank!")]
    [RegularExpression(@"^(?=.*[a-zA-Z])(?=.*\d)(?=.*[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]).{8,}$",
       ErrorMessage = "Password phải có ít nhất 8 ký tự, bao gồm chữ cái, số và ký tự đặc biệt.")]
    public string? Password { get; set; } = "";
    [Required(ErrorMessage = "FullName cannot be blank!")]

    public string? FullName { get; set; } = "";
    [Required(ErrorMessage = "Email cannot be blank!")]
    [RegularExpression(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$",
       ErrorMessage = "Email không hợp lệ.")]
    public string? Email { get; set; } = "";

    public UserRegisterVM()
    {
        
    }
}



