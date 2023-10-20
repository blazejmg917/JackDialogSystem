using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ConditionalDialogNode))]
public class ConditionalDialogNodeEditor : Editor
{
    public override void OnInspectorGUI(){
        ConditionalDialogNode dn = (ConditionalDialogNode)target;
        base.OnInspectorGUI();
        if(GUILayout.Button("Spawn New Node")){
            dn.SpawnNewNode();
        }
    }
}
