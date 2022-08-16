using NUnit.Framework;
using UnityEngine;
using Zenject;
using static EventConstraintsTestUtilities;

[TestFixture]
public class EventConstraintsGameStateTests : ZenjectUnitTestFixture
{
    const string state1Name = "EventConstraintsGameStateTestGameState1";

    readonly Event testEvent = GetTestEvent();
    GameState state1;
    EventConstraints eventConstraints;
    bool eventInvoked;

    [SetUp]
    public void CommonInstall()
    {
        state1 = ScriptableObject.CreateInstance<GameState>();
        state1.name = state1Name;
        eventInvoked = false;

        eventConstraints = new();
        testEvent.eventConstraints = eventConstraints;
        SetupReferences();
    }

    [TearDown]
    public void TearDown() =>
        TearDownReferences();

    [Test]
    [TestCase(true, true)]
    [TestCase(true, false)]
    [TestCase(false, true)]
    [TestCase(false, false)]
    public void SetGameState_CheckIfEventFired(bool isInList, bool allowedStates)
    {
        References.gameStateManager.SetState(state1);
        if (isInList) eventConstraints.states.Add(state1);
        eventConstraints.allowStates = allowedStates;

        testEvent.Subscribe(CheckEventWasInvoked, eventConstraints);
        testEvent.Invoke();
        testEvent.Unsubscribe(CheckEventWasInvoked);

        Assert.That(eventInvoked == !(isInList ^ allowedStates));
    }

    void CheckEventWasInvoked() =>
        eventInvoked = true;
}