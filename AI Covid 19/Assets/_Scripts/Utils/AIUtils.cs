using Covid19.AI.Behaviour;
using UnityEngine;

namespace Covid19.Utils
{
    public class AIUtils
    {
        public static bool CanSeeObject(Transform initial, Transform target, float dist, float angle)
        {
            var diferenta = target.position - initial.position;
            if (Vector3.Distance(initial.position, target.position) <= dist)
            {
                if (Vector3.Angle(initial.forward, diferenta) <= angle / 2)
                {
                    if (Physics.Raycast(initial.position, diferenta, out var hit))
                    {
                        if (hit.collider.gameObject.GetComponent<AgentNPC>() != null)
                            return true;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}