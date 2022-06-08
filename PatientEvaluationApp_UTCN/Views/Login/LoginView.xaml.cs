using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PatientEvaluationApp_UTCN.Views.Recording;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PatientEvaluationApp_UTCN.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginView : ContentPage
    {
        public LoginView()
        {
            InitializeComponent();
        }

        private async void Button_OnClicked(Object sender, EventArgs e)
        {
            if (this.Email.Text == "flesar" && this.Password.Text == "passwd")
            {
                await Shell.Current.GoToAsync($"//{nameof(RecordingView)}");
            }
        }
    }
}