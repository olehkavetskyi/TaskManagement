﻿using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

public class RegisterUserDto
{
    [Required]
    public string Username { get; set; } = null!;
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;
    [Required]
    [RegularExpression("(?=^.{6,10}$)(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])" +
        "(?=.*[!@#$%^&amp;*()_+}{&quot;:;'?/&gt;.&lt;,])(?!.*\\s).*$",
        ErrorMessage = "Password must have one uppercase, one lovercase," +
        "one number, one non numeric and at least six characters")]
    public string Password { get; set; } = null!;
}