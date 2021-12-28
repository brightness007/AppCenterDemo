using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AppCenterDemo
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            var enabled = Preferences.Get("Diagnostic", false);
            this.SendDiagnostic = enabled;

            Xamarin.Forms.Device.BeginInvokeOnMainThread(async () => await Update_AppCenter_Status());
        }

        private async void Write_Log_Button_Clicked(object sender, EventArgs e)
        {
            System.Guid? installId = await AppCenter.GetInstallIdAsync();
            var now = DateTime.Now;

            var trackEventCount = Preferences.Get("TrackEventCount", 0);
            ++trackEventCount;
            Preferences.Set("TrackEventCount", trackEventCount);

            Analytics.TrackEvent("Track Event A", new Dictionary<string, string>
            {
                { "InstallId", installId?.ToString() },
                { "TrackEventCount", trackEventCount.ToString() },
                { "Time", now.ToString() }
            });
        }

        private void Crash_It_Button_Clicked(object sender, EventArgs e)
        {
            throw new Exception("Crash it at " + DateTime.Now);
        }

        public bool SendDiagnostic
        {
            get => Preferences.Get("Diagnostic", false);
            set
            {
                Preferences.Set("Diagnostic", value);
                AppCenter.IsNetworkRequestsAllowed = value;
            }
        }

        private async void Check_Status_Button_Clicked(object sender, EventArgs e)
        {
            await Update_AppCenter_Status();
        }

        private async Task Update_AppCenter_Status()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"AppCenter.IsEnabled = {await AppCenter.IsEnabledAsync()}");
            sb.AppendLine($"AppCenter.IsNetworkRequestsAllowed = {AppCenter.IsNetworkRequestsAllowed}");
            sb.AppendLine($"AppCenter.LogLevel = {AppCenter.LogLevel}");
            sb.AppendLine($"Crashes.IsEnabled = {await Crashes.IsEnabledAsync()}");
            sb.AppendLine($"Analytics.IsEnabled = {await Analytics.IsEnabledAsync()}");
            this.AppCenterStatus.Text = sb.ToString();
        }
    }
}
