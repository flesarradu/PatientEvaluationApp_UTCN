using System;
using System.Windows.Input;
using PatientEvaluationApp_UTCN.DataModels.Recording;
using PatientEvaluationApp_UTCN.Images;
using PatientEvaluationApp_UTCN.Lib;
using Plugin.AudioRecorder;
using Prism.Commands;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PatientEvaluationApp_UTCN.ViewModels.Recording
{
    public interface IRecordingViewModel : IViewModel<IRecordingDataModel>
    {
        #region Properties
        ICommand RecordTrigger { get; }
        ICommand RecordSave { get; }
        #endregion
    }

    public class RecordingViewModel : ViewModel<IRecordingDataModel>, IRecordingViewModel
    {
        #region Fields
        private readonly AudioRecorderService recorder = new AudioRecorderService {StopRecordingOnSilence = true, AudioSilenceTimeout = new TimeSpan(0, 0, 3)};
        #endregion

        #region Constructors
        public RecordingViewModel(IRecordingDataModel model, IAppSettings appSettings, ILocalStorageService storageService) : base(model, appSettings, storageService)
        {
            this.RecordTrigger = new DelegateCommand(this.OnRecordingButtonClicked);
            this.RecordSave = new DelegateCommand(this.OnSaveButtonClicked);
        }
        #endregion

        #region Properties
        public ICommand RecordTrigger { get; }
        public ICommand RecordSave { get; }
        #endregion

        #region Events
        private async void OnSaveButtonClicked()
        {
            await Share.RequestAsync(new ShareFileRequest()
            {
                File = new ShareFile(this.recorder.GetAudioFilePath()),
                Title = "Share audio"
            });
        }
        private async void OnRecordingButtonClicked()
        {
            if (!this.DataModel.IsRecording.Value)
            {
                this.DataModel.RecordingMessage.Value = "$Recording... Press to stop recording";
                this.DataModel.IsRecording.Value = true;
                this.DataModel.ButtonImage.Value = ImageResourceExtension.RecordOnImage;
                this.DataModel.RecordingSeconds.Value = 0;
                this.DataModel.RecordingMinutes.Value = 0;
                Device.StartTimer(TimeSpan.FromSeconds(1), () =>
                {
                    this.DataModel.RecordingSeconds.Value++;
                    if (this.DataModel.RecordingSeconds.Value == 60)
                    {
                        this.DataModel.RecordingMinutes.Value++;
                        this.DataModel.RecordingSeconds.Value = 0;
                    }

                    return this.DataModel.IsRecording.Value;
                });
                if (!this.recorder.IsRecording) await this.recorder.StartRecording();
            }
            else
            {
                if (this.recorder.IsRecording) await this.recorder.StopRecording();
                this.DataModel.RecordingMessage.Value = "Press to start recording";
                this.DataModel.ButtonImage.Value = ImageResourceExtension.RecordOffImage;
                this.DataModel.IsRecording.Value = false;
                var player = new AudioPlayer();
                var filePath = this.recorder.GetAudioFilePath();
                player.Play(filePath);
            }
        }
        #endregion
    }
}