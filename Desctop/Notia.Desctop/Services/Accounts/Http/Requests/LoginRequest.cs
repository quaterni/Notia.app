﻿
namespace Notia.Desctop.Services.Accounts.Http.Requests;

internal class LoginRequest
{
    public required string Username { get; init; }
    public required string Password { get; init; }
}
