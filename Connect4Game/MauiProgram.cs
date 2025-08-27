using Microsoft.Extensions.Logging;
using Connect4Game.Services;
using Connect4Game.ViewModels;
using Connect4Game.Views;
using Connect4Game.Converters;

namespace Connect4Game
{
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
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Register services - using Demo services for easier testing
            // To use real Bluetooth, replace DemoBluetoothService with BluetoothService
            // and DemoGameService with GameService
#if DEBUG
            builder.Services.AddSingleton<IBluetoothService, DemoBluetoothService>();
            builder.Services.AddSingleton<IGameService, DemoGameService>();
#else
            builder.Services.AddSingleton<IBluetoothService, BluetoothService>();
            builder.Services.AddSingleton<IGameService, GameService>();
#endif
            
            // Register ViewModels
            builder.Services.AddTransient<MainMenuViewModel>();
            builder.Services.AddTransient<GameViewModel>();
            
            // Register Views
            builder.Services.AddTransient<MainMenuPage>();
            builder.Services.AddTransient<GamePage>();

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
