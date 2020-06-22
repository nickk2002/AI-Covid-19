using System;
using Covid19.AIBehaviour.Behaviour;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AgentNPC))]
public class CustomAgentNPC : Editor
{
    private Editor _cachedEditor;

    private void OnEnable()
    {
        _cachedEditor = null;
    }

    public override void OnInspectorGUI()
    {
        AgentNPC agentNPC = target as AgentNPC;
        if (_cachedEditor == null)
            _cachedEditor = Editor.CreateEditor(agentNPC.meetConfiguration);
        base.OnInspectorGUI();
        
        _cachedEditor.DrawDefaultInspector();
        
    }
}
