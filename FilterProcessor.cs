using System;
using System.Text;
using System.Threading.Tasks;
using Accord.Imaging;
using Accord.Imaging.Filters;

namespace ImageProcessingEngine
{
    public class FilterProcessor : IImageProcessor
    {
        private readonly IFilter Filter;
        private readonly Func<UnmanagedImage, IFilter> FilterGen;

        public FilterProcessor(Func<UnmanagedImage, IFilter> filterGen)
        {
            this.FilterGen = filterGen ?? throw new ArgumentNullException(nameof(filterGen));
        }

        public FilterProcessor(IFilter filter)
        {
            this.Filter = filter ?? throw new ArgumentNullException(nameof(filter));
        }

        public UnmanagedImage ProcessImage(UnmanagedImage image)
        {
            var filter = Filter ?? FilterGen(image);

            return filter.Apply(image);
        }
    }
}
