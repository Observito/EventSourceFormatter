using System;

namespace Observito.Trace.EventSourceFormatter
{
    /// <summary>
    /// Identifies an event source.
    /// </summary>
    public readonly struct EventSourceIdentifier
    {
        /// <summary>
        /// Creates a new readonly instance.
        /// </summary>
        /// <param name="name">Event source name</param>
        /// <param name="guid">Event source guid</param>
        public EventSourceIdentifier(string name, Guid? guid = null)
        {
            Name = name;
            Guid = guid;
        }

        /// <summary>
        /// Name of the event source.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Optional GUID of the event source.
        /// </summary>
        public Guid? Guid { get; }
    }
}
