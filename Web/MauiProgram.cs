using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using RazorLibrary.HttpServices;
using RazorLibrary.Services;

namespace Web;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			});

		builder.Services.AddMauiBlazorWebView();
 
		builder.Services.AddScoped<IAuthService, HybridAuthService>();
     ///   builder.Services.AddHttpClient<LoginHttpService>();
        builder.Services.AddHttpClient<LoginHttpService>(client =>
        {
            client.BaseAddress = new Uri("https://localhost:7093/api/"); // Set your API base URL
        });
		builder.Services.AddHttpClient<UserAccountHttpService>(client =>
        {
            client.BaseAddress = new Uri("https://localhost:7093/api/"); // Set your API base URL
        });
        builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
            (AuthenticationStateProvider)sp.GetRequiredService<IAuthService>());


#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
