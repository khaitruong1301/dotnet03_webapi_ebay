using System.ComponentModel.DataAnnotations;

public class UserLoginVM
{
    //Validation
    [Required(ErrorMessage = "Username hoặc email không được bỏ trống !")]
    public string userNameOrEmail { get; set; }
    [RegularExpression(@"^(?=.*[a-zA-Z])(?=.*\d)(?=.*[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]).{8,}$",
       ErrorMessage = "Password phải có ít nhất 8 ký tự, bao gồm chữ cái, số và ký tự đặc biệt.")]

    public string password { get; set; }

}


public class UserLoginResultVM
{
    public string token { get; set; }
}