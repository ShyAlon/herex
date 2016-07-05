using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using here.core;
using Android.Provider;
using Android.Gms.Common;
using Android.Util;

namespace here.droid
{
    [Activity(Label = "Here", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity//ListActivity
    {
        TextView msgText;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.console);

            msgText = FindViewById<TextView>(Resource.Id.msgText);

            if (IsPlayServicesAvailable())
            {
                var intent = new Intent(this, typeof(RegistrationIntentService));
                StartService(intent);
            }


            //// Set our view from the "main" layout resource
            //SetContentView(Resource.Layout.CalendarList);

            //// List Calendars
            //var calendarsUri = CalendarContract.Calendars.ContentUri;

            //string[] calendarsProjection = {
            //   CalendarContract.Calendars.InterfaceConsts.Id,
            //   CalendarContract.Calendars.InterfaceConsts.CalendarDisplayName,
            //   CalendarContract.Calendars.InterfaceConsts.AccountName
            //};

            //var cursor = ManagedQuery(calendarsUri, calendarsProjection, null, null, null);

            //string[] sourceColumns = {CalendarContract.Calendars.InterfaceConsts.CalendarDisplayName,
            //    CalendarContract.Calendars.InterfaceConsts.AccountName};

            //int[] targetResources = { Resource.Id.calDisplayName, Resource.Id.calAccountName };

            //SimpleCursorAdapter adapter = new SimpleCursorAdapter(this, Resource.Layout.CalListItem,
            //    cursor, sourceColumns, targetResources);

            //ListAdapter = adapter;

            //ListView.ItemClick += (sender, e) => {
            //    int i = (e as Android.Widget.AdapterView.ItemClickEventArgs).Position;

            //    cursor.MoveToPosition(i);
            //    int calId = cursor.GetInt(cursor.GetColumnIndex(calendarsProjection[0]));

            //    var showEvents = new Intent(this, typeof(EventListActivity));
            //    showEvents.PutExtra("calId", calId);
            //    StartActivity(showEvents);
            //};
        }

        public bool IsPlayServicesAvailable()
        {
            int resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
            if (resultCode != ConnectionResult.Success)
            {
                if (GoogleApiAvailability.Instance.IsUserResolvableError(resultCode))
                    msgText.Text = GoogleApiAvailability.Instance.GetErrorString(resultCode);
                else
                {
                    msgText.Text = "Sorry, this device is not supported";
                    Finish();
                }
                return false;
            }
            else
            {
                msgText.Text = "Google Play Services is available.";
                return true;
            }
        }
    }
}

