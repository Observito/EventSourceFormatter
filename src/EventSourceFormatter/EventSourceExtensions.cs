using System;
using System.Diagnostics.Tracing;

namespace Observito.Trace.EventSourceFormatter
{
    /// <summary>
    /// <see cref="EventSource"/> extensions.
    /// </summary>
    public static class EventSourceExtensions
    {
        /// <summary>
        /// Gets the event source identifier.
        /// </summary>
        /// <param name="event">Event data</param>
        /// <param name="index">Payload index</param>
        /// <returns>Payload result</returns>
        public static EventSourceIdentifier GetIdentfier(this EventSource source)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));

            var id = new EventSourceIdentifier
            {
                Name = source.Name,
                Guid = source.Guid
            };

            return id;
        }
    }
}
