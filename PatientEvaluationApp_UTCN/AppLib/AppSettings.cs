using System;
using System.Collections.Generic;
using System.Text;

namespace PatientEvaluationApp_UTCN.Lib
{
    public interface IAppSettings
    {
        String Client { get; set; }
    }

    public class AppSettings : IAppSettings
    {
        public AppSettings()
        {
            Client = "Unknown";
        }
        public String Client { get; set; }
    }
}
