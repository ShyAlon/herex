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
            StackLayout cellWrapper = new StackLayout() { Padding = new Thickness(20, 0, 10, 0) };
            StackLayout horizontalLayout = new StackLayout();
            Label left = new Label();
            Label right = new Label();

            //set bindings
            left.SetBinding(Label.TextProperty, "Title");
            right.SetBinding(Label.TextProperty, "StartString");

            //Set properties for desired design
            horizontalLayout.Orientation = StackOrientation.Horizontal;
            right.HorizontalOptions = LayoutOptions.EndAndExpand;

            //add views to the view hierarchy
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
