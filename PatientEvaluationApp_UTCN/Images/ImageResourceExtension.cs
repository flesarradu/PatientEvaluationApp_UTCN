using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PatientEvaluationApp_UTCN.Images
{
    [ContentProperty(nameof(ImageResourceExtension.Source))]
    public class ImageResourceExtension : IMarkupExtension
    {
        public String Source { get; set; }
        public static ImageSource RecordOffImage { get; set; }
        public static ImageSource RecordOnImage { get; set; }

        public Object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Source == null) return null;

            var assembly = typeof(ImageResourceExtension).Assembly;
            var imageSource = ImageSource.FromResource(Source, assembly);

            return imageSource;
        }
        
    }
}
