﻿using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebApiDocumentUploader.DB;
using WebApiDocumentUploader.DBService;

namespace WebApiDocumentUploader.Auth
{
    public class TokenAuthenticationService : IAuthenticateService
    {
        private readonly IUserService _userService;
        private readonly TokenManagement _tokenManagement;
        public TokenAuthenticationService(IUserService userService, IOptions<TokenManagement> tokenManagement)
        {
            _userService = userService;
            _tokenManagement = tokenManagement.Value;
        }

        public bool IsAuthenticated(LoginRequestDTO request, out string token)
        {
            token = string.Empty;
            if (!_userService.IsValid(request, out User user))
            {
                token = "用户名或密码不正确";
                return false;
            }
            var claims = new[]
            {
                new Claim(ClaimTypes.PrimarySid, user.UserId),
                new Claim(ClaimTypes.Name, request.Username)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenManagement.Secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            DateTime expireTime = DateTime.Now.AddMinutes(_tokenManagement.AccessExpiration);
            var jwtToken = new JwtSecurityToken(_tokenManagement.Issuer, _tokenManagement.Audience, claims, expires: expireTime, signingCredentials: credentials);

            token = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            return true;

        }
    }
}
