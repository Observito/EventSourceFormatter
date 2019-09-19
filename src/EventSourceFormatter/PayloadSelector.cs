namespace Observito.Trace.EventSourceFormatter
{
    /// <summary>
    /// Payload selector. Transformation from source payload type to another type.
    /// </summary>
    /// <typeparam name="TResult">Result type</typeparam>
    /// <param name="payload">Payload to transform</param>
    /// <returns>The transformation result</returns>
    public delegate TResult PayloadSelector<TResult>(Payload payload);
}
