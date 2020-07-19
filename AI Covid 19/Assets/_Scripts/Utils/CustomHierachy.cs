using System;
using System.Linq;
using System.Reflection;
using Covid19.AI;
using Covid19.AI.Behaviour;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Covid19.Utils
{
    [InitializeOnLoad]
    public class CustomHierarchy : MonoBehaviour
    {
        private static readonly Texture BotIcon = Resources.Load("Bot_icon") as Texture;
        private static readonly Texture BotIconSelected = Resources.Load("Bot_icon_selected") as Texture;
        public static readonly Texture MeetingPointIcon = Resources.Load("meet") as Texture;
        private int _nic;


        static CustomHierarchy()
        {
            EditorApplication.hierarchyWindowItemOnGUI += HandleHierarchyWindowItemOnGUI;
        }

        [MenuItem("GameObject/Set Meeting Point", false, priority = -100)]
        private static void SetMeetingPoint()
        {
            Debug.Log("entered in functie");
            SetIcon(Selection.activeGameObject, MeetingPointIcon);
        }

        [MenuItem("GameObject/Position Holder", false, priority = -100)]
        private static void SetUpPositionHolder()
        {
            var npc = Selection.activeGameObject.GetComponent<AgentNPC>();
            if (npc)
            {
                var posHolder = new GameObject();
                Transform npcTransform = npc.transform;
                posHolder.transform.position = npcTransform.position;
                posHolder.name = "Positions " + npc.name;
                posHolder.transform.SetParent(npcTransform.parent);
                npc.posHolder = posHolder;

                for (var i = 1; i <= 3; i++)
                {
                    var emptypos = new GameObject();
                    emptypos.transform.position = npc.transform.position;
                    emptypos.transform.SetParent(posHolder.transform);
                }
            }
        }

        public static void SetIcon(GameObject go, Texture texture)
        {
            Type editorGUIUtilityType = typeof(EditorGUIUtility);
            BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public |
                                        BindingFlags.InvokeMethod;
            //texture.;
            var args = new object[] {go, texture};
            editorGUIUtilityType.InvokeMember("SetIconForObject", bindingFlags, null, null, args);
        }

        private static void HandleHierarchyWindowItemOnGUI(int instanceId, Rect selectionRect)
        {
            Object obj = EditorUtility.InstanceIDToObject(instanceId);
            var go = EditorUtility.InstanceIDToObject(instanceId) as GameObject;

            if (obj != null)
                if (go.GetComponent<Bot>())
                {
                    Texture botTexture = BotIcon;
                    var position = new Rect(selectionRect);
                    position.size = new Vector2(17, 17);
                    var backgroundColor = new Color(194, 194, 194);
                    if (Selection.instanceIDs.Contains(instanceId)) botTexture = BotIconSelected;
                    EditorGUI.DrawRect(position, backgroundColor);
                    var style = new GUIStyle();
                    GUI.Box(position, botTexture, style);
                }
        }

        private static Texture2D CreateTextureFromColor(int width, int height, Color color)
        {
            var pixels = new Color[width * height];
            for (var i = 0; i < width * height; i++)
                pixels[i] = color;
            var texture = new Texture2D(width, height);
            texture.SetPixels(pixels);
            texture.Apply();
            return texture;
        }
    }
}