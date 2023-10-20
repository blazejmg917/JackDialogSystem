using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ChoiceDialogNode))]
public class ChoiceDialogNodeEditor : Editor
{
    public override void OnInspectorGUI(){
        ChoiceDialogNode dn = (ChoiceDialogNode)target;
        base.OnInspectorGUI();
        if(GUILayout.Button("Spawn New Node Choice")){
            dn.SpawnNewNode();
        }
    }
}
