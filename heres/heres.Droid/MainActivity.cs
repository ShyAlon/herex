using System;

using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using System.Threading.Tasks;
using Android.Util;
using Android.Content;
using heres.Droid.Utils;
using Android.Gms.Common;

namespace heres.Droid
{
    [Activity (Label = "", Icon = "@drawable/icon", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			global::Xamarin.Forms.Forms.Init (this, bundle);

            if (IsPlayServicesAvailable())
            {
                var intent = new Intent(this, typeof(RegistrationIntentService));
                StartService(intent);
            }
            LoadApplication (new heres.App ());
		}

        public bool IsPlayServicesAvailable()
        {
            var resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
            if (resultCode != ConnectionResult.Success)
            {
                if (GoogleApiAvailability.Instance.IsUserResolvableError(resultCode))
                {
                    Log.Error("Error", GoogleApiAvailability.Instance.GetErrorString(resultCode));
                } 
                else
                {
                    Log.Error("Error", "Sorry, this device is not supported");
                    Finish();
                }
                return false;
            }
            else
            {
                Log.Error("Error", "Google Play Services is available.");
                return true;
            }
        }
    }



    [Activity(Theme = "@style/MyTheme.Splash", MainLauncher = true, NoHistory = true, Label = "")]
    public class SplashActivity : AppCompatActivity
    {
        static readonly string TAG = "X:" + typeof(SplashActivity).Name;

        public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState)
        {
            base.OnCreate(savedInstanceState, persistentState);
            Log.Debug(TAG, "SplashActivity.OnCreate");
        }

        protected override void OnResume()
        {
            base.OnResume();

            var startupWork = new Task(() => {
                try
                {
                    Log.Debug(TAG, "Performing some startup work that takes a bit of time.");
                    var res = (new Calendar()).GetEvents(this);
                    // InsertToDB(res);
                    Log.Debug(TAG, "Working in the background - important stuff.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            });

            startupWork.ContinueWith(t => {
                Log.Debug(TAG, "Work is finished - start Activity1.");
                var intent = new Intent(Application.Context, typeof(MainActivity));
                StartActivity(intent);
                startupWork.Dispose();
            }, TaskScheduler.FromCurrentSynchronizationContext());

            startupWork.Start();
        }
    }
}

