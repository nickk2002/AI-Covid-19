using System.Collections.Generic;
using UnityEngine;

namespace Covid19.AIBehaviour.Behaviour
{
    public class AgentManager : MonoBehaviour
    {
        public static AgentManager Instance;
        public List<AgentNPC> agentNPCList = new List<AgentNPC>();
        public GeneralAIConfiguration generalConfiguration;

        public void AddAgent(AgentNPC agentNPC)
        {
            if (!agentNPCList.Contains(agentNPC))
                agentNPCList.Add(agentNPC);
        }

        private void Awake()
        {
            Instance = this;
        }
    }
}