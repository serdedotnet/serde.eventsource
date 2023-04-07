
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
        var typeInfos = new List<object>();
        var prescan = new TraceLoggingEventSourceBuilder(typeInfos);
        value.Serialize(prescan);
        WriteImpl(eventName, value, typeInfos);
    }

    private static readonly Type TraceLoggingEventTypesType =
        typeof(EventSource).Assembly.GetType("System.Diagnostics.Tracing.TraceLoggingEventTypes")!;

    private static readonly MethodInfo WriteImplMethod = typeof(EventSource).GetMethod(
        "WriteImpl", BindingFlags.NonPublic | BindingFlags.Instance,
        binder: null,
        types: new Type[] {
            typeof(string),
            typeof(EventSourceOptions),
            typeof(object),
            typeof(Guid*),
            typeof(Guid*),
            TraceLoggingEventTypesType },
        modifiers: new[] { new ParameterModifier(6) { [1] = true }})!;

    private void WriteImpl(string eventName, object data, object typeInfos)
    {
        EventSourceOptions options = default;
        WriteImplMethod.Invoke(this, new[] {
            eventName,
            options,
            data,
            null,
            null,
            typeInfos,
        });
    }
}


public partial class SerdeEventSource
{
    private unsafe sealed class EventSourceSerializer : ISerializer, ISerializeType
    {
        private readonly EventData* _eventData;
        private int _index = 0;

        public EventSourceSerializer(EventData* eventData)
        {
            _eventData = eventData;
        }

        public void SerializeBool(bool b)
        {
            throw new NotImplementedException();
        }

        public void SerializeByte(byte b)
        {
            throw new NotImplementedException();
        }

        public void SerializeChar(char c)
        {
            throw new NotImplementedException();
        }

        public void SerializeDecimal(decimal d)
        {
            throw new NotImplementedException();
        }

        public ISerializeDictionary SerializeDictionary(int? length)
        {
            throw new NotImplementedException();
        }

        public void SerializeDouble(double d)
        {
            throw new NotImplementedException();
        }

        public ISerializeEnumerable SerializeEnumerable(string typeName, int? length)
        {
            throw new NotImplementedException();
        }

        public void SerializeEnumValue<T>(string enumName, string? valueName, T value) where T : notnull, ISerialize
        {
            throw new NotImplementedException();
        }

        public void SerializeFloat(float f)
        {
            throw new NotImplementedException();
        }

        public void SerializeI16(short i16)
        {
            throw new NotImplementedException();
        }

        public void SerializeI32(int i32)
        {
            _eventData[_index++] = new EventData
            {
                DataPointer = (IntPtr)(&i32),
                Size = 4,
            };
        }

        public void SerializeI64(long i64)
        {
            throw new NotImplementedException();
        }

        public void SerializeNotNull<T>(T t) where T : notnull, ISerialize
        {
            throw new NotImplementedException();
        }

        public void SerializeNull()
        {
            throw new NotImplementedException();
        }

        public void SerializeSByte(sbyte b)
        {
            throw new NotImplementedException();
        }

        public void SerializeString(string s)
        {
            throw new NotImplementedException();
        }

        public ISerializeType SerializeType(string name, int numFields)
        {
            return this;
        }

        public void SerializeU16(ushort u16)
        {
            throw new NotImplementedException();
        }

        public void SerializeU32(uint u32)
        {
            throw new NotImplementedException();
        }

        public void SerializeU64(ulong u64)
        {
            throw new NotImplementedException();
        }

        void ISerializeType.End()
        {
        }

        void ISerializeType.SerializeField<T>(string name, T value)
        {
            value.Serialize(this);
        }
    }
}
