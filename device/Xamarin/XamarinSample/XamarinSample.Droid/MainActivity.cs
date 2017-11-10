using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using XamarinShared;

namespace XamarinSample.Droid
{
	[Activity (Label = "XamarinSample", Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
    {
        public Authentication auth;
        protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			global::Xamarin.Forms.Forms.Init (this, bundle);
			LoadApplication (new XamarinSample.App ());

            auth = new Authentication(Microsoft.Azure.Devices.Client.TransportType.Amqp);
            TestAuthConnStringAndSasToken();

        }

        public void TestAuthConnStringAndSasToken()
        {
            var connString = auth.AuthViaConnectionString("HostName=jasminel-h1.azure-devices.net;DeviceId=DeviceId_10109;SharedAccessKey=P61ZA8kaPcJAEJVIpG9eMuEfZvPuHqatbet0spv5Fg8=");
            var sasToken = auth.AuthViaSaSToken("HostName=jasminel-h1.azure-devices.net;DeviceId=DeviceId_10109;SharedAccessSignature=SharedAccessSignature sr=jasminel-h1.azure-devices.net%2Fdevices%2FDeviceId_10109&sig=A6iGHKH1ESpfnoNAM%2FP9QezsXiBryy1rdtH6kOhoSaA%3D&se=1533403744");

            Toast.MakeText(this, "Making device call via Connection String", ToastLength.Long);
            connString.SendEventAsync(new Microsoft.Azure.Devices.Client.Message(Encoding.ASCII.GetBytes("Sending With Connection String")));

            Toast.MakeText(this, "Making device call via SasToken", ToastLength.Long);
            Task.Delay(1000).Wait();
            sasToken.SendEventAsync(new Microsoft.Azure.Devices.Client.Message(Encoding.ASCII.GetBytes("Sending With SasToken")));

            Toast.MakeText(this, string.Format("Current TIme: {0}", DateTime.Now), ToastLength.Long).Show();
        }
    }
}

