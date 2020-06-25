using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Covid19.AIBehaviour.Behaviour
{
    public interface IBehaviour
    {
        void Enable();
        void Disable();
        IEnumerator OnUpdate();
    }
}