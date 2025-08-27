using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using AndroidX.Core.View;

namespace Connect4Game
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, 
              ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            // Enable full screen mode
            if (Build.VERSION.SdkInt >= BuildVersionCodes.R)
            {
                // For Android 11 (API 30) and above
                Window?.SetFlags(WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);
                WindowCompat.SetDecorFitsSystemWindows(Window, false);
            }
            else
            {
                // For older Android versions
                Window?.SetFlags(WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);
                
                // Hide navigation bar and status bar
                var decorView = Window?.DecorView;
                if (decorView != null)
                {
                    var uiOptions = (int)decorView.SystemUiVisibility;
                    uiOptions |= (int)SystemUiFlags.HideNavigation;
                    uiOptions |= (int)SystemUiFlags.Fullscreen;
                    uiOptions |= (int)SystemUiFlags.ImmersiveSticky;
                    decorView.SystemUiVisibility = (StatusBarVisibility)uiOptions;
                }
            }
        }
    }
}
