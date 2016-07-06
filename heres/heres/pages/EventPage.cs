using heres.poco;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Threading;
using heres.components;

namespace heres.pages
{
    public class EventPage : ContentPage
    {
        private ListView list;

        public EventPage()
        {
            var layout = new StackLayout()
            {
                Children =
                {
                    new Label
                    {
                        Text = "Event Page"
                    }
                }, VerticalOptions = LayoutOptions.Center

            };
            Content = new ScrollView
            {
                Content = layout
            };
        }
    }
}
