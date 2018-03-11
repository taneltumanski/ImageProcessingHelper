using Accord.Imaging;

namespace ImageProcessingEngine
{
    public interface IImageProcessor
    {
        UnmanagedImage ProcessImage(UnmanagedImage image);
    }
}
