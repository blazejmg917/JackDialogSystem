using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicDialogNode : DialogNode
{
    [Header("Basic Dialog Node fields")]
    [SerializeField, Tooltip("the next dialog node. If left empty, will end dialog after this node")]
    private DialogNode nextNode;
    [Space]
    [Header("spawning nodes")]
    [SerializeField, Tooltip("what type of node to spawn")]private DialogNode.DialogType spawnedDialogType;
    [SerializeField, Tooltip("If marked true, will override existing next node. \nIf marked false, can only spawn next node if it doesn't already exist")]private bool overrideExistingNode = false;
    [SerializeField, Tooltip("If marked true, will delete overriden node")]private bool deleteOverriden = false;

    /// <summary>
    /// for a basic node, simply returns the next node in the dialog, or null if this is the last node in dialog
    /// </summary>
    /// <returns>the next node</returns>
    public override DialogNode getNextNode()
    {
        return nextNode;
    }


    public void SpawnNextNode(){
        if(nextNode && !overrideExistingNode){
            Debug.LogError("TRYING TO SPAWN NEXT NODE, BUT IT ALREADY EXISTS, MUST MARK OVERRIDESTARTERNODE TRUE TO OVERRIDE");
            return;
        }
        DialogNode newNode = CreateNewNode(spawnedDialogType);
        

        if(nextNode && deleteOverriden){
            DestroyImmediate(nextNode);
        }

        nextNode = newNode;
        overrideExistingNode = false;
        deleteOverriden = false;
    }
}
