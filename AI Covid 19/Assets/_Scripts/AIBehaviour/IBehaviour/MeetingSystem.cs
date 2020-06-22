using Covid19.Utils;
using UnityEngine;

namespace Covid19.AIBehaviour.Behaviour
{
    public class MeetingSystem
    {
        private AgentNPC _ownerNPC;
        private Transform _transform;

        public MeetingSystem(AgentNPC ownerOwnerNPC)
        {
            _ownerNPC = ownerOwnerNPC;
            _transform = _ownerNPC.transform;
        }
        public AgentNPC FindNPCToMeet()
        {
            foreach (AgentNPC partnerNPC in _ownerNPC.ListAgents)
            {
                if (_ownerNPC != partnerNPC &&
                    AIUtils.CanSeeObject(_transform, partnerNPC.transform, _ownerNPC.viewDist, _ownerNPC.viewAngle) &&
                    partnerNPC.AcceptsMeeting(_ownerNPC))
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