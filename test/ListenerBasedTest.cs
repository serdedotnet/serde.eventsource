using System.Collections.ObjectModel;
using System.Diagnostics.Tracing;
using Xunit;

namespace Serde.EventSource;

public partial class ListenerBasedTest
{
    [Fact]
    public void TestTrivialPayload()
    {
        AssertEqualPayloads(new TestTrivialStruct { Value = 42 });
    }

    [Fact]
    public void TestWrapped()
    {
        AssertEqualPayloads(new TestWrappedStruct { Value = new TestTrivialStruct { Value = 42 } });
    }

    private const string TestEventName = "TestEventName";

    private class TestSerdeEventSource : SerdeEventSource { }

    private class TestRawEventSource : System.Diagnostics.Tracing.EventSource { }

    private class TestListener : EventListener
    {
        private ReadOnlyCollection<object?>? _payload = null;

        public ReadOnlyCollection<object?>? GetAndResetPayload()
        {
            var payload = _payload;
            _payload = null;
            return payload;
        }

        protected override void OnEventWritten(EventWrittenEventArgs eventData)
        {
            if (eventData.EventName == TestEventName)
            {
                _payload = eventData.Payload;
            }
        }
    }

    [EventData]
    [GenerateSerialize]
    partial struct TestTrivialStruct
    {
        public int Value { get; set; }
    }

    [EventData]
    [GenerateSerialize]
    partial struct TestWrappedStruct
    {
        public TestTrivialStruct Value { get; set; }
    }

    private void AssertEqualPayloads<T>(T data) where T : ISerialize
    {
        using var listener = new TestListener();
        using var testRaw = new TestRawEventSource();
        using var testSerde = new TestSerdeEventSource();
        listener.EnableEvents(testRaw, EventLevel.Verbose);
        listener.EnableEvents(testSerde, EventLevel.Verbose);
        testRaw.Write(TestEventName, data);
        var expectedPayload = listener.GetAndResetPayload();
        testSerde.Write(TestEventName, data);
        var actualPayload = listener.GetAndResetPayload();
        Assert.Equal(expectedPayload, actualPayload);
    }
}