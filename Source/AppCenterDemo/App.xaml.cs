using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AppCenterDemo
{
    public partial class App : Application
    {
        public const string LogTag = "AppCenterXamarinDemo";

        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
            AppCenter.LogLevel = LogLevel.Verbose;
            Crashes.ShouldAwaitUserConfirmation = UserConfirmationHandler;
            AppCenter.IsNetworkRequestsAllowed = Preferences.Get("Diagnostic", false);
            AppCenter.Start(
                "ios=96145684-8c4f-4637-8827-4f0a4e1c08a1;" +
                "android=1b6fa4c8-e136-4e1f-8193-48723aa7c919",
                typeof(Analytics), typeof(Crashes));
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }

        bool UserConfirmationHandler()
        {
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
            {
                var enabled = Preferences.Get("Diagnostic", false);
                if (enabled)
                {
                    var networkAccess = Connectivity.NetworkAccess;
                    if (networkAccess != NetworkAccess.Internet)
                    {
                        Current.MainPage.DisplayAlert(
                            "Notice",
                            "Failed to send crash report, please check your Internet connection.",
                            "OK");
                    }
                    Crashes.NotifyUserConfirmation(UserConfirmation.Send);
                }
                else
                {
                    Current.MainPage.DisplayActionSheet("Crash detected. Send anonymous crash report?", null, null,
                        "Send",
                        "Always Send",
                        "Don't Send").ContinueWith((arg) =>
                        {
                            var answer = arg.Result;
                            UserConfirmation userConfirmationSelection;
                            if (answer == "Send")
                            {
                                AppCenter.IsNetworkRequestsAllowed = true;
                                userConfirmationSelection = UserConfirmation.Send;
                            }
                            else if (answer == "Always Send")
                            {
                                Preferences.Set("Diagnostic", true); // Remember this settings by App, instead of AppCenter
                                AppCenter.IsNetworkRequestsAllowed = true;
                                userConfirmationSelection = UserConfirmation.Send;
                            }
                            else
                            {
                                userConfirmationSelection = UserConfirmation.DontSend;
                            }
                            if (userConfirmationSelection == UserConfirmation.Send)
                            {
                                var networkAccess = Connectivity.NetworkAccess;
                                if (networkAccess != NetworkAccess.Internet)
                                {
                                    Xamarin.Forms.Device.BeginInvokeOnMainThread(() => Current.MainPage.DisplayAlert(
                                        "Notice",
                                        "Internet connection unavailable, please check your Internet connection.",
                                        "OK"));
                                }
                            }
                            Crashes.NotifyUserConfirmation(userConfirmationSelection);
                        });
                }
            });
            return true;
        }
    }
}
