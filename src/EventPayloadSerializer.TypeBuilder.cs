
namespace Serde.EventSource;

internal sealed partial class EventPayloadSerializer
{
    private sealed class TypeSerializer : ISerializeType
    {
        private readonly EventPayloadSerializer _serializer;
        private readonly EventPayloadBuilder _builder;

        public TypeSerializer(EventPayloadSerializer serializer, int numFields)
        {
            _serializer = serializer;
            _builder = new EventPayloadBuilder(numFields);
        }

        void ISerializeType.End()
        {
            _serializer._result = _builder.Build();
        }

        void ISerializeType.SerializeField<T>(string name, T value)
        {
            _builder.AddName(name);
            value.Serialize(_serializer);
            _builder.AddValue(_serializer._result);
        }
    }
}