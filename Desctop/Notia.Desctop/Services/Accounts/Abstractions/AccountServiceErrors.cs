using Notia.Desctop.Services.Abstractions;

namespace Notia.Desctop.Services.Accounts.Abstractions;

internal class AccountServiceErrors
{
    public static Error Unauthorized => new("AccountService:Unauthorized", "Login was failed");
}
