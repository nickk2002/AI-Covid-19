using System;
using UnityEngine;

namespace Covid19.AIBehaviour.Actions
{
    [Serializable]
    public class BotAction : IEquatable<BotAction>
    {
        public float probability;
        public float stopDistance;
        public Transform targetTransform;
        public Vector3 position;
        public Quaternion rotation;
        public string name;
        public Place place;

        public BotAction()
        {
        }

        public BotAction(BotAction action)
        {
            probability = action.probability;
            stopDistance = action.stopDistance;
            targetTransform = action.targetTransform;
            position = action.position;
            rotation = action.rotation;
            name = action.name;
            place = action.place;
        }

        public bool Equals(BotAction other)
        {
            if (name == other.name && position == other.position && place == other.place)
            {
                return true;
            }
            else
            {
                if (place == other.place) Debug.Log($"failed {name} {other.name} {place} {other.place}");
                return false;
            }
        }
    }
}