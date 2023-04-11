
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;

namespace Serde.EventSource;

/// <summary>
/// This is a serializer that runs through the type and pre-calculates the size of the payload and
/// other data needed for writing to base event source.
/// </summary>
internal sealed partial class EventPayloadSerializer : ISerializer
{
    private object? _result = null;

    public IDictionary<string, object?> GetPayload()
    {
        if (_result == null)
        {
            throw new InvalidOperationException("Payload not yet calculated");
        }

        if (_result is IDictionary<string, object?> d)
        {
            return d;
        }
        throw new ArgumentException("Cannot write primitive type directly");
    }

    private void WriteBoxed(object? o) => _result = o;

    public void SerializeBool(bool b) => WriteBoxed(b);
    public void SerializeByte(byte b) => WriteBoxed(b);
    public void SerializeChar(char c) => WriteBoxed(c);
    public void SerializeDecimal(decimal d) => WriteBoxed(d);
    public void SerializeDouble(double d) => WriteBoxed(d);
    public void SerializeFloat(float f) => WriteBoxed(f);
    public void SerializeI16(short i16) => WriteBoxed(i16);
    public void SerializeI32(int i32) => WriteBoxed(i32);
    public void SerializeI64(long i64) => WriteBoxed(i64);
    public void SerializeNotNull<T>(T t) where T : notnull, ISerialize
        => t.Serialize(this);
    public void SerializeNull() => WriteBoxed(null);
    public void SerializeSByte(sbyte b) => WriteBoxed(b);
    public void SerializeString(string s) => WriteBoxed(s);
    public void SerializeU16(ushort u16) => WriteBoxed(u16);
    public void SerializeU32(uint u32) => WriteBoxed(u32);
    public void SerializeU64(ulong u64) => WriteBoxed(u64);
    public void SerializeEnumValue<T>(string enumName, string? valueName, T value) where T : notnull, ISerialize
    {
        throw new System.NotImplementedException();
    }

    public ISerializeDictionary SerializeDictionary(int? length)
    {
        throw new System.NotImplementedException();
    }


    public ISerializeEnumerable SerializeEnumerable(string typeName, int? length)
    {
        throw new System.NotImplementedException();
    }

    private readonly struct EventPayloadBuilder
    {
        private readonly List<string> _names;
        private readonly List<object?> _values;

        public EventPayloadBuilder(List<string> names, List<object?> values)
        {
            _names = names;
            _values = values;
        }

        public EventPayloadBuilder(int numFields)
        {
            _names = new List<string>(numFields);
            _values = new List<object?>(numFields);
        }

        public void AddName(string name)
        {
            _names.Add(name);
        }

        public void AddValue(object? value)
        {
            _values.Add(value);
        }

        public IDictionary<string, object?> Build()
        {
            return ReflectionInfo.MakeEventPayload(_names.ToArray(), _values.ToArray());
        }
    }
    public ISerializeType SerializeType(string name, int numFields)
    {
        return new TypeSerializer(this, numFields);
    }
}