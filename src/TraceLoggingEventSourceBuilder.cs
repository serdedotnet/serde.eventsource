
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Serde.EventSource;

/// <summary>
/// This is a serializer that runs through the type and pre-calculates the size of the payload and
/// other data needed for writing to base event source.
/// </summary>
internal sealed class TraceLoggingEventSourceBuilder : ISerializer, ISerializeType
{
    private bool _arrayOf = false;
    private readonly List<object> _typeInfos = new List<object>();

    public TraceLoggingEventSourceBuilder(List<object> typeInfos)
    {
        _typeInfos = typeInfos;
    }

    private object MakeTraceLoggingTypeInfo()
    {
        return null!;
    }

    private static class ScalarTypeInfo
    {
        private static readonly Type ScalarTypeInfoType = Type.GetType("System.Diagnostics.Tracing.ScalarTypeInfo`1, System.Private.CoreLib")!;
        public static readonly object Boolean = ScalarTypeInfoType.GetMethod("Boolean")!.Invoke(null, null)!;
        public static readonly object Byte = ScalarTypeInfoType.GetMethod("Byte")!.Invoke(null, null)!;
        public static readonly object Char = ScalarTypeInfoType.GetMethod("Char")!.Invoke(null, null)!;
        public static readonly object Int16 = ScalarTypeInfoType.GetMethod("Int16")!.Invoke(null, null)!;
        public static readonly object Int32 = ScalarTypeInfoType.GetMethod("Int32")!.Invoke(null, null)!;
        public static readonly object Int64 = ScalarTypeInfoType.GetMethod("Int64")!.Invoke(null, null)!;
        public static readonly object SByte = ScalarTypeInfoType.GetMethod("SByte")!.Invoke(null, null)!;
        public static readonly object UInt16 = ScalarTypeInfoType.GetMethod("UInt16")!.Invoke(null, null)!;
        public static readonly object UInt32 = ScalarTypeInfoType.GetMethod("UInt32")!.Invoke(null, null)!;
        public static readonly object UInt64 = ScalarTypeInfoType.GetMethod("UInt64")!.Invoke(null, null)!;
        public static readonly object Single = ScalarTypeInfoType.GetMethod("Single")!.Invoke(null, null)!;
        public static readonly object Double = ScalarTypeInfoType.GetMethod("Double")!.Invoke(null, null)!;
    }

    public void SerializeBool(bool b)
    {
        _typeInfos.Add(ScalarTypeInfo.Boolean);
    }

    public void SerializeByte(byte b)
    {
        throw new System.NotImplementedException();
    }

    public void SerializeChar(char c)
    {
        throw new System.NotImplementedException();
    }

    public void SerializeDecimal(decimal d)
    {
        throw new System.NotImplementedException();
    }

    public ISerializeDictionary SerializeDictionary(int? length)
    {
        throw new System.NotImplementedException();
    }

    public void SerializeDouble(double d)
    {
        throw new System.NotImplementedException();
    }

    public ISerializeEnumerable SerializeEnumerable(string typeName, int? length)
    {
        throw new System.NotImplementedException();
    }

    public void SerializeEnumValue<T>(string enumName, string? valueName, T value) where T : notnull, ISerialize
    {
        throw new System.NotImplementedException();
    }

    public void SerializeFloat(float f)
    {
        throw new System.NotImplementedException();
    }

    public void SerializeI16(short i16)
    {
        throw new System.NotImplementedException();
    }

    public void SerializeI32(int i32) => _typeInfos.Add(ScalarTypeInfo.Int32);

    public void SerializeI64(long i64)
    {
        throw new System.NotImplementedException();
    }

    public void SerializeNotNull<T>(T t) where T : notnull, ISerialize
    {
        throw new System.NotImplementedException();
    }

    public void SerializeNull()
    {
        throw new System.NotImplementedException();
    }

    public void SerializeSByte(sbyte b)
    {
        throw new System.NotImplementedException();
    }

    public void SerializeString(string s)
    {
        throw new System.NotImplementedException();
    }

    public ISerializeType SerializeType(string name, int numFields)
    {
        return this;
    }

    public void SerializeU16(ushort u16)
    {
        throw new System.NotImplementedException();
    }

    public void SerializeU32(uint u32)
    {
        throw new System.NotImplementedException();
    }

    public void SerializeU64(ulong u64)
    {
        throw new System.NotImplementedException();
    }

    void ISerializeType.End()
    {
    }

    void ISerializeType.SerializeField<T>(string name, T value)
    {
        value.Serialize(this);
    }
}