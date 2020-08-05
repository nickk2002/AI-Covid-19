using System.Collections.Generic;
using Covid19.AI.Behaviour.Configuration;
using Covid19.AI.Behaviour.States;
using UnityEngine;

namespace Covid19.AI.Behaviour.Systems
{
    public class DebuggerSystem
    {
        private readonly AgentConfiguration _agentConfig;
        private readonly BehaviourSystem _behaviour;
        private readonly GeneralAIConfiguration _generalConfig;
        private readonly AgentNPC _npc;

        // TODO : put the info into a file, maybe Like Wake up patrol-> .. ->
        private List<string> _behaviourDebugs = new List<string>();

        public DebuggerSystem(AgentNPC owner)
        {
            _npc = owner;
            _agentConfig = _npc.agentConfig;
            _generalConfig = _npc.generalConfig;
            _behaviour = _npc.BehaviourSystem;
        }

        public void DebugStart(IBehaviour behaviour)
        {
            Debug.Assert(_behaviour.Dictionary.ContainsKey(behaviour) == false,
                $"{_npc} There are two coroutines running at the same time {behaviour}. Check if there is EntryTransition or if there is a yield before transition");
            Log($"<color=green> {_npc} started coroutine  {behaviour}</color>", _npc);
        }

        public void DebugExit(IBehaviour behaviour)
        {
            if (_behaviour.Dictionary.ContainsKey(behaviour))
                Log($"<color=red> {_npc} exited {behaviour} </color>", _npc);
            else if (_behaviour.EntryBehaviours.Contains(behaviour))
            {
                // daca este enable, daca este entry transition atunci nu este in wakeUp
                Debug.LogError(
                    $"{_npc} <color=red> exited {behaviour}  but the coroutine was not in dictionary, put yield return null before transition </color>", _npc);
            }
        }

        public void Log(string debug, Object context = null)
        {
            if (_npc.agentConfig.consoleDebug)
                Debug.Log(debug, context);
            _behaviourDebugs.Add(debug);
        }

        public void PrintDictionary()
        {
            Log($"{_npc} dict is : {string.Join("", _behaviour.Dictionary.Keys)}");
        }

        public void DrawLineOfSight()
        {
            if (_agentConfig == null || _generalConfig == null)
                return;
            if (!_agentConfig.sightDebug)
                return;

            Gizmos.color = Color.green;
            var circle = new Vector3[_generalConfig.viewAngle + 2];
            var index = 0;
            var position = _npc.transform.position;
            for (var angle = -_generalConfig.viewAngle / 2; angle <= _generalConfig.viewAngle / 2; angle++)
            {
                var direction = Quaternion.Euler(0, angle, 0) * _npc.transform.forward;
                if (_agentConfig.drawRealSight)
                {
                    if (Physics.Raycast(position, direction, out RaycastHit hit, _generalConfig.viewDistance))
                        circle[++index] = hit.point;
                    else
                        circle[++index] = position + direction.normalized * _generalConfig.viewDistance;
                }
                else
                {
                    circle[++index] = position + direction.normalized * _generalConfig.viewDistance;
                }
            }

            Gizmos.DrawLine(position, circle[1]);
            Gizmos.DrawLine(position, circle[index]);
            for (var i = 1; i <= index - 1; i++)
                Gizmos.DrawLine(circle[i], circle[i + 1]);
        }
        
    }
}