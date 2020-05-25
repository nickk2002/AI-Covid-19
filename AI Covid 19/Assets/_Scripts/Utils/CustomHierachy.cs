using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class CustomHierarchy : MonoBehaviour
{

    private static Texture botIcon = Resources.Load("Bot_icon") as Texture;
    private static Texture botIconSelected = Resources.Load("Bot_icon_selected") as Texture;
    private static Texture meetingPoint = Resources.Load("meet") as Texture;


    static CustomHierarchy()
    {
        EditorApplication.hierarchyWindowItemOnGUI += HandleHierarchyWindowItemOnGUI;
    }

    [MenuItem("GameObject/Set Meeting Point",false,priority = -100)]
    static void Functie()
    {
        
        Debug.Log("entered in functie");
        SetIcon(Selection.activeGameObject, meetingPoint);
    }
    static void SetIcon(GameObject go,Texture texture)
    {
        var editorGUIUtilityType = typeof(EditorGUIUtility);
        var bindingFlags = BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod;
        //texture.;
        var args = new object[] { go, texture };
        editorGUIUtilityType.InvokeMember("SetIconForObject", bindingFlags, null, null, args);
    }

    private static void HandleHierarchyWindowItemOnGUI(int instanceId, Rect selectionRect)
    {
        var obj = EditorUtility.InstanceIDToObject(instanceId);
        GameObject go = EditorUtility.InstanceIDToObject(instanceId) as GameObject;
        
        if (obj != null)
        {
            if (go.GetComponent<Bot>())
            {
                Texture botTexture = botIcon;
                Rect position = new Rect(selectionRect);
                position.x -= 1;
                position.y -= 1;
                position.size = new Vector2(18, 18);
                Color backgroundColor = new Color(194, 194, 194);
                if (Selection.instanceIDs.Contains(instanceId))
                {
                    botTexture = botIconSelected;
                }
                EditorGUI.DrawRect(position, backgroundColor);
                GUIStyle style = new GUIStyle();
                GUI.Box(position, botTexture, style);
            }
        }
    }
    static private Texture2D CreateTextureFromColor(int width,int height,Color color)
    {
        Color[] pixels = new Color[width * height];
        for (int i = 0; i < width * height; i++)
            pixels[i] = color;
        Texture2D texture = new Texture2D(width, height);
        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }
}