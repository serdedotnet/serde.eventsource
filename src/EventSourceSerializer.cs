
using System;

namespace Serde;

public sealed class EventSourceSerializer : ISerializer
{
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
        throw new NotImplementedException();
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
        throw new NotImplementedException();
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
}