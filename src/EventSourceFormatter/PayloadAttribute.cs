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
        /// <param name="type">Payload type</param>
        public PayloadAttribute(PayloadType @type)
        {
            Type = @type;
        }
        
        /// <summary>
        /// Payload type.
        /// </summary>
        public PayloadType Type { get; }
    }

    /// <summary>
    /// Payload type.
    /// </summary>
    public enum PayloadType
    {
        /// <summary>
        /// Sensitive information, e.g. personal data.
        /// </summary>
        Sensitive,

        //FileName,

        /// <summary>
        /// URL string.
        /// </summary>
        Url,

        /// <summary>
        /// JSON string.
        /// </summary>
        Json,
    }
}
