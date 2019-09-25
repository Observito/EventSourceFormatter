using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Xml.Linq;

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

            return new EventSourceIdentifier(source.Name, source.Guid);
        }

        /// <summary>
        /// Get payload metadata.
        /// </summary>
        /// <param name="source">Event source to inspect</param>
        /// <returns>Payload metadata</returns>
        public static IEnumerable<(PayloadData, PayloadType?)> GetPayloadMetadata(this EventSource source)
        {
            static (int id, string name)[] GetEvents(Type type)
            {
                var manifest = EventSource.GenerateManifest(type, ""/*, EventManifestOptions.*/);
                var xdoc = XDocument.Parse(manifest);
                var xevents = xdoc.Descendants().Where(x => x.Name.LocalName == "event");
                var results =
                    from ev in xevents
                    select
                    (
                        (int)ev.Attribute("value"),
                        (string)ev.Attribute("symbol")
                    );
                return results.ToArray();
            }

            var type = source.GetType();

            var eventIndex = GetEvents(type);

            var eventMeta =
                from mdecl in type.GetMethods()
                let eventInfo = eventIndex.FirstOrDefault(ev => ev.name == mdecl.Name)
                where mdecl.IsPublic &&
                      mdecl.DeclaringType.BaseType == typeof(EventSource) &&
                      !mdecl.IsAbstract &&
                      !mdecl.CustomAttributes.Any(ca => ca.AttributeType == typeof(NonEventAttribute)) &&
                      eventInfo != default
                      //m.GetMethodBody() .Write(int, ...)
                select new
                {
                    mdecl.Name,
                    Id = eventInfo.id,
                    mdecl.CustomAttributes,
                    Payload =
                        from p in mdecl.GetParameters()
                        select new
                        {
                            p.Name,
                            p.CustomAttributes,
                        }
                };

            var id = source.GetIdentfier();
            foreach (var e in eventMeta)
            {
                var payloadIndex = 0;
                foreach (var p in e.Payload)
                {
                    var attr = p.CustomAttributes.FirstOrDefault(at => at.AttributeType == typeof(PayloadAttribute));
                    PayloadType? payloadType = null;
                    if (attr != null)
                    {
                        foreach (var arg in attr.ConstructorArguments)
                        {
                            if (arg.ArgumentType == typeof(PayloadType))
                            {
                                payloadType = (PayloadType)arg.Value;
                                break;
                            }
                        }
                    }
                    var pd = new PayloadData(id, e.Id, e.Name, payloadIndex, p.Name, null);
                    yield return (pd, payloadType);
                    payloadIndex++;
                }
            }
        }
    }
}
