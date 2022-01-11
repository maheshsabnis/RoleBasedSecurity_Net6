using Microsoft.AspNetCore.Components.Authorization;
using Blazor_ClientApp.StateStore;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text.Json;
using Blazor_ClientApp.ClientCaller;

namespace Blazor_ClientApp.CustomProvider
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ISessionStorageService _sessionStorageService;
         
        public CustomAuthStateProvider(ISessionStorageService session)
        {
            _sessionStorageService = session;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            // get the token from the state
            string? token = await _sessionStorageService.GetItemAsync<string>("Token");
            if (string.IsNullOrEmpty(token))
            {
                var anonymous = new ClaimsIdentity();
                // This means the the Login is not yet takes place so Claims are Empty
                var noClaim = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(anonymous)));
                return noClaim;
            }

            // Read Claims from the Token
            var userClaims = new ClaimsPrincipal(new ClaimsIdentity(GetClaimsFromToken(token), "Fake"));
            var authUser = new AuthenticationState(userClaims);
            
            return authUser;

            
        }

        public void NotifyAuthStatus()
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        private List<Claim> GetClaimsFromToken(string token)
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            // read the token values
            var jwtSecurityToken = jwtHandler.ReadJwtToken(token);
            // read claims
            var claims = jwtSecurityToken.Claims;
            return claims.ToList();
        }
    }
}
