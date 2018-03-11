using System;
using System.Drawing;
using System.Drawing.Imaging;
using Accord.Imaging;

namespace ImageProcessingEngine
{
    public interface IImageProcessorEngine<T> : IObservable<MidProcessingResult>
    {
        T ProcessImage(Bitmap image);
        T ProcessImage(BitmapData image);
        T ProcessImage(UnmanagedImage image);
    }

    public interface IImageProcessorEngineWithOptions<T, TOptions> : IImageProcessorEngine<T>, IObservable<MidProcessingResult<TOptions>>
        where TOptions : class
    {
        TOptions Options { get; }
    }
}
