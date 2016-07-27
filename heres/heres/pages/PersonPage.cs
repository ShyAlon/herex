using heres.poco;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
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

            private async void Delete(object sender, EventArgs e)
            {
                var p = (Role)BindingContext;
                var db = new Database();
                await db.DeleteItem(p, db.PrimaryEmail);
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
                CreateContent();

                RefreshList += (s, e) =>
                {
                    CreateContent();
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }


        private async void CreateContent()
        {
            try
            {
                person.Roles = new ObservableCollection<Role>(await GetSavedRoles());
                BindingContext = person;
                Title = person.Name;
                var AddRole = new Button
                {
                    Text = "Add Role"
                };
                AddRole.Clicked += async (s, e) =>
                {
                    if (!string.IsNullOrWhiteSpace(newRole.Text))
                    {
                        var r = new Role
                        {
                            Name = newRole.Text,
                            ParentID = person.ID,
                            Importance = 1
                        };
                        var db = new Database();
                        await db.SaveItem(r, db.PrimaryEmail);
                        person.Roles.Add(r);
                        newRole.Text = string.Empty;
                        PersonPage.RefreshList(s, e);
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
                        }
                    }
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            
        }

        public bool HasRole() => !string.IsNullOrWhiteSpace(newRole.Text);

        private async Task<IEnumerable<Role>> GetSavedRoles()
        {
            try
            {
                var db = new Database();
                var addresses = db.GetEmailAddresses();
                var result = new List<Role>();
                foreach (var address in addresses)
                {
                    var roles = await db.GetItems<Role>(person.ID, address);
                    if (roles != null && roles.items != null)
                    {
                        result.AddRange(roles.items);
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
    }
}
