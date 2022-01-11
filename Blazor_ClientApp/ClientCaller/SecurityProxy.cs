using SharedModels.Models;
using System.Net.Http.Json;
using Blazor_ClientApp.CustomProvider;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Blazored.SessionStorage;

namespace Blazor_ClientApp.ClientCaller
{
    public class SecurityProxy
    {
        private HttpClient _httpClient;
        private string url;
        private readonly ISessionStorageService _sessionStorageService;

        private AuthenticationStateProvider _customAuthStateProvider;
        public SecurityProxy(AuthenticationStateProvider authState, ISessionStorageService session)
        {
            _httpClient = new HttpClient();
            url = "https://localhost:7145";
            _customAuthStateProvider=authState;
            _sessionStorageService = session;

        }


        public async Task<ResponseStatus> RegisterUserAsync(RegisterUser user)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync<RegisterUser>($"{url}/post/register/user", user);
                if (!response.IsSuccessStatusCode)
                {
                    var message = await response.Content.ReadFromJsonAsync<ResponseStatus>();
                    throw new Exception(message.Message);
                }
                var responseStatus = await response.Content.ReadFromJsonAsync<ResponseStatus>();
              
                return responseStatus;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ResponseStatus> AuthenticateUserAsync(LoginUser user)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync<LoginUser>($"{url}/post/auth/user", user);
                if (!response.IsSuccessStatusCode)
                {
                    var message = await response.Content.ReadFromJsonAsync<ResponseStatus>();
                    throw new Exception(message.Message);
                }
                var responseStatus = await response.Content.ReadFromJsonAsync<ResponseStatus>();
                // store the information in the session Storage
                await _sessionStorageService.SetItemAsync("UserName", responseStatus.UserName);
                await _sessionStorageService.SetItemAsync("RoleName", responseStatus.Role);
                await _sessionStorageService.SetItemAsync("Token", responseStatus.Token);
                // if th e login is successull then notify the client the login is successful
                ((CustomAuthStateProvider)_customAuthStateProvider).NotifyAuthStatus();
                return responseStatus;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<ResponseStatus> CreateNewRole(ApplicationRole userRole)
        {
            return null;
        }

        public async Task<ResponseStatus> AssignRoleToUserAsync(UserRole userRole)
        {
            return null;
        }

        public async Task<Users> GetUsersAsync()
        {
            return null;
        }

        public async Task<ApplicationRole> GetRolesAsync()
        {
            return null;
        }

        public async Task<bool> LogoutAsync()
        {
            await _sessionStorageService.RemoveItemAsync("Token");
            await _sessionStorageService.RemoveItemAsync("UserName");
            await _sessionStorageService.RemoveItemAsync("RoleName");
            ((CustomAuthStateProvider)_customAuthStateProvider).NotifyAuthStatus();
            return true;
        }


    }
}
