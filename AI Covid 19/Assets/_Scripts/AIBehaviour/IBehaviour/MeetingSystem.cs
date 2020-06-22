using Covid19.Utils;
using UnityEngine;

namespace Covid19.AIBehaviour.Behaviour
{
    public class MeetingSystem
    {
        private AgentNPC _ownerNPC;
        private Transform _transform;

        public int ceva;
        public float lastMeetingTime = 0;

        public MeetingSystem(AgentNPC ownerOwnerNPC)
        {
            _ownerNPC = ownerOwnerNPC;
            _transform = _ownerNPC.transform;
        }

        private bool AcceptsMeeting(AgentNPC agentNPC)
        {
            // TODO : Modify to make the list of ignored bots etc.
            if (lastMeetingTime == 0 || (Time.time - lastMeetingTime) > _ownerNPC.cooldownMeeting)
            {
                if (AIUtils.CanSeeObject(_transform, agentNPC.transform, agentNPC.viewDist, agentNPC.viewAngle))
                {
                    return true;
                }
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