using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DialogTree))]
public class DialogTreeEditor : Editor
{
    public override void OnInspectorGUI(){
        DialogTree dt = (DialogTree)target;
        base.OnInspectorGUI();
        if(GUILayout.Button("Spawn Starter Node")){
            dt.SpawnStarterNode();
        }
    }
}
