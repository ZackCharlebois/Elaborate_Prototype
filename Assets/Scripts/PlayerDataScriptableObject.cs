using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "Game Data/PlayerData")]
public class PlayerDataScriptableObject : ScriptableObject
{
    public int playerHpMax;
    public int playerHp;
    public int playerSlimeCount;
    public int entranceNumber = -1; //entrance number Player will arrive at
    
    /*
#if UNITY_EDITOR
    [CustomEditor(typeof(PlayerDataScriptableObject))]
    public class PlayerDataScriptableObjectInspector : Editor
    {
        [SerializeField] private bool test = false;
        public override void OnInspectorGUI()
        {
            this.DrawDefaultInspector ();
 
            GUILayout.Space (25);
 
            GUILayout.BeginHorizontal();
            if (test)
            {
                GUI.color = new Color( 1.0f, 0.5f, 0.5f);
                if(GUILayout.Button("Clear Instantiated Prefabs"))
                {
                    test = false;
                }
            }
            else
            {
                GUI.color = new Color( 0.5f, 1.0f, 0.5f);
                if(GUILayout.Button("Instantiate Prefabs Above"))
                {
                    test = true;
                }
            }
            GUILayout.EndHorizontal();
        }
    }
#endif
*/

}
