using Observito.Trace.EventSourceFormatter;
using System;
using System.Diagnostics.Tracing;
using System.Threading;
using System.Threading.Tasks;

namespace Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var cts = new CancellationTokenSource();

            // Generate in-process events using the sample-provided event source (Observito-Trace-Echo)
            _ = Task.Run(async () =>
            {
                var c = 0;
                while (!cts.IsCancellationRequested)
                {
                    await Task.Delay(TimeSpan.FromSeconds(1), cts.Token);
                    if (c % 4 == 0)
                        EchoEventSource.Log.EchoMore("ping", DateTime.Now, c);
                    else
                        EchoEventSource.Log.Echo("ping");
                    c++;
                }
            });

            using var source = new Source();

            source.EventWritten += Source_EventWritten;

            source.EnableEvents(EchoEventSource.Log, EventLevel.Informational);

            Console.WriteLine("Press enter to stop sample...");
            Console.ReadLine();

            cts.Cancel();
        }

        private static void Source_EventWritten(object sender, EventWrittenEventArgs e)
        {
            //var fmt = EventSourceFormatter.FormatMessage(e, p =>

            var fmt = EventSourceFormatter.Format(e, includePayload: true, p =>
            {
                if (p.Name == "count")
                {
                    var count = Convert.ToInt32(p.Value);
                    return $"{count} => {-count}";
                }
                return p.Value;
            });

            Console.WriteLine(fmt);
        }

        public class Source : EventListener
        {
        }
    }
}
