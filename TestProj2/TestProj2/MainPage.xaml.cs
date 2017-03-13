using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TestProjLib;
using Xamarin.Forms;

namespace TestProj2
{
    public partial class MainPage : ContentPage
    {
        private dynamic countries;
        protected List<Country> Countries { get; private set; }
        protected ObservableCollection<City> Cities { get; private set; } = 
            new ObservableCollection<City>();
        protected int country_id { get; private set; } = 0;
        protected int university_id { get; private set; }

        protected ObservableCollection<University> Universities { get; private set; } =
            new ObservableCollection<University>();
        protected int CityId { get; private set; }
        protected bool[] EntriesName = { false, false };
        public MainPage()
        {
            InitializeComponent();
            bool connectivity = HelpFuncs.IsInternetConnectionAvaible();
            if (!connectivity)
            {
                lab1.Text = "Отсуствует подключение";
                lab1.IsVisible = true;
            }
            LoadCountries();
            CountriesList.SelectedIndexChanged += CountriesList_SelectedIndexChanged;
            this.BindingContext = this;
            CitiesList.ItemsSource = Cities;
            UnivesitiesList.ItemsSource = Universities;
        }

        private void CountriesList_SelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = sender as Picker;
            if (picker == null)
                return;
            int selected = picker.SelectedIndex;
            var sel = picker.Items[selected];
            Country country = Countries.FindLast(x => x.caption.Contains(sel));
            this.country_id = country.id;
          //  CitiesList.ItemsSource = Cities;
            CitiesEntry.IsEnabled = true;
            //if (country_id)
            //s  CitiesEntry.IsVisible = true;
        }

        private async void LoadCountries()
        {
            HelpFuncs.TaskComletedEventHandler += HelpFuncsOnTaskComletedEventHandler;
            await HelpFuncs.GetCounties();
        }

        private void HelpFuncsOnTaskComletedEventHandler(dynamic objects, HelpFuncs.TaskType taskType)
        {
            //lab1.Text = objects.ToString();
            // if (tastType == HelpFuncs.TaskType.Country)
            switch (taskType)
            {
                case HelpFuncs.TaskType.Country:
                    {
                        this.Countries = (List<Country>)objects;
                        Countries.Sort((p1, p2) => String.Compare(p1.caption, p2.caption, StringComparison.Ordinal));

                        foreach (var country in Countries)
                        {
                            CountriesList.Items.Add(country.caption);
                        }

                    }
                    ; break;
                case HelpFuncs.TaskType.City:
                    {

                        var tmp = (List<City>)objects;
                        tmp.Sort((p1, p2) => String.Compare(p1.Title, p2.Title, StringComparison.Ordinal));
                        while (Cities.Count > 0)
                        {
                            Cities.RemoveAt(0);
                        }
                        foreach (var city in tmp)
                        {
                            Cities.Add(city);
                        }
                            CitiesList.IsVisible = Cities.Count > 0;

                        tmp.Clear();
                        //  CitiesList.IsVisible = false;

                    }
                    ; break;
                case HelpFuncs.TaskType.University:
                    {
                        var tmp = (List<University>)objects;
                        tmp.Sort((p1,p2)=>String.Compare(p1.Title,p2.Title,StringComparison.Ordinal));
                        while (Universities.Count > 0)
                        {
                            Universities.RemoveAt(0);
                        }
                        foreach (var university in tmp)
                        {
                            Universities.Add(university);
                        }
                        UnivesitiesList.IsVisible = Universities.Count > 0;

                    }
                    ;
                    break;
            }
        }

        private async void CitiesEntry_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(CitiesEntry.Text))
                return;
            await HelpFuncs.GetCities(CitiesEntry.Text, this.country_id);

        }

        private void CitiesList_OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            City city = e.Item as City;
            // CitiesList.SelectedItem = e.Item;
            CitiesEntry.Text = city?.Title;
            CitiesList.IsVisible = false;
            if (city != null) CityId = city.Id;
            //CitiesEntry.IsEnabled = true;
            UniversityEntry.IsEnabled = true;
        }

        private void CitiesEntry_OnFocused(object sender, FocusEventArgs e)
        {
            //if (String.IsNullOrWhiteSpace(CitiesEntry.Text))
            //    return;
            //CitiesList.IsVisible = true;
        }

        private void Firstname_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(Surname.Text) && String.IsNullOrWhiteSpace(Firstname.Text))
                return;
            EntriesName[0] = true;
            if (EntriesName[0] && EntriesName[1])

                CountriesList.IsEnabled = true;
        }

        private void Surname_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(Surname.Text) && String.IsNullOrWhiteSpace(Firstname.Text))
                return;
            EntriesName[1] = true;
            if (EntriesName[0] && EntriesName[1])

                CountriesList.IsEnabled = true;
        }

        private async void UniversityCitiesEntry_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(CitiesEntry.Text))
                return;
            await HelpFuncs.GetUniversities(UniversityEntry.Text, this.country_id, this.CityId);
        }

        private void UnivesitiesList_OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            University university = e.Item as University;
            UniversityEntry.Text = university?.Title;
            UnivesitiesList.IsVisible = false;
            if (university!= null) university_id = university.Id;
        }

        private async void Button1_OnClicked(object sender, EventArgs e)
        {

            BlankInfo info = new BlankInfo
            {
                FirstName = $"Имя: {Firstname.Text}",
                SurName = $"Фамилия: {Surname.Text}",
                Country = $"Страна: {CountriesList.Items[CountriesList.SelectedIndex]}",
                City = $"Город: { CitiesEntry.Text}",
                University = $"Университет: {UniversityEntry.Text}"
            };
            await Navigation.PushAsync(new Page2(info));
        }
    }
}
