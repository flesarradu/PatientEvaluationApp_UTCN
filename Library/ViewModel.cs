using System;
using System.Collections.Generic;
using System.Text;

namespace PatientEvaluationApp_UTCN.Lib
{
    public interface IViewModel
    {
        Boolean IsBusy { get; }
    }
    public class ViewModel : IViewModel
    {
        public ViewModel()
        {
            IsBusy=false;
        }
        public Boolean IsBusy { get; }
    }
}
