namespace Observito.Trace.EventSourceFormatter
{
    /// <summary>
    /// Payload with contextual metadata.
    /// </summary>
    public struct Payload
    {
        /// <summary>
        /// Event source identifier.
        /// </summary>
        public EventSourceIdentifier Source { get; set; }

        /// <summary>
        /// Event name.
        /// </summary>
        public string EventName { get; set; }

        /// <summary>
        /// Payload name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Payload value.
        /// </summary>
        public object Value { get; set; }
    }
}
