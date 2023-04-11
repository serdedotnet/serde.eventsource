
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Tracing;
using System.Reflection;

namespace Serde.EventSource;

using EventSource = System.Diagnostics.Tracing.EventSource;

internal static class ReflectionInfo
{
    private static readonly Type s_eventPayloadType =
        Type.GetType("System.Diagnostics.Tracing.EventPayload, System.Private.CoreLib")!;

    public static readonly Type EventDescriptorType =
        Type.GetType("System.Diagnostics.Tracing.EventDescriptor, System.Private.CoreLib")!;

    private static readonly ConstructorInfo s_makeEventPayload =
        s_eventPayloadType.GetConstructor(
            BindingFlags.NonPublic | BindingFlags.Instance,
            new[] { typeof(string[]), typeof(object?[]) })!;

    public static IDictionary<string, object?> MakeEventPayload(string[] names, object?[] values)
    {
        return (IDictionary<string, object?>)s_makeEventPayload.Invoke(new object[] { names, values });
    }

    private static readonly MethodInfo s_dispatchToAllListeners =
        typeof(EventSource).GetMethod(
            "DispatchToAllListeners",
            BindingFlags.NonPublic | BindingFlags.Instance,
            binder: null,
            types: new[] {
                typeof(EventWrittenEventArgs)
            },
            modifiers: null)!;

    private static class EventArgsHelpers
    {
        public static EventWrittenEventArgs MakeEventWrittenEventArgs(EventSource ev, int eventId)
        {
            return (EventWrittenEventArgs)typeof(EventWrittenEventArgs).GetConstructor(
                BindingFlags.NonPublic | BindingFlags.Instance,
                types: new[] {
                    typeof(EventSource),
                    typeof(int),
                })!.Invoke(new object?[] { ev, eventId });
        }

        public static void SetName(EventWrittenEventArgs eventargs, string? name)
        {
            typeof(EventWrittenEventArgs).GetMethod(
                "set_EventName",
                BindingFlags.NonPublic | BindingFlags.Instance)!
            .Invoke(eventargs, new object?[] { name });
        }

        public static void SetLevel(EventWrittenEventArgs eventargs, EventLevel level)
        {
            typeof(EventWrittenEventArgs).GetMethod(
                "set_Level",
                BindingFlags.NonPublic | BindingFlags.Instance)!
            .Invoke(eventargs, new object?[] { level });
        }

        public static void SetOpcode(EventWrittenEventArgs eventargs, EventOpcode opcode)
        {
            typeof(EventWrittenEventArgs).GetMethod(
                "set_Opcode",
                BindingFlags.NonPublic | BindingFlags.Instance)!
            .Invoke(eventargs, new object?[] { opcode });
        }

        public static void SetPayload(EventWrittenEventArgs eventargs, IList<object?> payload)
        {
            typeof(EventWrittenEventArgs).GetMethod(
                "set_Payload",
                BindingFlags.NonPublic | BindingFlags.Instance)!
            .Invoke(eventargs, new object?[] { new ReadOnlyCollection<object?>(payload) });
        }

        public static void SetPayloadNames(EventWrittenEventArgs eventargs, IList<string> payloadNames)
        {
            typeof(EventWrittenEventArgs).GetMethod(
                "set_PayloadNames",
                BindingFlags.NonPublic | BindingFlags.Instance)!
            .Invoke(eventargs, new object?[] { new ReadOnlyCollection<string>(payloadNames) });
        }
    }


    public static void DispatchToAllListeners(
        this EventSource eventSource,
        string? eventName,
        IDictionary<string, object?> eventPayload)
    {
        var eventargs = EventArgsHelpers.MakeEventWrittenEventArgs(eventSource, -1);
        EventArgsHelpers.SetName(eventargs, eventName);
        EventArgsHelpers.SetLevel(eventargs, EventLevel.Verbose);
        EventArgsHelpers.SetOpcode(eventargs, EventOpcode.Info);
        EventArgsHelpers.SetPayload(eventargs, (IList<object?>)eventPayload.Values);
        EventArgsHelpers.SetPayloadNames(eventargs, (IList<string>)eventPayload.Keys);
        s_dispatchToAllListeners.Invoke(eventSource, new object?[] { eventargs });
    }
}