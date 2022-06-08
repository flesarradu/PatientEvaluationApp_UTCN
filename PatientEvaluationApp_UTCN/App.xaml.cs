using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using PatientEvaluationApp_UTCN.Lib;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PatientEvaluationApp_UTCN.Images;
using PatientEvaluationApp_UTCN.Views;
using Prism.Events;

namespace PatientEvaluationApp_UTCN
{
    public partial class App : Application
    {
        private IAppSettings appSettings;
        public static Boolean LoggedIn = false;
        public App()
        {
            InitializeComponent();

            var configuration = new ConfigurationBuilder()
                                .AddJsonFile(new EmbeddedFileProvider(typeof(App).Assembly, typeof(App).Namespace), @"Config\appsettings.json", true, false)
                                .Build();

            //this.appSettings = configuration.Get<AppSettings>();
            this.appSettings = new AppSettings{Client = "Radoo"};

            ServiceProviderFactory.Create()
                                  .AddAppSettings(this.appSettings)
                                  .AddAppMain()
                                  .AddCommonLibrary();
            ServiceProviderFactory.BuildServiceProvider();


            ImageResourceExtension.RecordOffImage = ImageSource.FromFile("recOff.png");
            ImageResourceExtension.RecordOnImage = ImageSource.FromFile("recOn.png");

            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
