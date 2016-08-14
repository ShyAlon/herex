using System;

using Android.App;
using Android.Content;
using Android.Util;
using Android.Gms.Gcm.Iid;
using Android.Gms.Gcm;

namespace heres.Droid.Utils
{
    [Service(Exported = false)]
    class RegistrationIntentService : IntentService
    {
        static object locker = new object();

        public RegistrationIntentService() : base(nameof(RegistrationIntentService)) { }

        protected override void OnHandleIntent(Intent intent)
        {
            try
            {
                Log.Info(nameof(RegistrationIntentService), "Calling InstanceID.GetToken");
                lock (locker)
                {
                    var instanceID = InstanceID.GetInstance(this);
                    var token = instanceID.GetToken(
                        "823817818555", GoogleCloudMessaging.InstanceIdScope, null);

                    Log.Info(nameof(RegistrationIntentService), "GCM Registration Token: " + token);
                    SendRegistrationToAppServer(token);
                    Subscribe(token);
                }
            }
            catch (Exception e)
            {
                Log.Debug(nameof(RegistrationIntentService), "Failed to get a registration token");
                Log.Debug(nameof(RegistrationIntentService), e.ToString());

                return;
            }
        }

        void SendRegistrationToAppServer(string token)
        {
            // Add custom implementation here as needed.
        }

        void Subscribe(string token)
        {
            var pubSub = GcmPubSub.GetInstance(this);
            pubSub.Subscribe(token, "/topics/global", null);
        }
    }
}