using System;
using System.Collections.Generic;
using Covid19.AIBehaviour.Behaviour.States;
using Covid19.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Covid19.AIBehaviour.Behaviour
{
    [Serializable]
    public class MeetSystem
    {
        private List<Tuple<AgentNPC, float>> _ignoredAgents = new List<Tuple<AgentNPC, float>>();
        private AgentNPC _ownerNPC;
        private Transform _transform;

        public MeetSystem(AgentNPC ownerOwnerNPC)
        {
            _ownerNPC = ownerOwnerNPC;
            _transform = _ownerNPC.transform;
        }

        public int TalkAnimationID { get; private set; }
        public float TalkDuration { get; private set; }
        public float LastMeetingTime { set; private get; }

        private bool AcceptsMeeting(AgentNPC agentNPC)
        {
            if (agentNPC.GetComponent<MeetBehaviour>())
                return false; // if it is already in meeting then return false
            // check to prevent two agents meeting without having time to turn around and walk away
            if (LastMeetingTime != 0 && !(Time.time - LastMeetingTime > _ownerNPC.agentConfiguration.cooldownMeeting))
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
            var randomValue = Random.Range(1, 10);
            if (randomValue <= _ownerNPC.agentConfiguration.sociableLevel)
            {
                if (AIUtils.CanSeeObject(_transform, agentNPC.transform,
                    NPCManager.Instance.generalConfiguration.viewDistance,
                    NPCManager.Instance.generalConfiguration.viewAngle))
                    return true;
            }
            else
            {
                // failed due to probability, than we prevent this two agents to try again, in order to mentain math probability
                Debug.Log(
                    $"meeting failed due to probability, expected <= {_ownerNPC.agentConfiguration.sociableLevel} but random was {randomValue}");
                _ignoredAgents.Add(new Tuple<AgentNPC, float>(agentNPC, Time.time));
            }

            return false;
        }

        public AgentNPC FindNPCToMeet()
        {
            foreach (AgentNPC partnerNPC in NPCManager.Instance.agentNpcs)
                if (_ownerNPC != partnerNPC &&
                    _ownerNPC.MeetSystem.AcceptsMeeting(partnerNPC) &&
                    partnerNPC.MeetSystem.AcceptsMeeting(_ownerNPC))
                    return partnerNPC;
            return null;
        }

        public void SetTalkDuration(AgentNPC partnerNpc, float duration)
        {
            TalkAnimationID = Random.Range(0, 3);
            while (TalkAnimationID == partnerNpc.MeetSystem.TalkAnimationID) TalkAnimationID = Random.Range(0, 3);

            Debug.Log($"Talk Animation ID is {TalkAnimationID}");
        }

        public Vector3 GetMeetingPosition(AgentNPC npc)
        {
            Vector3 position = _transform.position;
            Vector3 direction = npc.transform.position - position;
            direction /= 2;
            float meetingDistance = NPCManager.Instance.generalConfiguration.meetingDistance / 2;
            Vector3 meetingPosition = position + direction - meetingDistance * direction.normalized;

            return meetingPosition;
        }
    }
}