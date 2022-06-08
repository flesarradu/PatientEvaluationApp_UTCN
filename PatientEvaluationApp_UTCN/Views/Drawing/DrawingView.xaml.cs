using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using TouchTracking;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.Xaml;

namespace PatientEvaluationApp_UTCN.Views.Drawing
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DrawingView : ContentPage
    {
        readonly Dictionary<Int64, SKPath> inProgressPaths = new Dictionary<Int64, SKPath>();
        readonly List<SKPath> completedPaths = new List<SKPath>();
        private Dictionary<DateTime, TouchTrackingPoint> poisitionDictionary = new Dictionary<DateTime, TouchTrackingPoint>();
        private SKSurface surface;

        private readonly SKPaint paint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.SkyBlue,
            StrokeWidth = 10,
            StrokeCap = SKStrokeCap.Round,
            StrokeJoin = SKStrokeJoin.Round
        };

        public DrawingView()
        {
            InitializeComponent();

            
        }

        private async void OnCanvasViewPaintSurface(Object sender, SKPaintSurfaceEventArgs args)
        {

            var canvas = args.Surface.Canvas;
            this.surface = args.Surface;
            canvas.Clear();

            foreach (var path in this.completedPaths)
            {
                canvas.DrawPath(path, paint);
            }

            foreach (var path in this.inProgressPaths.Values)
            {
                canvas.DrawPath(path, paint);
            }

        }

        private void OnTouchEffectAction(Object sender, TouchActionEventArgs args)
        {
            this.poisitionDictionary.Add(DateTime.Now,args.Location);
            switch (args.Type)
            {
                case TouchActionType.Pressed:
                    if (!this.inProgressPaths.ContainsKey(args.Id))
                    {
                        var path = new SKPath();
                        path.MoveTo(this.OnConvertToPixel(args.Location));
                        this.inProgressPaths.Add(args.Id, path);
                        canvasView.InvalidateSurface();
                    }

                    break;

                case TouchActionType.Moved:
                    if (this.inProgressPaths.ContainsKey(args.Id))
                    {
                        var path = this.inProgressPaths[args.Id];
                        path.LineTo(this.OnConvertToPixel(args.Location));
                        canvasView.InvalidateSurface();
                    }

                    break;

                case TouchActionType.Released:
                    if (this.inProgressPaths.ContainsKey(args.Id))
                    {
                        this.completedPaths.Add(this.inProgressPaths[args.Id]);
                        this.inProgressPaths.Remove(args.Id);
                        canvasView.InvalidateSurface();
                    }

                    break;

                case TouchActionType.Cancelled:
                    if (this.inProgressPaths.ContainsKey(args.Id))
                    {
                        this.inProgressPaths.Remove(args.Id);
                        canvasView.InvalidateSurface();
                    }

                    break;
            }
        }

        SKPoint OnConvertToPixel(TouchTrackingPoint pt)
        {
            return new SKPoint((float) (canvasView.CanvasSize.Width * pt.X / canvasView.Width),
                (float) (canvasView.CanvasSize.Height * pt.Y / canvasView.Height));
        }

        private void Button_OnClicked(Object sender, EventArgs e)
        {
            inProgressPaths.Clear();
            completedPaths.Clear();
            canvasView.InvalidateSurface();
        }

        private async void Button_OnClicked2(Object sender, EventArgs e)
        {
            var skSurface = SKSurface.Create((int) canvasView.CanvasSize.Width, (int) canvasView.CanvasSize.Height, SKImageInfo.PlatformColorType, SKAlphaType.Premul);

            var canvas = skSurface.Canvas;

            foreach (var path in this.completedPaths)
            {
                canvas.DrawPath(path, paint);
            }

            foreach (var path in this.inProgressPaths.Values)
            {
                canvas.DrawPath(path, paint);
            }

            var skImage = skSurface.Snapshot();
            var skData = skImage.Encode(SKEncodedImageFormat.Png, 100);

            using (var stream = new FileStream(Path.Combine(FileSystem.CacheDirectory, "canvas.png"), FileMode.OpenOrCreate))
            {
                skData.SaveTo(stream);
            }
            using (var stream = new FileStream(Path.Combine(FileSystem.CacheDirectory, "data.txt"), FileMode.Create))
            {
                using (var writer = new StreamWriter(stream))
                {
                    this.poisitionDictionary.ForEach(x => { writer.WriteLine($"{x.Value.X} {x.Value.Y} {x.Key}"); });
                }
            }

            await Share.RequestAsync(new ShareMultipleFilesRequest()
            {
                Files = new List<ShareFile> {new ShareFile(Path.Combine(FileSystem.CacheDirectory, "canvas.png")), new ShareFile(Path.Combine(FileSystem.CacheDirectory, "data.txt")) },
                Title = "Share data"
            });
        }
    }
}