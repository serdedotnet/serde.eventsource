
using Serde;
using System.Collections.ObjectModel;
using System.Diagnostics.Tracing;
using System.Runtime.Serialization;
using Serde.EventSource;
using Xunit;

namespace Serde.EventSource;

using EventSource = System.Diagnostics.Tracing.EventSource;

public class EventSourceTest
{
    private const string TestEventName = "TestEventName";

    private class TestSerdeEventSource : SerdeEventSource
    {
    }

    private class TestRawEventSource : EventSource
    {
    }

    private class TestListener : EventListener
    {
        public ReadOnlyCollection<object?>? Payload { get; private set; } = null;

        protected override void OnEventWritten(EventWrittenEventArgs eventData)
        {
            if (eventData.EventName == TestEventName)
            {
                Payload = eventData.Payload;
            }
        }
    }

    [EventData]
    partial struct TestTrivialStruct
    {
        public int Value { get; set; }
    }

    [EventData]
    partial struct TestWrappedStruct
    {
        public TestTrivialStruct Value { get; set; }
    }

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

    private void AssertEqualPayloads<T>(T data)
    {
        using var listener = new TestListener();
        using var testRaw = new TestRawEventSource();
        using var testSerde = new TestSerdeEventSource();
        listener.EnableEvents(testRaw, EventLevel.Verbose);
        listener.EnableEvents(testSerde, EventLevel.Verbose);
        testRaw.Write(TestEventName, data);
        var expectedPayload = listener.Payload;
        testSerde.Write(TestEventName, data);
        var actualPayload = listener.Payload;
        Assert.Equal(expectedPayload, actualPayload);
    }
}