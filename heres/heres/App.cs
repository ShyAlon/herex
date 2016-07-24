using heres.pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace heres
{
	public class App : Application
	{
		public App ()
		{
            MainPage = new NavigationPage(new EventsPage() { Title = "Coming Events" });
        }

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
            if(Resumed != null)
            {
                Resumed(this, new EventArgs());
            }
		}

        public static event EventHandler Resumed;
	}
}
