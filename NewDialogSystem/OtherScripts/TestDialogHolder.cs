using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDialogHolder : MonoBehaviour
{
    [SerializeField, Tooltip("the dialog tree this will try to run")]private DialogTree dialogTree;
    [SerializeField, Tooltip("if this dialog tree should be run on game start")]private bool runOnStart = true;
    // Start is called before the first frame update
    void Start()
    {
        
        if(!dialogTree){
            dialogTree = GetComponent<DialogTree>();
            
        }
        if(!runOnStart){
            return;
        }
        if(dialogTree){
            DialogNode dialogNode = dialogTree.GetStarterNode();
            if(!dialogNode){
                Debug.LogError("STARTER NODE HAS NOT BEEN SET IN THE DIALOG TREE");
                return;
            }
            //slight delay to give dialog manager time to set up before displaying dialog
            Invoke("DisplayStartDialog",.3f);

        }
        else{
            Debug.LogError("NO DIALOG TREE SET");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisplayStartDialog(){
        DialogManager.Instance.StartDialog(dialogTree.GetStarterNode());
    }
}
