using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewAnimation : MonoBehaviour
{
    public string animationName;
    Animator animator;

    private void OnDrawGizmos()
    {
        //if (animator == null)
        //{
        //    animator = GetComponent<Animator>();
        //    animator.Play(animationName);
        //    animator.speed = 0;
        //}
        //animator.Update(Time.deltaTime);
    }
}
