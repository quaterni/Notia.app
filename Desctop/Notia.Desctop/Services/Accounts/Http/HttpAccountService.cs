
using Microsoft.Extensions.Options;
using Notia.Desctop.Services.Abstractions;
using Notia.Desctop.Services.Accounts.Abstractions;
using Notia.Desctop.Services.Accounts.Http.Requests;
using Notia.Desctop.Services.Accounts.Http.Response;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Notia.Desctop.Services.Accounts.Http;

internal class HttpAccountService : IAccountService
{
    private readonly HttpClient _httpClient;
    private readonly HttpAccountServiceOptions _options;

    public HttpAccountService(
        HttpClient httpClient,
        IOptions<HttpAccountServiceOptions> options)
    {
        _options = options.Value;
        _httpClient = httpClient;
    }

    public async Task<Result<Token>> Login(string username, string password, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync(
            _options.LoginUrl, 
            new LoginRequest() 
            {
                Username = username,
                Password = password
            });

        if (response.StatusCode.Equals(HttpStatusCode.BadRequest))
        {
            return Result.Failure<Token>(AccountServiceErrors.Unauthorized);
        }
        response.EnsureSuccessStatusCode();

        var loginResponse = await response.Content.ReadFromJsonAsync<LoginSuccessResponse>() 
            ?? throw new ApplicationException("Cannot serialize login response");

        return new Token(loginResponse.Token.AccessToken, loginResponse.Token.RefreshToken);
    }

    public Task<Result<Token>> UpdateToken(string refreshToken, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
