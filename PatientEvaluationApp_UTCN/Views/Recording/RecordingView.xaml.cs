using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using PatientEvaluationApp_UTCN.DataModels.Recording;
using PatientEvaluationApp_UTCN.Images;
using PatientEvaluationApp_UTCN.Lib;
using PatientEvaluationApp_UTCN.ViewModels.Recording;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PatientEvaluationApp_UTCN.Views.Recording
{
    public partial class RecordingView : UtContentPage
    {
        private Boolean recording = false;
        public RecordingView()
        {
            InitializeComponent();
            this.BindingContext = ServiceProviderServiceExtensions.GetService<IRecordingViewModel>(ServiceProviderFactory.ServiceProvider).SetView(this);
        }

    }
}
