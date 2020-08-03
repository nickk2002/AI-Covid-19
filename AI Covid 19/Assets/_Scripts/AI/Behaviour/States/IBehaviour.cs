using System.Collections;

namespace Covid19.AI.Behaviour.States
{
    public interface IBehaviour
    {
        void Entry();
        void Exit();
        IEnumerator OnUpdate();
    }
}