using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;

namespace Observito.Trace.EventSourceFormatter
{
    /// <summary>
    /// <see cref="EventWrittenEventArgs"/> extensions.
    /// </summary>
    public static class EventWrittenEventArgsExtensions
    {
        /// <summary>
        /// Gets the payload the given index.
        /// </summary>
        /// <param name="event">Event data</param>
        /// <param name="index">Payload index</param>
        /// <returns>Payload result</returns>
        public static Payload GetIdentfier(this EventWrittenEventArgs @event, int index)
        {
            if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));

            var ctx = new Payload();
            ctx.Source = @event.EventSource.GetIdentfier();
            ctx.EventName = @event.EventName;
            //result.Index = payloadIndex
            ctx.Name = @event.PayloadNames[index];
            ctx.Value = @event.Payload[index];

            return ctx;
        }

        /// <summary>
        /// Gets the payload the given index.
        /// </summary>
        /// <param name="event">Event data</param>
        /// <param name="index">Payload index</param>
        /// <param name="payload">Payload result if found</param>
        /// <returns>True if found, false otherwise</returns>
        public static bool TryGetPayloadAt(this EventWrittenEventArgs @event, int index, out Payload payload)
        {
            if (@event.Payload == null || index < 0 || index >= @event.PayloadNames.Count)
            {
                payload = default;
                return false;
            }

            var ctx = new Payload();

            var source = new EventSourceIdentifier(@event.EventSource.Name, @event.EventSource.Guid);

            ctx.Source = source;
            ctx.EventName = @event.EventName;
            //result.Index = payloadIndex
            ctx.Name = @event.PayloadNames[index];
            ctx.Value = @event.Payload[index];

            payload = ctx;

            return true;
        }

        /// <summary>
        /// Enumerates the payload as a sequence of tuples with the payload index, name and value.
        /// </summary>
        /// <param name="event">Event to enumerate from</param>
        /// <returns>Sequence of payload values</returns>
        public static IEnumerable<(int index, string name, object value)> EnumeratePayload(this EventWrittenEventArgs @event)
        {
            if (@event.PayloadNames == null)
                yield break;

            var index = 0;
            foreach (var name in @event.PayloadNames)
            {
                var value = @event.Payload[index];

                yield return (index, name, value);

                index++;
            }
        }

        /// <summary>
        /// Enumerates the payload as a sequence of tuples with the payload index, name and value.
        /// </summary>
        /// <typeparam name="TResult">Transformed result payload type</typeparam>
        /// <param name="event">Event to enumerate from</param>
        /// <param name="selector">Payload transformer</param>
        /// <returns>Sequence of payload values</returns>
        public static IEnumerable<(int index, string name, TResult value)> EnumeratePayload<TResult>(this EventWrittenEventArgs @event, PayloadSelector<TResult> selector)
        {
            if (selector is null) throw new ArgumentNullException(nameof(selector));

            if (@event.PayloadNames == null)
                yield break;

            var index = 0;
            foreach (var name in @event.PayloadNames)
            {
                var value = default(TResult);

                if (@event.TryGetPayloadAt(index, out var payloadInfo))
                    value = selector(payloadInfo);

                yield return (index, name, value);

                index++;
            }
        }
    }
}
