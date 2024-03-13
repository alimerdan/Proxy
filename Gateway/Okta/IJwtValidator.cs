﻿using System.IdentityModel.Tokens.Jwt;

namespace Gateway.Okta
{
    public interface IJwtValidator
    {
        Task<JwtSecurityToken> ValidateToken(string token, CancellationToken ct = default(CancellationToken));
    }
}