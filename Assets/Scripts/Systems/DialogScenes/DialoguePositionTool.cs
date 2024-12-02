using UnityEditor;
using UnityEngine;

public static class DialoguePositionTool
{
    public static void StartPositionSelection(System.Action<Vector3> onPositionSelected)
    {
        SceneView.duringSceneGui += OnSceneGUI;
        selectionCallback = onPositionSelected;
    }

    private static System.Action<Vector3> selectionCallback;

    private static void OnSceneGUI(SceneView sceneView)
    {
        Event e = Event.current;

        if (e.type == EventType.MouseDown && e.button == 0)
        {
            Ray worldRay = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            if (Physics.Raycast(worldRay, out RaycastHit hit))
            {
                selectionCallback?.Invoke(hit.point);
                SceneView.duringSceneGui -= OnSceneGUI;
            }

            e.Use();
        }
    }
}
