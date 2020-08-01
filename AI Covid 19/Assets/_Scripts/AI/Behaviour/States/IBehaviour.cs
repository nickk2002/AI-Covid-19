using System.Collections;

namespace Covid19.AI.Behaviour.States
{
    public interface IBehaviour
    {
        void WakeUp();
        void Disable();
        IEnumerator OnUpdate();
    }
}