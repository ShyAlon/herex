using heres.poco;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Threading;
using heres.components;
using System.Reflection;
using System.Collections.ObjectModel;

namespace heres.pages
{
    public class PersonPage : ContentPage
    {
        private Person person;
        private Entry newRole;
        private DataTemplate dataTemplate;

        internal static event EventHandler RefreshList;

        internal class RoleCell : TextCell
        {
            public RoleCell()
            {

            }

            protected override void OnBindingContextChanged()
            {
                base.OnBindingContextChanged();
                var p = (Role)BindingContext;
                var deleteAction = new MenuItem { Text = "Delete", IsDestructive = true }; // red background
                deleteAction.SetBinding(MenuItem.CommandParameterProperty, new Binding("."));
                deleteAction.Clicked += Delete;
                ContextActions.Add(deleteAction);
            }

            private void Delete(object sender, EventArgs e)
            {
                var p = (Role)BindingContext;
                var db = new Database();
                db.DeleteItem(p);
                PersonPage.RefreshList(sender, e);
            }
        }


        public PersonPage(Person _person)
        {
            dataTemplate = new DataTemplate(typeof(RoleCell));
            dataTemplate.SetBinding(RoleCell.TextProperty, "Name");
            try
            {
                person = _person;

                ReadRoles();
                CreateContent();

                RefreshList += (s, e) =>
                {
                    ReadRoles();
                    CreateContent();
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void ReadRoles()
        {
            var additional = GetSavedRoles();
            person.Roles = new ObservableCollection<Role>(additional);
        }

        private void CreateContent()
        {
            BindingContext = person;
            Title = person.Name;
            var AddRole = new Button
            {
                Text = "Add Role"
            };
            AddRole.Clicked += (s, e) =>
            {
                if (!string.IsNullOrWhiteSpace(newRole.Text))
                {
                    var r = new Role
                    {
                        Name = newRole.Text,
                        ParentID = person.ID
                    };
                    var db = new Database();
                    db.SaveItem(r);
                    person.Roles.Add(r);
                }
            };
            newRole = new Entry() { };
            AddRole.SetBinding(Button.IsEnabledProperty, new Binding(@"HasRole"));
            var listView = new ListView
            {
                Header = "Roles",
                ItemsSource = person.Roles,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                ItemTemplate = dataTemplate,
                BackgroundColor = Color.Pink
            };

            // listView.ItemTapped += ParticipantTapped;

            Content = new StackLayout
            {
                Children =
                    {
                        new StackLayout { Orientation = StackOrientation.Vertical,
                            Children =
                            {
                                new Label {Text = "Role:"},
                                newRole,
                            }
                        },
                        AddRole,
                        new ScrollView
                        {
                            VerticalOptions = LayoutOptions.FillAndExpand,
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                            Content = listView,
                            BackgroundColor = Color.Yellow
                        }
                    }
            };
        }

        public bool HasRole() => !string.IsNullOrWhiteSpace(newRole.Text);

        private IEnumerable<Role> GetSavedRoles()
        {
            var db = new Database();
            var roles = db.GetItems<Role>(person.ID);
            return roles;
        }
    }
}
