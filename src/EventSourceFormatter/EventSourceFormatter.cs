using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Globalization;
using System.Text;

namespace Observito.Trace.EventSourceFormatter
{
    /// <summary>
    /// Formatting of in-process <see cref="EventSource"/> events, i.e. <see cref="EventWrittenEventArgs"/>.
    /// </summary>
    public class EventSourceFormatter
    {
        /// <summary>
        /// Format message.
        /// TODO culture
        /// </summary>
        /// <param name="event">Event data</param>
        /// <param name="selector">Optional payload transformer</param>
        /// <returns>Formatted message</returns>
        public static string FormatMessage(EventWrittenEventArgs @event, PayloadSelector<object> selector = null)
        {
            var raw = @event.Message;

            if (string.IsNullOrEmpty(raw) ||
                !raw.Contains("{", StringComparison.OrdinalIgnoreCase) ||
                !raw.Contains("}", StringComparison.OrdinalIgnoreCase))
            {
                return raw;
            }

            var sequentialPayloadValues = new List<object>();
            HandlePayloadUsages(@event, payloadIndex =>
            {
                try
                {
                    var value = default(object);

                    if (@event.TryGetPayloadData(payloadIndex, out var data))
                    {
                        if (selector == null)
                            value = data.Value;
                        else
                            value = selector(data);
                    }

                    sequentialPayloadValues.Add(value);
                }
                catch (Exception ex)
                {
                    sequentialPayloadValues.Add($"PAYLOAD_ERROR({ex.Message}");
                }
            });

            var args = sequentialPayloadValues.ToArray();

            return string.Format(raw, args);
        }

        /// <summary>
        /// Format message header.
        /// TODO culture
        /// </summary>
        /// <param name="event">Event data</param>
        /// <param name="formatMessage">If true formats message part to include payload values</param>
        /// <param name="selector">Optional payload transformer</param>
        /// <returns>Formatted header</returns>
        public static string FormatHeader(EventWrittenEventArgs @event, bool formatMessage, PayloadSelector<object> selector = null)
        {
            var msg = @event.Message;

            if (formatMessage)
            {
                try
                {
                    msg = FormatMessage(@event, selector);
                }
                catch (Exception ex)
                {
                    msg = $"(Internal error formatting event message: {ex.Message})";
                }
            }

            return @event.Opcode == EventOpcode.Info
                ? $"[{@event.TimeStamp:yyyy-MM-dd HH:mm:ss.ffff}] {@event.EventSource.Name}/{@event.EventName}: {msg}"
                : $"[{@event.TimeStamp:yyyy-MM-dd HH:mm:ss.ffff}] {@event.EventSource.Name}/{@event.EventName}/{@event.Opcode}: {msg}";
        }

        /// <summary>
        /// Formats event payload data to a string.
        /// TODO culture
        /// </summary>
        /// <param name="event">Event data</param>
        /// <param name="selector">Optional payload transformer</param>
        /// <param name="prefix">Prefix to use before payload name</param>
        /// <returns>Formatted stringæ</returns>
        public static string FormatPayload(EventWrittenEventArgs @event, PayloadSelector<object> selector = null, string prefix = "@")
        {
            var sb = new StringBuilder();

            var seq = selector == null ? @event.EnumeratePayload() : @event.EnumeratePayload(selector);

            foreach (var payload in seq)
            {
                var name = payload.name;
                var value = payload.value.ToString();

                sb.AppendLine($"{prefix ?? ""}{name}={value}");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Formats the event data to a string.
        /// TODO culture
        /// </summary>
        /// <param name="event">Event data</param>
        /// <param name="includePayload">Include payload</param>
        /// <param name="selector">Payload transformer</param>
        /// <returns>Formatted event</returns>
        public static string Format(EventWrittenEventArgs @event, bool includePayload, PayloadSelector<object> selector = null)
        {
            // Create log message builder
            var sbFmt = new StringBuilder();

            // Append log header line

            var header = FormatHeader(@event, true, selector);

            sbFmt.AppendLine(header);
            sbFmt.AppendLine();

            // Data dictionary
            var map = new Dictionary<string, object>();
            map["EventSourceName"] = @event.EventSource.Name;
            map["EventSourceGuid"] = @event.EventSource.Guid;
            map["Version"] = @event.Version;

            // Add payload to data
            if (includePayload)
            {
                DumpPayload(@event, map, selector);
            }

            foreach (var kv in map)
            {
                string val = kv.Value?.ToString();

                sbFmt.AppendLine($"{kv.Key}={val}");
            }

            var fmt = sbFmt.ToString();
            return fmt;
        }

        private static void DumpPayload(EventWrittenEventArgs @event, Dictionary<string, object> map, PayloadSelector<object> selector = null, string prefix = "@")
        {
            try
            {
                var seq = selector == null ? @event.EnumeratePayload() : @event.EnumeratePayload(selector);

                foreach (var payload in seq)
                    map[$"{prefix ?? ""}{payload.name}"] = payload.value;
            }
            catch (Exception ex)
            {
                map[$"EventSourceFormatterError"] = ex.Message;
            }
        }

        #region Internal helpers
        /// <summary>
        /// Iterates through the unformatted message and captures payload indices.
        /// </summary>
        /// <param name="event">Event with raw message to process</param>
        /// <param name="handler">Payload index handler</param>
        private static void HandlePayloadUsages(EventWrittenEventArgs @event, Action<int> handler)
        {
            var raw = @event.Message;
            var cindex = 0;
            var startCurleyIndex = default(int?);
            var isNumber = true;
            foreach (var c in raw)
            {
                if (startCurleyIndex == null)
                {
                    if (c == '{')
                    {
                        startCurleyIndex = cindex;
                        isNumber = true;
                    }
                }
                else
                {
                    if (c == '}')
                    {
                        var i = startCurleyIndex.Value + 1;
                        var j = cindex;
                        if (isNumber)
                        {
                            var numSpan = raw.AsSpan().Slice(i, j - i);
                            var s = numSpan.ToString();
                            var payloadIndex = int.Parse(numSpan, NumberStyles.Integer);
                            handler(payloadIndex);
                        }
                        startCurleyIndex = null;
                        isNumber = false;
                    }
                    else if (char.IsDigit(c))
                    {
                        // ok
                    }
                    else
                    {
                        isNumber = false;
                    }
                }
                cindex++;
            }
        }
        #endregion
    }
}
