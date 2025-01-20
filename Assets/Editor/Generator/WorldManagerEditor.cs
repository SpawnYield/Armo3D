using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WorldManager))]
public class WorldManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        WorldManager worldManager = (WorldManager)target;

        if (GUILayout.Button("Generate World"))
        {
            worldManager.GenerateWorld();
        }
        if (GUILayout.Button("Save World"))
        {
            worldManager.SaveWorld();
        }
        if (GUILayout.Button("Load World"))
        {
            worldManager.LoadWorld();
        }
    }
}
