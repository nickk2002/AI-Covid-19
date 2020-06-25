using System.Collections;
using Covid19.AIBehaviour.Behaviour;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Tests
{
    public class BehaviourTests
    {
        
        [UnitySetUp] 
        public IEnumerator SetUp()
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync("_Scenes/BasicScene");
            while (operation.isDone == false)
                yield return null;
            yield return null;
        }

        private AgentNPC LoadNpc()
        {
            GameObject prefab = Resources.Load("AgentBot") as GameObject;
            Assert.IsNotNull(prefab,"Resources not found");
            GameObject npc = Object.Instantiate(prefab);
            return npc.GetComponent<AgentNPC>();
        }
        [UnityTest]
        public IEnumerator BehaviourTestsWithEnumeratorPasses()
        {
            
            AgentNPC npc1 = LoadNpc();
            AgentNPC npc2 = LoadNpc();
            npc1.transform.position = Vector3.zero;
            npc2.transform.position = Vector3.zero + Vector3.forward * 10;
            Debug.Break();
            
            yield return new WaitForSeconds(10f);
        }
    }
}
