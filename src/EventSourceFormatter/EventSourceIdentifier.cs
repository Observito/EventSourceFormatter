using System;

namespace Observito.Trace.EventSourceFormatter
{
    /// <summary>
    /// Identifies an event source.
    /// </summary>
    public struct EventSourceIdentifier
    {
        /// <summary>
        /// Name of the event source.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Optional GUID of the event source.
        /// </summary>
        public Guid? Guid { get; set; }
    }
}
