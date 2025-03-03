using Blazored.LocalStorage;
using MedicalRecordWeb.Components;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using RazorLibrary.HttpServices;
using RazorLibrary.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddHttpClient<LoginHttpService>();
builder.Services.AddHttpClient<UserAccountHttpService>();
builder.Services.AddHttpClient<IPDPatientHttpService>();
builder.Services.AddHttpClient<OPDPatientHttpService>();
builder.Services.AddScoped<IAuthService, WebAuthService>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => (WebAuthService)sp.GetRequiredService<IAuthService>());
builder.Services.AddBlazoredLocalStorage();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
