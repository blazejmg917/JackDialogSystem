using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogTree : MonoBehaviour
{
    [SerializeField, Tooltip("the id reference to this dialog tree. Can be used to assign tree to maps for quick storage")]private string dialogTreeId;
    [SerializeField, Tooltip("the starter dialog node for this tree")]private DialogNode starterNode;
    [Space]
    [Header("spawning nodes")]
    [SerializeField, Tooltip("what type of node to spawn")]private DialogNode.DialogType dialogType;
    [SerializeField, Tooltip("If marked true, will override existing starter node. \nIf marked false, can only spawn starter node if it doesn't already exist")]private bool overrideStarterNode = false;
    [SerializeField, Tooltip("If marked true, will delete overriden node")]private bool deleteOverriden = false;
    // Start is called before the first frame update
    public DialogNode GetStarterNode(){
        return starterNode;
    }

    public string GetTreeId(){
        return dialogTreeId;
    }


    //Editor functions

    public void SpawnStarterNode(){
        if(starterNode && !overrideStarterNode){
            Debug.LogError("TRYING TO SPAWN STARTER NODE, BUT IT ALREADY EXISTS, MUST MARK OVERRIDESTARTERNODE TRUE TO OVERRIDE");
            return;
        }
        DialogNode newNode;
        switch(dialogType){
            case DialogNode.DialogType.CONDITIONAL:
                newNode = gameObject.AddComponent<ConditionalDialogNode>();
                break;
            case DialogNode.DialogType.CHOICE:
                newNode = gameObject.AddComponent<ChoiceDialogNode>();
                break;
            case DialogNode.DialogType.RANDOM:
                newNode = gameObject.AddComponent<RandomDialogNode>();
                break;
            default:
                newNode = gameObject.AddComponent<BasicDialogNode>();
                break;
        }

        if(starterNode && deleteOverriden){
            DestroyImmediate(starterNode);
        }

        starterNode = newNode;
        overrideStarterNode = false;
        deleteOverriden = false;
    }
}
