using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.Tracing;
using Xunit;

namespace Serde.EventSource;

using EventSource = System.Diagnostics.Tracing.EventSource;

public partial class ListenerBasedTest
{
    [Fact]
    public void TestRawInt()
    {
        using var listener = new TestListener();

        using var testRaw = new TestRawEventSource();
        listener.EnableEvents(testRaw, EventLevel.Verbose);
        const int expected = 23;
        Assert.ThrowsAny<Exception>(() => testRaw.Write<int>(TestEventName, expected));

        using var testSerde = new TestSerdeEventSource();
        listener.EnableEvents(testSerde, EventLevel.Verbose);
        Assert.Throws<ArgumentException>(() => testSerde.Write<Int32Wrap>(TestEventName, new Int32Wrap(expected)));
    }

    [Fact]
    public void SelfDescribingTestTrivialPayload()
    {
        AssertEqualSelfDescribingPayloads(new TestTrivialStruct { Value = 42 });
    }

    [Fact]
    public void SelfDescribingWrapped()
    {
        AssertEqualSelfDescribingPayloads(new TestWrappedStruct { Value = new TestTrivialStruct { Value = 42 } });
    }

    [Fact]
    public void ManifestTrivialPayload()
    {
        AssertEqualManifestPayloads(
            ev => ev.TestEvent(),
            ev => ev.TestEvent());
    }

    private const string TestEventName = "TestEventName";
    private static readonly TestTrivialStruct s_manifestPayload1 = new TestTrivialStruct { Value = 42 };

    private class TestSerdeEventSource : SerdeEventSource
    {
        [Event(1)]
        public void TestEvent()
        {
            base.WriteEvent(1, s_manifestPayload1);
        }
    }

    private class TestRawEventSource : System.Diagnostics.Tracing.EventSource
    {
        [Event(1)]
        public void TestEvent()
        {
            base.WriteEvent(1, s_manifestPayload1);
        }
    }

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
    [SerdeTypeOptions(MemberFormat = MemberFormat.None)]
    partial struct TestTrivialStruct
    {
        public int Value { get; set; }
    }

    [EventData]
    [GenerateSerialize]
    [SerdeTypeOptions(MemberFormat = MemberFormat.None)]
    partial struct TestWrappedStruct
    {
        public TestTrivialStruct Value { get; set; }
    }

    private void AssertEqualSelfDescribingPayloads<T>(T data) where T : ISerialize
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

    private void AssertEqualManifestPayloads(
        Action<TestSerdeEventSource> serdeAction,
        Action<TestRawEventSource> rawAction)
    {
        using var listener = new TestListener();
        using var testRaw = new TestRawEventSource();
        using var testSerde = new TestSerdeEventSource();
        listener.EnableEvents(testRaw, EventLevel.Verbose);
        listener.EnableEvents(testSerde, EventLevel.Verbose);
        rawAction(testRaw);
        var expectedPayload = listener.GetAndResetPayload();
        serdeAction(testSerde);
        var actualPayload = listener.GetAndResetPayload();
        Assert.Equal(expectedPayload, actualPayload);
    }
}