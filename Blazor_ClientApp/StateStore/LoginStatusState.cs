using SharedModels.Models;
namespace Blazor_ClientApp.StateStore
{
    public class LoginStatusState
    {
        public bool IsAuthenticated { get; set; } = false;
        public ResponseStatus? Response { get; set; } = null;

        public LoginStatusState()
        {
            Response = new ResponseStatus();
        }

        public event Action? OnAuthenticatedStatusChange;

        public void ChangeLoginStatus(ResponseStatus status)
        {
            Response = status;
            NotifyLoginStatusChanged();
        }


        private void NotifyLoginStatusChanged()=> OnAuthenticatedStatusChange?.Invoke();
    }
}
