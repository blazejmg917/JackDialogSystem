using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RandomDialogNode))]
public class RandomDialogNodeEditor : Editor
{
    public override void OnInspectorGUI(){
        RandomDialogNode dn = (RandomDialogNode)target;
        base.OnInspectorGUI();
        if(GUILayout.Button("Spawn New Node")){
            dn.SpawnNewNode();
        }
    }
}
