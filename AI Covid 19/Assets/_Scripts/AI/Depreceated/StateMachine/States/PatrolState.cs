using UnityEngine;
using UnityEngine.AI;

namespace Covid19.AI.Depreceated.StateMachine.States
{
    public class PatrolState : FSMState
    {
        public PatrolState(FiniteStateMachine finiteStateMachine) : base(finiteStateMachine)
        {
        }

        private bool _startPatroling;
        private int _currentIndexPatrol = 0;

        private Vector3 RandomLoc()
        {
            //locatie random pentru mapa mare.de 1000 * 1000 Scena: AISimulations
            NavMeshHit hit;
            for (var i = 0; i < 30; i++)
            {
                var randomPosition = new Vector3(Random.Range(-CurrentBot.RandomRange, CurrentBot.RandomRange), 0,
                    Random.Range(-CurrentBot.RandomRange, CurrentBot.RandomRange));
                if (NavMesh.SamplePosition(randomPosition, out hit, 1, NavMesh.AllAreas)) return hit.position;
            }

            return new Vector3(Random.Range(-CurrentBot.RandomRange, CurrentBot.RandomRange), 0,
                Random.Range(-CurrentBot.RandomRange, CurrentBot.RandomRange));
        }

        public override void Update()
        {
            if (Agent.isOnNavMesh == false)
                Debug.LogError("nu este pe navmesh");
            if (Agent.isStopped)
                Agent.isStopped = false;
            if (Agent.enabled == false)
                Agent.enabled = true;
            // in caz ca agentul intra in stare de patrol iar agent este oprit/dezactivat
            // aleg destinatia
            // folosesc startPatroling pentru a putea sa intru in starea de patrol la prima iteratie
            if (_startPatroling == false ||
                Vector3.Distance(Transform.position, CurrentDestination) < CurrentBot.StoppingDistance)
            {
                if (CurrentBot.RandomLocations) // aleg pozitii random
                {
                    var randomLocation =
                        RandomLoc(); // o functie care returneaza o pozitie random valida in pe baza RandomRange
                    CurrentDestination = randomLocation;
                    // folosesc currentDestination care este de tip Vector3 pentru a retine mereu care este destinatie curenta
                    // uneori agent.destination nu imi spune care este exact destinatia
                    Agent.SetDestination(CurrentBot.CurrentDestination);
                }
                else
                {
                    // aleg pozitiile din patrolPositions care este setat in inspector
                    //agent.SetDestination(bot.patrolPositions[currentIndexPatrol].transform.position);
                    CurrentDestination =
                        CurrentBot.PatrolPositions[_currentIndexPatrol].transform
                            .position; // nu uit sa pun currentDestination;
                    _currentIndexPatrol++; // cresc indexul din vector
                    if (_currentIndexPatrol >= CurrentBot.PatrolPositions.Length)
                        _currentIndexPatrol = 0; // resetez indexul
                }

                _startPatroling = true;
            }
        }
    }
}