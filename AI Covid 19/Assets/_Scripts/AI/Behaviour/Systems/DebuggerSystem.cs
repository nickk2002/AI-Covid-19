using System.Collections.Generic;
using UnityEngine;

namespace Covid19.AI.Behaviour.Systems
{
    public class DebuggerSystem
    {
        private AgentNPC _npc;
        private List<string> _behaviourDebugs = new List<string>(); // TODO : put the info into a file, maybe Like Wake up patrol-> .. ->
        
        public DebuggerSystem(AgentNPC owner)
        {
            _npc = owner;
        }

        public void AddDebugLog(string debug, Object context = null)
        {
            if(_npc.agentConfig.debug)
                Debug.Log(debug,context);
            _behaviourDebugs.Add(debug);
        }
    }
}