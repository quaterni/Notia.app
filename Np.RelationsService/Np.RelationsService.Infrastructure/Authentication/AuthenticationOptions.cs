﻿
namespace Np.RelationsService.Infrastructure.Authentication;

internal class AuthenticationOptions
{
    public string MetadataAddress { get; set; } = string.Empty;
    public string ValidIssuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
}
