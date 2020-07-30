using UnityEditor;

namespace Covid19.AI.Behaviour.Custom_Editors
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(AgentNPC))]
    public class CustomAgentNPC : Editor
    {
        private AgentNPC _agentNPC;
        private Editor _cachedEditor;
        private bool _ok;

        private void OnEnable()
        {
            _cachedEditor = null;
            _agentNPC = target as AgentNPC;
        }

        public override void OnInspectorGUI()
        {
            if (_cachedEditor == null && _ok)
                if (_agentNPC.agentConfig != null)
                    _cachedEditor = CreateEditor(_agentNPC.agentConfig);

            base.OnInspectorGUI();

            if (_cachedEditor)
                _cachedEditor.DrawDefaultInspector();
        }
    }
}