﻿namespace Donace_BE_Project.Models.User;

public class UserModel
{
    public Guid Id { get; set; }

    public string UserName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
}