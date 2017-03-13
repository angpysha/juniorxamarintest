using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestProjLib;
using Xamarin.Forms;

namespace TestProj2
{
    public partial class Page2 : ContentPage
    {
        private BlankInfo info;
        public Page2()
        {
            InitializeComponent();
            BindingContext = info;
        }

        public Page2(BlankInfo info)
        {
            InitializeComponent();
            this.info = info;
            BindingContext = this.info;
        }

        private async void Back_OnClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
