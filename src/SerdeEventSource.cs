
namespace Serde.EventSource;

using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Globalization;
using System.Reflection;
using EventSource = System.Diagnostics.Tracing.EventSource;

public abstract partial class SerdeEventSource : EventSource
{
    public new unsafe void Write<T>(string eventName, T value) where T : ISerialize
    {
        if (IsEnabled())
        {
            var serializer = new EventPayloadSerializer();
            value.Serialize(serializer);
            var payload = serializer.GetPayload();
            ReflectionInfo.DispatchToAllListeners(this, eventName, payload);
        }
    }
}