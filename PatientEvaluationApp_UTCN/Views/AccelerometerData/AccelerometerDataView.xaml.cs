using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PatientEvaluationApp_UTCN.Views.AccelerometerData
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AccelerometerDataView : ContentPage
    {
        private const SensorSpeed Speed = SensorSpeed.Fastest;

        public List<String> AccelerometerRecordings = new List<String>();

        public AccelerometerDataView()
        {
            InitializeComponent();
            Accelerometer.ReadingChanged += Accelerometer_ReadingChanged;

        }

        void Accelerometer_ReadingChanged(Object sender, AccelerometerChangedEventArgs e)
        {
            var data = e.Reading;
            this.AccelerometerRecordings.Add($"Reading: X: {data.Acceleration.X}, Y: {data.Acceleration.Y}, Z: {data.Acceleration.Z}");
        }
        public void ToggleAccelerometer()
        {
            try
            {
                if (Accelerometer.IsMonitoring)
                {
                    Accelerometer.Stop();
                    this.labelState.Text = "Stopped";
                    this.startButton.Text = "Start";
                }
                else
                {
                    Accelerometer.Start(AccelerometerDataView.Speed);
                    this.labelState.Text = "Recording...";
                    this.startButton.Text = "Stop";
                }
            }
            catch (FeatureNotSupportedException)
            {
                Console.WriteLine("Eroare not supported");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("????? ACC ERROR");
            }
        }

        private void StartButton_OnClicked(Object sender, EventArgs e)
        {
            this.ToggleAccelerometer();
        }

        private async void RecordButton_OnClicked(Object sender, EventArgs e)
        {
            using (var stream = new FileStream(Path.Combine(FileSystem.CacheDirectory, "acc_data.txt"), FileMode.Create))
            {
                using (var writer = new StreamWriter(stream))
                {
                    this.AccelerometerRecordings.ForEach(x => { writer.WriteLine(x); });
                }
            }

            await Share.RequestAsync(new ShareFileRequest()
            {
                File = new ShareFile(Path.Combine(FileSystem.CacheDirectory, "acc_data.txt")),
                Title = "Share accelerometer data"
            });
        }
    }
}