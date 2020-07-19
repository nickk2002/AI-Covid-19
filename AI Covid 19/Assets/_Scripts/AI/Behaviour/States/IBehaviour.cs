using System.Collections;

namespace Covid19.AI.Behaviour.States
{
    public interface IBehaviour
    {
        void Enable();
        void Disable();
        IEnumerator OnUpdate();
    }
}