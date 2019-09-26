using System;

namespace Observito.Trace.EventSourceFormatter
{
    /// <summary>
    /// Payload attribute. Specifies characteristics of the payload.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, Inherited = true, AllowMultiple = false)]
    public class PayloadAttribute : Attribute
    {
        /// <summary>
        /// Creates a new payload attribute.
        /// </summary>
        /// <param name="types">Payload types</param>
        public PayloadAttribute(params PayloadType[] types)
        {
            Types = types ?? new PayloadType[] { };
        }

        /// <summary>
        /// Payload types.
        /// </summary>
        public PayloadType[] Types { get; }

        /// <summary>
        /// Suggested default value.
        /// </summary>
        public object DefaultValue { get; set; }
    }

    /// <summary>
    /// Characteristic of a payload type. This is useful for e.g. string-encoded values
    /// that are parseable into specialized types.
    /// </summary>
    public enum PayloadType
    {
        Secret,
        FileName,
        Uri,
        Base64,
        Json,
        Xml,
        Sql,
    }
}
