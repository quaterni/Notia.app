using Notia.Desctop.Services.Abstractions;
using System.Threading;
using System.Threading.Tasks;

namespace Notia.Desctop.Services.Accounts.Abstractions;

internal interface IAccountService
{
    Task<Result<Token>> Login(string username, string password, CancellationToken cancellationToken = default);

    Task<Result<Token>> UpdateToken(string refreshToken, CancellationToken cancellationToken = default);
}
