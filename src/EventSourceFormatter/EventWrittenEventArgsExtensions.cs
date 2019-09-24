using System;
using System.Linq;
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
        /// Tries to get the payload at the given index.
        /// </summary>
        /// <param name="event">Event to access</param>
        /// <param name="index">Payload index</param>
        /// <param name="value">Payload if found, default otherwise</param>
        /// <returns>True if found, False otherwise</returns>
        public static bool TryGetPayload(this EventWrittenEventArgs @event, int index, out object value)
        {
            var result = false;
            if (@event.Payload == null || @event.Payload.Count == 0 || index >= @event.Payload.Count)
            {
                value = default;
            }
            else
            {
                result = true;
                value = @event.Payload[index];
            }
            return result;
        }

        /// <summary>
        /// Tries to get the payload at the given index.
        /// </summary>
        /// <typeparam name="T">Result type</typeparam>
        /// <param name="event">Event to access</param>
        /// <param name="index">Payload index</param>
        /// <param name="value">Payload if found, default otherwise</param>
        /// <returns>True if found, False otherwise</returns>
        /// <exception cref="InvalidCastException">If the payload does not match the given type</exception>
        public static bool TryGetPayload<T>(this EventWrittenEventArgs @event, int index, out T value)
        {
            if (@event.TryGetPayload(index, out var temp))
            {
                value = (T)temp;
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }

        /// <summary>
        /// Tries to get the index of the payload.
        /// </summary>
        /// <param name="event">Event to access</param>
        /// <param name="name">Payload name</param>
        /// <param name="index">Payload index if found, default otherwise</param>
        /// <returns>True if found, False otherwise</returns>
        public static bool TryGetPayloadIndex(this EventWrittenEventArgs @event, string name, out int index)
        {
            var result = false;
            if (@event.Payload == null || @event.Payload.Count == 0)
            {
                index = default;
            }
            else
            {
                index = default;
                var n = @event.Payload.Count;
                for (var i = 0; i < n || result; i++)
                {
                    if (string.Equals(@event.PayloadNames[i], name, StringComparison.Ordinal))
                    {
                        index = i;
                        result = true;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Tries to get the payload.
        /// </summary>
        /// <param name="event">Event to access</param>
        /// <param name="name">Payload name</param>
        /// <param name="value">Payload value if found, default otherwise</param>
        /// <returns>True if found, False otherwise</returns>
        public static bool TryGetPayload(this EventWrittenEventArgs @event, string name, out object value)
        {
            var result = false;
            if (@event.TryGetPayloadIndex(name, out var index))
            {
                value = @event.Payload[index];
                result = true;
            }
            else
            {
                value = default;
            }
            return result;
        }

        /// <summary>
        /// Tries to get the payload.
        /// </summary>
        /// <typeparam name="T">Payload type</typeparam>
        /// <param name="event">Event to access</param>
        /// <param name="name">Payload name</param>
        /// <param name="value">Payload value if found, default otherwise</param>
        /// <returns>True if found, False otherwise</returns>
        /// <exception cref="InvalidCastException">If the payload does not match the given type</exception>
        public static bool TryGetPayload<T>(this EventWrittenEventArgs @event, string name, out T value)
        {
            if (@event.TryGetPayload(name, out var temp))
            {
                value = (T)temp;
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }

        /// <summary>
        /// Tries to get the payload the given index.
        /// </summary>
        /// <param name="event">Event data</param>
        /// <param name="index">Payload index</param>
        /// <param name="value">Payload if found, default otherwise</param>
        /// <returns>True if found, False otherwise</returns>
        public static bool TryGetPayloadData(this EventWrittenEventArgs @event, int index, out PayloadData value)
        {
            var result = false;
            if (@event.Payload == null || @event.Payload.Count == 0 || index >= @event.Payload.Count)
            {
                value = default;
            }
            else
            {
                result = true;
                value = new PayloadData
                (
                    @event.EventSource.GetIdentfier(),
                    @event.EventId,
                    @event.EventName,
                    //result.Index = payloadIndex
                    index,
                    @event.PayloadNames[index],
                    @event.Payload[index]
                );
            }
            return result;
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

                if (@event.TryGetPayloadData(index, out var data))
                    value = selector(data);

                yield return (index, name, value);

                index++;
            }
        }
    }
}
