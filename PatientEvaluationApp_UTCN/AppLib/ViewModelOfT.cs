using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using PatientEvaluationApp_UTCN.Annotations;
using Xamarin.Forms;

namespace PatientEvaluationApp_UTCN.Lib
{
    public interface IViewModel<TModel> : IViewModel, INotifyPropertyChanged where TModel : class, IDataModel
    {
        #region Properties
        TModel DataModel { get; }
        #endregion

        #region Public Methods
        IViewModel<TModel> SetView(IView view);
        #endregion
    }
    public abstract class ViewModel<TModel> : ViewModel, IViewModel<TModel> where TModel : class, IDataModel
    {
        #region Delegates/Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Constructors
        protected ViewModel(TModel model, IAppSettings appSettings, ILocalStorageService storageService)
        {
            this.DataModel = model;

            this.AppSettings = appSettings;
            this.StorageService = storageService;
        }
        #endregion

        #region Properties
        public TModel DataModel { get; }

        protected IAppSettings AppSettings { get; }

        protected ILocalStorageService StorageService { get; }

        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
        protected IView View { get; private set; }
        #endregion

        #region Public Methods
        public IViewModel<TModel> SetView(IView view)
        {
            this.View = view;

            if (!(view is ContentPage contentPage)) return this;
            contentPage.Appearing += (s, e) => this.OnViewAppearing();
            contentPage.Disappearing += (s, e) => this.OnViewDisappearing();

            return this;
        }

        protected virtual void OnViewAppearing()
        {
        }

        protected virtual void OnViewDisappearing()
        {
        }
        #endregion

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] String propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
