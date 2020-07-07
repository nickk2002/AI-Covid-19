using System.Collections;

namespace Covid19.AIBehaviour.Behaviour
{
    public interface IBehaviour
    {
        void Enable();
        void Disable();
        IEnumerator OnUpdate();
    }
}