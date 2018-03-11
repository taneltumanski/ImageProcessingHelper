using Accord.Imaging;
using System;

namespace ImageProcessingEngine
{
    public class MidProcessingResult
    {
        public string EngineName { get; }
        public string ProcessorName { get; }
        public UnmanagedImage Image { get; }
        public TimeSpan Elapsed { get; }

        public MidProcessingResult(string processorName, string engineName, UnmanagedImage image, TimeSpan elapsed)
        {
            ProcessorName = processorName;
            EngineName = engineName;
            Image = image;
            Elapsed = elapsed;
        }        
    }

    public class MidProcessingResult<TOptions> : MidProcessingResult
        where TOptions : class
    {
        public TOptions Options { get; }

        public MidProcessingResult(string processorName, string engineName, UnmanagedImage image, TimeSpan elapsed, TOptions options) : base(processorName, engineName, image, elapsed)
        {
            Options = options;
        }
    }
}
