using System;
using System.Collections.Generic;
using Covid19.AIBehaviour.Behaviour;
using Covid19.Utils;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

[CustomEditor(typeof(AgentNPC))]
public class CustomAgentNPC : Editor
{
    private Editor _cachedEditor;
    private AgentNPC _agentNPC;
    private bool _ok;
    
    private void OnEnable()
    {
        _cachedEditor = null;
        _agentNPC = target as AgentNPC;
    }

    public override void OnInspectorGUI()
    {
        if (_cachedEditor == null && _ok)
        {
            if(_agentNPC.agentConfiguration != null)
                _cachedEditor = Editor.CreateEditor(_agentNPC.agentConfiguration);
        }

        base.OnInspectorGUI();
        
        if(_cachedEditor)
            _cachedEditor.DrawDefaultInspector();
    }



}
