﻿using Covid19.AIBehaviour;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
% – CTRL on Windows / CMD on OSX
# – Shift
& – Alt
LEFT/RIGHT/UP/DOWN – Arrow keys
F1…F2 – F keys
HOME, END, PGUP, PGDN
*/
namespace Covid19.Learn.Custom_Editors.Editor
{
    public class MenuItems : MonoBehaviour
    {
        //%#Q = Q
        [MenuItem("Component/MyComponent _G")]
        static void Functie()
        {
            Debug.Log("called functie");
        }
        [MenuItem("Window/MyWindow")]
        static void MyWindow()
        {

        }
        [MenuItem("Tools/Add new bot configuration #&S")]
        static void AddSomething()
        {

        }
        [MenuItem("CONTEXT/Camera/Camera test #d")]
        static void Ceva()
        {

        }
        [MenuItem("GameObject/Random Instantiate/Cube")]
        static void CubeSpawn()
        {
            Vector3 randomPos = UnityEngine.Random.insideUnitSphere * 100;
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.transform.position = randomPos;
        }
        [MenuItem("Assets/Inspect Script")]
        static void ScriptInspection()
        {

        }
        [MenuItem("CONTEXT/Bot/Randomise Probabilites")]
        static void RandomiseStuff(MenuCommand menuCommand)
        {
            var bot = menuCommand.context as Bot;
        }
    }
}
