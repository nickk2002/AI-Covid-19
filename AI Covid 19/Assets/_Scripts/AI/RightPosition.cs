using UnityEngine;

namespace Covid19.AI
{
    public class RightPosition : StateMachineBehaviour
    {
        public bool start = true;

        // OnStateEnter is called before OnStateEnter is called on any state inside this state machine
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (start)
            {
                Debug.Log("Before modifying" + animator.transform.position.ToString("F10"));
                animator.transform.position = new Vector3(-53.20843f, 0.5326362f, -5.56715f);
                Debug.Log("After modifying" + animator.transform.position.ToString("F10"));
                animator.Update(0f);
            }
        }

        // OnStateUpdate is called before OnStateUpdate is called on any state inside this state machine
        //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    
        //}

        // OnStateExit is called before OnStateExit is called on any state inside this state machine
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!start)
            {
                Debug.Log("Before modifying" + animator.transform.position.ToString("F10"));
                //animator.transform.position = new Vector3(-53.20843f, 0.5326362f, -5.76715f);
                Debug.Log("After modifying" + animator.transform.position.ToString("F10"));
                animator.Update(0f);
            }
        }

        // OnStateMove is called before OnStateMove is called on any state inside this state machine
        //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    
        //}

        // OnStateIK is called before OnStateIK is called on any state inside this state machine
        //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    
        //}

        // OnStateMachineEnter is called when entering a state machine via its Entry Node
        //override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
        //{
        //    
        //}

        // OnStateMachineExit is called when exiting a state machine via its Exit Node
        public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
        {
        }
    }
}