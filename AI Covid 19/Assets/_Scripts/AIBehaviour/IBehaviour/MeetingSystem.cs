using Covid19.Utils;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace Covid19.AIBehaviour.Behaviour
{
    [Serializable]
    public class MeetingSystem
    {
        private AgentNPC _ownerNPC;
        private Transform _transform;
        
        public float lastMeetingTime = 0; // TODO : don't make it public
        private List<Tuple<AgentNPC, float>> _ignoredAgents = new List<Tuple<AgentNPC, float>>();

        public MeetingSystem(AgentNPC ownerOwnerNPC)
        {
            _ownerNPC = ownerOwnerNPC;
            _transform = _ownerNPC.transform;
        }

        private bool AcceptsMeeting(AgentNPC agentNPC)
        {
            // TODO : Modify to make the list of ignored bots etc.
            
            // check to prevent two agents meeting without having time to turn around and walk away
            if (lastMeetingTime != 0 && !(Time.time - lastMeetingTime > _ownerNPC.cooldownMeeting)) 
                return false;
            
            var found = _ignoredAgents.Find(tuple => tuple.Item1 == agentNPC);
            // if we found an agent ignored and 
            if (found != null)
            {
                // if 10 seconds had not still passed, then we simply cancel the meeting
                if (Time.time - found.Item2 < 10f)
                {
                    Debug.Log("Failed due to ignored list");
                    return false;
                }
                // if 10 seconds passed than remove the bot is no longer ignored
                _ignoredAgents.Remove(found);
            }
            
            // check if the probabilites and the sociable level are satisfed
            int randomValue = UnityEngine.Random.Range(1, 10);
            if (randomValue <= _ownerNPC.sociabalLevel)
            {
                if (AIUtils.CanSeeObject(_transform, agentNPC.transform, agentNPC.viewDist, agentNPC.viewAngle))
                {
                    return true;
                }
            }
            else
            {
                // failed due to probability, than we prevent this two agents to try again, in order to mentain math probability
                _ignoredAgents.Add(new Tuple<AgentNPC,float>(agentNPC,Time.time));
            }

            return false;
        }

        public AgentNPC FindNPCToMeet()
        {
            foreach (AgentNPC partnerNPC in _ownerNPC.ListAgents)
            {
                if (_ownerNPC != partnerNPC &&
                    _ownerNPC.MeetingSystem.AcceptsMeeting(partnerNPC) &&
                    partnerNPC.MeetingSystem.AcceptsMeeting(_ownerNPC))
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
            Vector3 meetingPosition = position + direction - _ownerNPC.offsetMeeting * direction.normalized;
            
            return meetingPosition;
        }
    }
}