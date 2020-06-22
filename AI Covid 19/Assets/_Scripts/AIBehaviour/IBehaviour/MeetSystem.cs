using Covid19.Utils;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace Covid19.AIBehaviour.Behaviour
{
    [Serializable]
    public class MeetSystem
    {
        private AgentNPC _ownerNPC;
        private Transform _transform;

        public float LastMeetingTime { set; private get; }
        private List<Tuple<AgentNPC, float>> _ignoredAgents = new List<Tuple<AgentNPC, float>>();

        public MeetSystem(AgentNPC ownerOwnerNPC)
        {
            _ownerNPC = ownerOwnerNPC;
            _transform = _ownerNPC.transform;
        }

        private bool AcceptsMeeting(AgentNPC agentNPC)
        {
            // check to prevent two agents meeting without having time to turn around and walk away
            if (LastMeetingTime != 0 && !(Time.time - LastMeetingTime > _ownerNPC.meetConfiguration.cooldownMeeting)) 
                return false;
            
            var found = _ignoredAgents.Find(tuple => tuple.Item1 == agentNPC);
            // if we found an agent ignored and 
            if (found != null)
            {
                // if 10 seconds had not still passed, then we simply cancel the meeting
                if (Time.time - found.Item2 < 10f)
                {
                    Debug.Log("Meeting Failed due to ignored list");
                    return false;
                }
                // if 10 seconds passed than remove the bot is no longer ignored
                _ignoredAgents.Remove(found);
            }
            
            // check if the probabilites and the sociable level are satisfed
            int randomValue = UnityEngine.Random.Range(1, 10);
            if (randomValue <= _ownerNPC.meetConfiguration.sociableLevel)
            {
                if (AIUtils.CanSeeObject(_transform, agentNPC.transform, agentNPC.meetConfiguration.viewDist, agentNPC.meetConfiguration.viewAngle))
                {
                    return true;
                }
            }
            else
            {
                // failed due to probability, than we prevent this two agents to try again, in order to mentain math probability
                Debug.Log($"meeting failed due to probability, expected >= {_ownerNPC.meetConfiguration.sociableLevel} but random was {randomValue}");
                _ignoredAgents.Add(new Tuple<AgentNPC,float>(agentNPC,Time.time));
            }

            return false;
        }

        public AgentNPC FindNPCToMeet()
        {
            foreach (AgentNPC partnerNPC in _ownerNPC.ListAgents)
            {
                if (_ownerNPC != partnerNPC &&
                    _ownerNPC.MeetSystem.AcceptsMeeting(partnerNPC) &&
                    partnerNPC.MeetSystem.AcceptsMeeting(_ownerNPC))
                {
                    return partnerNPC;
                }
            }
            return null;
        }

        public Vector3 GetMeetingPosition(AgentNPC npc)
        {
            var position = _transform.position;
            Vector3 direction = npc.transform.position - position;
            direction /= 2;
            Vector3 meetingPosition = position + direction - _ownerNPC.meetConfiguration.offsetMeeting * direction.normalized;
            
            return meetingPosition;
        }
    }
}