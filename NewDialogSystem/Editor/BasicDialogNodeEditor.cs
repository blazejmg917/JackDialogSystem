using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BasicDialogNode))]
public class BasicDialogNodeEditor : Editor
{
   public override void OnInspectorGUI(){
        BasicDialogNode dn = (BasicDialogNode)target;
        base.OnInspectorGUI();
        if(GUILayout.Button("Spawn Next Node")){
            dn.SpawnNextNode();
        }
    }
}
