using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using Accord.Imaging;
using Accord.Imaging.Filters;

namespace ImageProcessingEngine
{
    public abstract class ImageProcessorEngine<T> : ObservableBase<MidProcessingResult>, IImageProcessorEngine<T>
    {
        private Dictionary<Guid, IObserver<MidProcessingResult>> Subscribers = new Dictionary<Guid, IObserver<MidProcessingResult>>();

        protected abstract T GetResult(UnmanagedImage processedImage);
        protected abstract IEnumerable<IImageProcessor> GetProcessors(); 

        public T ProcessImage(Bitmap image)
        {
            using (var unmanagedImage = UnmanagedImage.FromManagedImage(image))
            {
                return ProcessImage(unmanagedImage);
            }
        }

        public T ProcessImage(BitmapData image)
        {
            using (var unmanagedImage = UnmanagedImage.FromManagedImage(image))
            {
                return ProcessImage(unmanagedImage);
            }
        }

        public T ProcessImage(UnmanagedImage image)
        {
            var processors = GetProcessors();
            var processedImage = image;
            var isFirstIteration = true;

            OnImage(image, null, TimeSpan.Zero, 0);

            var sw = Stopwatch.StartNew();

            var processorIndex = 0;

            foreach (var processor in processors)
            {
                sw.Restart();

                var newImage = processor.ProcessImage(processedImage);

                if (!isFirstIteration)
                {
                    processedImage.Dispose();
                }

                isFirstIteration = false;

                sw.Stop();

                OnImage(newImage, processor, sw.Elapsed, processorIndex);

                processedImage = newImage;
                processorIndex++;
            }

            var result = GetResult(processedImage);

            processedImage.Dispose();

            return result;
        }

        private void OnImage(UnmanagedImage processedImage, IImageProcessor processor, TimeSpan elapsed, int index)
        {
            var name = GetName(processor, index);

            var result = new MidProcessingResult(name, this.GetType().Name, processedImage, elapsed);

            foreach (var subscriber in Subscribers)
            {
                subscriber.Value.OnNext(result);
            }
        }

        private string GetName(IImageProcessor processor, int index)
        {
            if (processor == null)
            {
                return "Original";
            }

            return $"{processor.GetType().Name}[{index}]";
        }

        protected override IDisposable SubscribeCore(IObserver<MidProcessingResult> observer)
        {
            var id = Guid.NewGuid();

            Subscribers.Add(id, observer);

            return Disposable.Create(() =>
            {
                Subscribers.Remove(id);
            });
        }
    }

    public abstract class ImageProcessorEngineWithOptions<T, TOptions> : ImageProcessorEngine<T>, IImageProcessorEngineWithOptions<T, TOptions>
        where TOptions : class
    {
        public TOptions Options { get; }

        public ImageProcessorEngineWithOptions(TOptions options)
        {
            this.Options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public IDisposable Subscribe(IObserver<MidProcessingResult<TOptions>> observer)
        {
            var wrapperObserver = Observer.Create<MidProcessingResult>(next =>
            {
                observer.OnNext(new MidProcessingResult<TOptions>(next.ProcessorName, next.EngineName, next.Image, next.Elapsed, Options));
            });

            return base.SubscribeCore(wrapperObserver);
        }
    }
}
