using NUnit.Framework;
using UnityEngine;
using Zenject;
using UnityEditor;
using static EventConstraintsGameStateAssetPostProcessor;
using static EventConstraintsTestUtilities;

[TestFixture]
public class EventConstraintsGameStateAssetPostProcessorTests : ZenjectUnitTestFixture
{
    const string stateName = "EventConstrainsGameStateTest";
    const string renamedStateName = "EventConstrainsGameStateTestRenames";
    readonly string allowedFolder = pathsToSearch[0];
    readonly string notAllowedFolder = "Assets/Tests";
    GameState state;
    EventConstraintsMono eventConstraints;

    [SetUp]
    public void CommonInstall()
    {
        state = ScriptableObject.CreateInstance<GameState>();
        state.name = stateName;

        GameObject testObject = GetTestPrefab(1);
        eventConstraints = testObject.AddComponent<EventConstraintsMono>();
        eventConstraints.eventConstraints.states.Clear();
    }

    [TearDown]
    public void TearDown()
    {
        AssetDatabase.DeleteAsset(GetAssetPath(state, true));
        if (state) AssetDatabase.DeleteAsset(GetAssetPath(state, false));
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void CreateAssetInFolder_CheckRegistration(bool createInAllowedFolder)
    {
        CreateAsset(state, createInAllowedFolder);
        Assert.That(GetGameStateNames().Contains(stateName) == createInAllowedFolder);
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void CreateAssetInFolder_MoveToTheOtherFolder_CheckRegistration(bool createInAllowedFolder)
    {
        CreateAsset(state, createInAllowedFolder);
        MoveToFolder(state, !createInAllowedFolder);
        Assert.That(GetGameStateNames().Contains(stateName) != createInAllowedFolder);
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void CreateAndRenameAsset_CheckRegistration(bool createInAllowedFolder)
    {
        CreateAsset(state, createInAllowedFolder);
        RenameAsset(state, renamedStateName, createInAllowedFolder);
        Assert.That(GetGameStateNames().Contains(stateName) == false);
        Assert.That(GetGameStateNames().Contains(renamedStateName) == createInAllowedFolder);
    }

    void MoveToFolder(GameState state, bool moveToAllowed) =>
        AssetDatabase.MoveAsset(GetAssetPath(state, !moveToAllowed), GetAssetPath(state, moveToAllowed));

    void CreateAsset(GameState state, bool allowed) =>
        AssetDatabase.CreateAsset(state, GetAssetPath(state, allowed));

    void RenameAsset(GameState state, string newName, bool allowedFolder)
    {
        // Must use the MoveAsset method; using RenameAsset will not trigger
        // the asset to be imported for purposes of an AssetPostProcessor.
        // This method also correctly stimulates the behaviour of an 
        // AssetPostProcessor as if the asset was renamed manually in the Editor
        AssetDatabase.MoveAsset(GetAssetPath(state, allowedFolder),
                                GetAssetPath(newName, allowedFolder));
    }

    string GetAssetPath(GameState state, bool allowed)
    {
        string path = allowed ? allowedFolder : notAllowedFolder;
        return $"{path}/{state.name}.asset";
    }

    string GetAssetPath(string name, bool allowed)
    {
        string path = allowed ? allowedFolder : notAllowedFolder;
        return $"{path}/{name}.asset";
    }
}