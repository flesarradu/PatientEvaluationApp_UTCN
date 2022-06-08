using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace PatientEvaluationApp_UTCN.Lib
{
    public abstract class UtContentPage : ContentPage, IView
    {

        public String ViewName => this.GetType().FullName;

        public T GetDataContext<T>() where T : class
        {
            return this.BindingContext as T;
        }
    }
}
