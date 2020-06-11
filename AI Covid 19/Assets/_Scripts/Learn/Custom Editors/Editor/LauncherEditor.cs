using UnityEditor;
using UnityEngine;

namespace Covid19.Learn.Custom_Editors.Editor
{
    [CustomEditor(typeof(Launcher))]
    public class LauncherEditor : UnityEditor.Editor
    {
        private void OnSceneGUI()
        {
            Launcher launcher = target as Launcher;
            Transform transform = launcher.transform;
            launcher.offset = Handles.PositionHandle(transform.position + launcher.offset, Quaternion.identity) - transform.position;
            Handles.Label(transform.position + launcher.offset, "Offset");
            Handles.BeginGUI();
            Vector3 position = Camera.current.WorldToScreenPoint(launcher.transform.position + launcher.offset);
            var rect = new Rect();
            rect.x = position.x;
            rect.yMin = SceneView.currentDrawingSceneView.position.height - position.y;
            rect.width = 64;
            rect.height = 16;
            GUILayout.BeginArea(rect);
            if (GUILayout.Button("Fire"))
                launcher.Fire();
            GUILayout.EndArea();
            Handles.EndGUI();

            //launcher.offset = transform.InverseTransformPoint(Handles.PositionHandle(transform.TransformPoint(launcher.offset), transform.rotation));
        }
    }
}
