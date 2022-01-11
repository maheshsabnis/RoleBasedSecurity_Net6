using Blazor_ClientApp;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Blazor_ClientApp.StateStore;
using Blazor_ClientApp.ClientCaller;
using Microsoft.AspNetCore.Components.Authorization;
using Blazor_ClientApp.CustomProvider;
using Blazored.SessionStorage;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddAuthorizationCore();
// Registe the Custom Auth Provider
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddBlazoredSessionStorage();

// Register the Client Proxy Services for Security and Order
builder.Services.AddScoped<SecurityProxy>();
builder.Services.AddScoped<OrdersProxy>();


await builder.Build().RunAsync();
