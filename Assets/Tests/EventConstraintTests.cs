using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Zenject;
using UnityEditor;

[TestFixture]
public class GameObjectEventConstraintsTests : ZenjectUnitTestFixture
{
    bool eventInvoked = false;
    bool setup = false;
    GameObject testPrefab1;
    GameObject testPrefab2;
    GameObject instantiatedTestPrefab;
    EventConstraints eventConstraints;
    EventInfo eventInfo;
    Event testEvent;

    [SetUp]
    public void CommonInstall()
    {
        testPrefab1 = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Tests/EventConstraintsTest1.prefab");
        testPrefab2 = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Tests/EventConstraintsTest2.prefab");
        instantiatedTestPrefab = Object.Instantiate(testPrefab1);

        eventConstraints = new();
        eventInfo = new();

        testEvent = AssetDatabase.LoadAssetAtPath<Event>("Assets/Tests/TestEvent.asset");
    }

    [UnityTest]
    public IEnumerator InstanceOfPrefabConstraint_WithInstanceOfGivenPrefab()
    {
        EventConstraints.GameObjectConstraint constraint = new(testPrefab1, EventConstraints.GameObjectConstraintType.InstanceOfPrefab);
        eventConstraints.gameObjectConstraints = new() { constraint };
        eventInfo.gameObject = instantiatedTestPrefab;
        testEvent.Subscribe(EventInvoked, eventConstraints);

        setup = true;

        testEvent.Invoke(eventInfo);
        Assert.That(eventInvoked);

        yield return null;
    }

    [UnityTest]
    public IEnumerator InstanceOfPrefabConstraint_WithInstanceOfDifferentPrefab()
    {
        EventConstraints.GameObjectConstraint constraint = new(testPrefab1, EventConstraints.GameObjectConstraintType.InstanceOfPrefab);
        eventConstraints.gameObjectConstraints = new() { constraint };
        eventInfo.gameObject = testPrefab2;
        testEvent.Subscribe(EventInvoked, eventConstraints);

        setup = true;

        testEvent.Invoke(eventInfo);
        Assert.That(!eventInvoked);

        yield return null;
    }

    void EventInvoked()
    {
        eventInvoked = true;
    }
}