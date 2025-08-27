using Foundation;
using UIKit;

namespace Connect4Game
{
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate
    {
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
        
        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            var result = base.FinishedLaunching(application, launchOptions);
            
            // Hide status bar for full screen experience
            UIApplication.SharedApplication.SetStatusBarHidden(true, UIStatusBarAnimation.None);
            
            return result;
        }
    }
}
