using System;
using System.Collections.Generic;
using Covid19.AIBehaviour.Behaviour;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

[CustomEditor(typeof(AgentNPC))]
public class CustomAgentNPC : Editor
{
    private Editor _cachedEditor;
    private AgentNPC _agentNPC;

    private void OnEnable()
    {
        _cachedEditor = null;
        _agentNPC = target as AgentNPC;
    }

    public override void OnInspectorGUI()
    {

        if (_cachedEditor == null)
            _cachedEditor = Editor.CreateEditor(_agentNPC.meetConfiguration);
        base.OnInspectorGUI();
        
        _cachedEditor.DrawDefaultInspector();
    }

    [DrawGizmo(GizmoType.Selected | GizmoType.Active | GizmoType.Pickable)]
    static void DrawPatrolPositions(AgentNPC npc, GizmoType gizmoType)
    {
        Debug.Log("intra in gizmos");

        List<Vector3> positions = new List<Vector3>();
        int index = 0;
        Color randomColor = Color.green;
        Gizmos.color = randomColor;
        Handles.color = randomColor;
        foreach (Transform child in npc.posHolder.transform)
        {
            var childPosition = child.position;
            index++;
            Gizmos.DrawCube(childPosition,new Vector3(0.5f,0.5f,0.5f));
            Handles.Label(childPosition,$"Positions {index}");
            positions.Add(childPosition);
        }
        for (int i = 0; i < positions.Count - 1; i++)
        {
            Gizmos.DrawLine(positions[i],positions[i + 1]);
        }
        Gizmos.DrawLine(positions[positions.Count - 1], positions[0]);
    }

}
