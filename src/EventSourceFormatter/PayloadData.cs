namespace Observito.Trace.EventSourceFormatter
{
    /// <summary>
    /// Payload with contextual metadata.
    /// </summary>
    public sealed class PayloadData
    {
        /// <summary>
        /// Payload constructor.
        /// </summary>
        /// <param name="source">Event source id</param>
        /// <param name="eventId">Event id</param>
        /// <param name="eventName">Event name</param>
        /// <param name="index">Payload index</param>
        /// <param name="name">Payload name</param>
        /// <param name="value">Payload value</param>
        public PayloadData(EventSourceIdentifier source, int eventId, string eventName, int index, string name, object value)
        {
            Source = source;
            EventId = eventId;
            EventName = eventName;
            Index = index;
            Name = name;
            Value = value;
        }
        
        /// <summary>
        /// Event source identifier.
        /// </summary>
        public EventSourceIdentifier Source { get; }

        /// <summary>
        /// Event ID.
        /// </summary>
        public int EventId { get; }

        /// <summary>
        /// Event name.
        /// </summary>
        public string EventName { get; }

        /// <summary>
        /// Payload index.
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// Payload name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Payload value.
        /// </summary>
        public object Value { get; }
    }
}
