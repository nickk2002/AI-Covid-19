using UnityEngine;
using System.Collections.Generic;

namespace Covid19.AIBehaviour.Behaviour
{
    public class NPCManager : MonoBehaviour
    {
        public List<AgentNPC> agentNpcs = new List<AgentNPC>();
        public GeneralAIConfiguration generalConfiguration;
        public static NPCManager Instance;

        void Awake()
        {
            Instance = this;
        }
    }
}