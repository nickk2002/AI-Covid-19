using System.Collections.Generic;
using Covid19.AI.Behaviour.States;
using UnityEngine;

namespace Covid19.AI.Behaviour.Systems
{
    public class DebuggerSystem
    {
        private readonly AgentNPC _npc;

        private List<string>
            _behaviourDebugs = new List<string>(); // TODO : put the info into a file, maybe Like Wake up patrol-> .. ->

        public DebuggerSystem(AgentNPC owner)
        {
            _npc = owner;
        }

        public void DebugStart(IBehaviour behaviour)
        {
            Debug.Assert(_npc.BehaviourSystem.Dictionary.ContainsKey(behaviour) == false,
                $"{_npc} There are two coroutines running at the same time {behaviour}");
            Log($"<color=green> {_npc} started coroutine  {behaviour} + add to dict </color>", _npc);
        }

        public void DebugExit(IBehaviour behaviour)
        {
            if (_npc.BehaviourSystem.Dictionary.ContainsKey(behaviour))
            {
                Log($"<color=red> {_npc} exited {behaviour} + remove from dict</color>", _npc);
            }
            else if (_npc.BehaviourSystem.wakedUpBehaviours.Contains(behaviour)
            ) // daca este enable, daca este entry transition atunci nu este in wakeUp
            {
                Debug.LogError(
                    $"{_npc} <color=red> exited {behaviour}  but the coroutine was not in dictionary, put yield return null before transition </color>",
                    _npc);
            }
        }

        public void Log(string debug, Object context = null)
        {
            if (_npc.agentConfig.debug)
                Debug.Log(debug, context);
            _behaviourDebugs.Add(debug);
        }

        public void PrintDictionary()
        {
            Log($"{_npc} dict is : {string.Join("", _npc.BehaviourSystem.Dictionary.Keys)}");
        }
    }
}