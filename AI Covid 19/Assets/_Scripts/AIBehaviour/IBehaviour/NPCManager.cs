using System.Collections.Generic;
using UnityEngine;

namespace Covid19.AIBehaviour.Behaviour
{
    public class NPCManager : MonoBehaviour
    {
        public static NPCManager Instance;
        public List<AgentNPC> agentNpcs = new List<AgentNPC>();
        public GeneralAIConfiguration generalConfiguration;

        private void Awake()
        {
            Instance = this;
        }
    }
}