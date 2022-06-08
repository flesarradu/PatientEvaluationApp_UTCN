using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PatientEvaluationApp_UTCN.Views;
using PatientEvaluationApp_UTCN.Views.Recording;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PatientEvaluationApp_UTCN
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(LoginView), typeof(LoginView));
        }

    }
}