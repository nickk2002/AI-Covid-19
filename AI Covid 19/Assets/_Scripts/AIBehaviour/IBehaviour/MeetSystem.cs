using System;
using System.Collections.Generic;
using Covid19.AIBehaviour.Behaviour.States;
using Covid19.Utils;
using UnityEngine;

namespace Covid19.AIBehaviour.Behaviour
{
    [Serializable]
    public class MeetSystem
    {
        private List<Tuple<AgentNPC, int, float>> _ignoredAgents = new List<Tuple<AgentNPC, int, float>>();
        private AgentNPC _npc;
        private Transform _transform;

        public MeetSystem(AgentNPC ownerOwnerNPC)
        {
            _npc = ownerOwnerNPC;
            _transform = _npc.transform;
        }

        public float LastMeetingTime { set; private get; }

        public void Meet(AgentNPC partnerNPC, float talkDuration)
        {
            // set up every thing in order to meet the agent partnerNPC
            Vector3 meetingPosition = GetMeetingPosition(partnerNPC);
            var meetBehaviour = _npc.gameObject.AddComponent<MeetBehaviour>();
            meetBehaviour.MeetPosition = meetingPosition;
            meetBehaviour.partnerNPC = partnerNPC;
            meetBehaviour.talkDuration = talkDuration;
            _npc.SetBehaviour(meetBehaviour);
        }

        private bool AcceptsMeeting(AgentNPC agentNPC)
        {
            if (agentNPC.GetComponent<MeetBehaviour>())
                return false; // if it is already in meeting then return false
            // check to prevent two agents meeting without having time to turn around and walk away
            if (LastMeetingTime != 0 && !(Time.time - LastMeetingTime > _npc.agentConfiguration.cooldownMeeting))
                return false;

            var found = _ignoredAgents.Find(tuple => tuple.Item1 == agentNPC);
            // if we found an agent ignored and 
            if (found != null)
            {
                // if the duration has passed seconds had not still passed, then we simply cancel the meeting
                var ignoreDuration = found.Item2;
                var initialIgnoreTIme = found.Item3;
                if (Time.time - initialIgnoreTIme < ignoreDuration)
                {
                    Debug.Log($"Meeting Failed because {_npc.name} ignored {agentNPC.name}");
                    return false;
                }

                // if the duration in seconds passed(e.g 10 seconds) than remove the bot because is no longer ignored
                _ignoredAgents.Remove(found);
            }

            if (AIUtils.CanSeeObject(_transform, agentNPC.transform,
                AgentManager.Instance.generalConfiguration.viewDistance,
                AgentManager.Instance.generalConfiguration.viewAngle))
                return true;

            return false;
        }

        public void IgnoreAgent(AgentNPC agent, int duration)
        {
            _ignoredAgents.Add(new Tuple<AgentNPC, int, float>(agent, duration, Time.time));
        }

        public AgentNPC FindNPCToMeet()
        {
            foreach (AgentNPC partnerNPC in AgentManager.Instance.agentNPCList)
                if (_npc != partnerNPC &&
                    _npc.MeetSystem.AcceptsMeeting(partnerNPC) &&
                    partnerNPC.MeetSystem.AcceptsMeeting(_npc))
                    return partnerNPC;
            return null;
        }

        private Vector3 GetMeetingPosition(AgentNPC npc)
        {
            Vector3 position = _transform.position;
            Vector3 direction = npc.transform.position - position;
            direction /= 2;
            var meetingDistance = AgentManager.Instance.generalConfiguration.meetingDistance / 2;
            Vector3 meetingPosition = position + direction - meetingDistance * direction.normalized;

            return meetingPosition;
        }
    }
}