using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SmartPark.Frontend;
using SmartPark.Frontend.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiBaseUrl = "https://localhost:5067";
builder.Services.AddTransient<CookieHandler>();
builder.Services.AddScoped(sp =>
    new HttpClient(new CookieHandler())
    {
        BaseAddress = new Uri(apiBaseUrl)
    });

builder.Services.AddScoped<ParkingSpaceService>();
builder.Services.AddScoped<ReservationService>();
builder.Services.AddScoped<AiDetectionService>();
builder.Services.AddScoped<AuthService>();

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<CookieAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<CookieAuthStateProvider>());

var host = builder.Build();

await host.RunAsync();