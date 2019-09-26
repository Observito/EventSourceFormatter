using Observito.Trace.EventSourceFormatter;
using System;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sample
{
    class Program
    {
        private static (PayloadData, PayloadAttribute)[] _meta;

        static async Task Main(string[] args)
        {
            Console.WriteLine("Event/payload metadata:");
            _meta = EchoEventSource.Log.GetPayloadMetadata().ToArray();
            foreach (var pm in _meta)
            {
                var pd = pm.Item1;
                var str = $"{pd.Source.Name}/{pd.EventName}[{pd.EventId}]/{pd.Name}[{pd.Index}] -> {pm.Item2}";
                Console.WriteLine(str);
            }
            Console.WriteLine();
            await Task.Delay(TimeSpan.FromSeconds(1.5));

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
                        EchoEventSource.Log.Echo("ping", "do not log this");
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

                var match = _meta.FirstOrDefault(ev => ev.Item1.EventId == e.EventId && ev.Item1.Name == p.Name && ev.Item2 != null);
                if (match != default)
                {
                    if (match.Item2.Types.Any(value => value == PayloadType.Secret))
                        return $"Omitting sensitive payload value";
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
