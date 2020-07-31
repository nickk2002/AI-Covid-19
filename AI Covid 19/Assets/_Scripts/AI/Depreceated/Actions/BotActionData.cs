using UnityEngine;

namespace Covid19.AI.Depreceated.Actions
{
    [System.Serializable]
    public class BotActionData
    {
        public float[] targetTransformPosition;
        public float[] relativePosition;
        public float[] rotation;
        public string name;
        public Place place;

        public BotActionData(BotAction action)
        {
            targetTransformPosition = new float[3];
            if (action.targetTransform)
            {
                targetTransformPosition[0] = action.targetTransform.position.x;
                targetTransformPosition[1] = action.targetTransform.position.y;
                targetTransformPosition[2] = action.targetTransform.position.z;
            }

            relativePosition = new float[3];
            relativePosition[0] = action.position.x;
            relativePosition[1] = action.position.y;
            relativePosition[2] = action.position.z;

            rotation = new float[4];
            rotation[0] = action.rotation.x;
            rotation[1] = action.rotation.y;
            rotation[2] = action.rotation.z;
            rotation[3] = action.rotation.w;

            name = action.name;
            place = action.place;
        }

        public BotAction ConvertToBotAction()
        {
            var action = new BotAction();
            if (action.targetTransform)
                action.targetTransform.position = new Vector3(targetTransformPosition[0], targetTransformPosition[1],
                    targetTransformPosition[2]);
            action.position = new Vector3(relativePosition[0], relativePosition[1], relativePosition[2]);
            action.rotation = new Quaternion(rotation[0], rotation[1], rotation[2], rotation[3]);
            action.name = name;
            action.place = place;
            return action;
        }
    }
}