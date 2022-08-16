using NUnit.Framework;
using UnityEngine;
using Zenject;
using static EventConstraintsTestUtilities;
using Constraint = EventConstraints.GameObjectConstraint;
using ConstraintType = EventConstraints.GameObjectConstraintType;

[TestFixture]
public class EventConstraintsGameObjectTests : ZenjectUnitTestFixture
{
    readonly Event testEvent = GetTestEvent();
    GameObject testPrefab1;
    GameObject testPrefab2;
    GameObject instantiatedTestPrefab1;
    EventConstraints eventConstraints;
    EventInfo eventInfo;
    bool eventInvoked;

    [SetUp]
    public void CommonInstall()
    {
        testPrefab1 = GetTestPrefab(1);
        testPrefab2 = GetTestPrefab(2);
        instantiatedTestPrefab1 = Object.Instantiate(testPrefab1);

        eventConstraints = new();
        eventConstraints.allowStates = false;
        eventInfo = new();
        eventInvoked = false;

        Container.Bind<GameStateManager>().FromInstance(new()).AsSingle();
        Container.Inject(testEvent);
        SetupReferences();
    }

    [TearDown]
    public void TearDown() =>
        TearDownReferences();

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    // No constraints should always success regardless of other factors
    public void NoConstraints(bool gameObjectInEventInfo)
    {
        eventInfo.gameObject = gameObjectInEventInfo ? instantiatedTestPrefab1 : null;
        testEvent.Subscribe(EventInvoked, eventConstraints);

        testEvent.Invoke(eventInfo);

        Assert.That(eventInvoked);
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void InstanceOfPrefabConstraint(bool useInstance)
    {
        AddConstraint(testPrefab1, ConstraintType.InstanceOfPrefab);
        eventInfo.gameObject = useInstance ? instantiatedTestPrefab1 : testPrefab2;
        testEvent.Subscribe(EventInvoked, eventConstraints);

        testEvent.Invoke(eventInfo);

        Assert.That(eventInvoked == useInstance);
    }

    void EventInvoked() =>
        eventInvoked = true;

    void AddConstraint(GameObject gameObject, ConstraintType constraintType)
    {
        Constraint constraint = new(gameObject, constraintType);
        eventConstraints.gameObjectConstraints.Add(constraint);
    }
}