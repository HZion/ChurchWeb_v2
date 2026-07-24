using System.ComponentModel.DataAnnotations;

namespace ChurchWeb.Web.Models.Account;

public class LoginViewModel
{
    [Required(ErrorMessage = "사용자 이름을 입력하세요.")]
    [Display(Name = "사용자 이름")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "비밀번호를 입력하세요.")]
    [DataType(DataType.Password)]
    [Display(Name = "비밀번호")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "로그인 상태 유지 (30일)")]
    public bool RememberMe { get; set; }

    public string? ReturnUrl { get; set; }
}
