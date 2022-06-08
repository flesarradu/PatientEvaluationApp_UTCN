using System;
using System.ComponentModel;
using PatientEvaluationApp_UTCN.Images;
using PatientEvaluationApp_UTCN.Lib;
using Xamarin.Forms;

namespace PatientEvaluationApp_UTCN.DataModels.Recording
{
    public interface IRecordingDataModel : IDataModel
    {
        #region Properties
        DtoProperty<String> RecordingMessage { get; }
        DtoProperty<Boolean> IsRecording { get; }
        DtoProperty<Int16> RecordingSeconds { get; }
        DtoProperty<Int16> RecordingMinutes { get; }
        DtoProperty<ImageSource> ButtonImage { get; }
        #endregion
    }

    public class RecordingDataModel : DataModel, IRecordingDataModel
    {
        #region Delegates/Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Constructors
        public RecordingDataModel(IAppSettings appSettings) : base(appSettings)
        {
            this.RecordingMessage = new DtoProperty<String>("Press to start recording");
            this.IsRecording = new DtoProperty<Boolean>(false);
            this.RecordingMinutes = new DtoProperty<Int16>(0);
            this.RecordingSeconds = new DtoProperty<Int16>(0);
            this.ButtonImage = new DtoProperty<ImageSource>(ImageResourceExtension.RecordOffImage);
        }
        #endregion

        #region Properties
        public DtoProperty<String> RecordingMessage { get; }
        public DtoProperty<Boolean> IsRecording { get; }
        public DtoProperty<Int16> RecordingSeconds { get; }
        public DtoProperty<Int16> RecordingMinutes { get; }
        public DtoProperty<ImageSource> ButtonImage { get; }
        #endregion

        #region Public Methods
        public override void RegisterValueChanges()
        {
            this.RegisterValueChanges(this, m => m.RecordingMessage);
            this.RegisterValueChanges(this, m => m.IsRecording);
            this.RegisterValueChanges(this, m => m.RecordingSeconds);
            this.RegisterValueChanges(this, m => m.RecordingMinutes);
            this.RegisterValueChanges(this, m=>m.ButtonImage);
        }
        #endregion
    }
}