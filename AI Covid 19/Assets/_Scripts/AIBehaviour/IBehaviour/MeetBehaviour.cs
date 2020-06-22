using System.Collections;
using UnityEngine;

namespace Covid19.AIBehaviour.Behaviour
{
    public class MeetBehaviour : MonoBehaviour, IBehaviour
    {
        public Vector3 MeetPosition { set; private get; }
        private Animator _animator;
        private AgentNPC _npc;

        private static readonly int Talking = Animator.StringToHash("talking");

        public void Enable()
        {
            Debug.Log("Am intrat in enable de la meeting");
            _npc = GetComponent<AgentNPC>();
            _npc.Agent.SetDestination(MeetPosition);
            _animator = GetComponent<Animator>();
        }


        public void Disable()
        {

        }

        public IEnumerator OnUpdate()
        {
            while (true)
            {
                Debug.Assert(_npc.Agent != null,"Assertion failed");
                if (_npc.Agent.remainingDistance < 0.1f)
                {
                    _npc.Agent.isStopped = true;
                    _animator.SetBool(Talking, true);
                    yield return new WaitForSeconds(UnityEngine.Random.Range(3,5));
                    Debug.Log("Finished the talking stuff");
                    _npc.Agent.isStopped = false;
                    _animator.SetBool(Talking, false);
                    _npc.RemoveBehaviour(this);
                }

                yield return null;
            }
        }
    }
}