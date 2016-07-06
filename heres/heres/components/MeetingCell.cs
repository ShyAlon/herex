using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace heres.components
{
    public class MeetingCell : ViewCell
    {
        public MeetingCell()
        {
            var image = new Image();
            StackLayout cellWrapper = new StackLayout();
            StackLayout horizontalLayout = new StackLayout();
            Label left = new Label();
            Label right = new Label();

            //set bindings
            left.SetBinding(Label.TextProperty, "Title");
            right.SetBinding(Label.TextProperty, "StartString");
            image.SetBinding(Image.SourceProperty, "image");

            //Set properties for desired design
            cellWrapper.BackgroundColor = Color.FromHex("#eee");
            horizontalLayout.Orientation = StackOrientation.Horizontal;
            right.HorizontalOptions = LayoutOptions.EndAndExpand;
            left.TextColor = Color.FromHex("#f35e20");
            right.TextColor = Color.FromHex("503026");

            //add views to the view hierarchy
            horizontalLayout.Children.Add(image);
            horizontalLayout.Children.Add(left);
            horizontalLayout.Children.Add(right);
            cellWrapper.Children.Add(horizontalLayout);
            View = cellWrapper;
        }

        //async protected override void OnTapped()
        //{
        //    base.OnTapped();
        //    await View.Navigation.PushAsync(new heres.pages.EventPage());
        //}
    }
}
