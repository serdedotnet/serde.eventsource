
namespace Serde.EventSource;

using EventSource = System.Diagnostics.Tracing.EventSource;

public abstract class SerdeEventSource : EventSource
{
    protected new void Write<T>(string eventName, T value) where T : ISerialize
    {
        base.Write(eventName, value);
    }
}
