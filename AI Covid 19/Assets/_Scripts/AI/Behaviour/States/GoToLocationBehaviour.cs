using System.Collections;
using Covid19.Utils;
using UnityEngine;
using UnityEngine.AI;

namespace Covid19.AI.Behaviour.States
{
    public class GoToLocationBehaviour : MonoBehaviour, IBehaviour
    {
        private AgentNPC _npc;
        public Vector3 destination;
        public float stopDistance = 0.1f;
        public float remainingDistance;
        public string LocationName { set; get; } = "Somewhere";

        public void Entry()
        {
            _npc = GetComponent<AgentNPC>();
            _npc.Agent.isStopped = false;
            _npc.Agent.SetDestination(destination);
        }

        public void Exit()
        {
            
        }
        
        public IEnumerator OnUpdate()
        {
            NavMeshPath path = new NavMeshPath();
            NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, path);
       
            // if (path.status == NavMeshPathStatus.PathPartial)
            // {
            //     Debug.LogWarning($"{_npc} The Path to the {destination} at {LocationName} is partial. Check the navmesh and the position",CreateGameObject());
            // }
            //
            // if (path.status == NavMeshPathStatus.PathInvalid)
            // {
            //     Debug.LogError($"{_npc} The Path to the {destination} at {LocationName} is invalid, can't be reached!", CreateGameObject());
            // }

            while (true)
            {
                // TODO Check if the position is reachable
                remainingDistance = Vector3.Distance(destination, transform.position);
                if (Vector3.SqrMagnitude(destination - transform.position) < stopDistance * stopDistance)
                {
                    yield return null;
                    _npc.BehaviourSystem.RemoveBehaviour(this);
                }

                yield return null;
            }
        }

        private GameObject CreateGameObject()
        {
            Debug.Log("called once");
            GameObject go = new GameObject();
            go.name = "Invalid Destination!!!";
            go.transform.position = destination;
            Texture2D texture = Resources.Load("invalid") as Texture2D;
            
            CustomHierarchy.SetIcon(go,texture);
            return go;
        }

        public override string ToString()
        {
            return "Going To" + LocationName;
        }
    }
}