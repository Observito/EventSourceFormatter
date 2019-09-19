﻿using System;
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
        /// <returns>Payload result</returns>
        public static Payload GetPayloadAt(this EventWrittenEventArgs @event, int index)
        {
            if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));

            var ctx = new Payload();

            var source = new EventSourceIdentifier();
            source.Name = @event.EventSource.Name;
            source.Guid = @event.EventSource.Guid;

            ctx.Source = source;
            ctx.EventName = @event.EventName;
            //result.Index = payloadIndex
            ctx.Name = @event.PayloadNames[index];
            ctx.Value = @event.Payload[index];

            return ctx;
        }

        /// <summary>
        /// Enumerates the payload as a sequence of tuples with the payload index, name and value.
        /// </summary>
        /// <param name="event">Event to enumerate from</param>
        /// <returns>Sequence of payload values</returns>
        public static IEnumerable<(int index, string name, object value)> EnumeratePayload(this EventWrittenEventArgs @event)
        {
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
            var index = 0;
            foreach (var name in @event.PayloadNames)
            {
                var payload = @event.GetPayloadAt(index);

                var value = selector(payload);

                yield return (index, name, value);

                index++;
            }
        }
    }
}