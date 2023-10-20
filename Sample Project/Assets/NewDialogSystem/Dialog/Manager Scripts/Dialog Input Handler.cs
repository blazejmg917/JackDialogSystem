using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogInputHandler : MonoBehaviour
{
    [SerializeField, Tooltip("All input actions that can be used to click through dialog")]
    private List<InputActionProperty> dialogContinueActions = new List<InputActionProperty>();
    private bool inputreceived = false;
    [SerializeField, Tooltip("if this can accept input to continue dialog")]
    private bool canAcceptInput = false;

    void Update(){
        
        
            
        foreach(InputActionProperty action in dialogContinueActions){
            
            if(action.action.ReadValue<float>() > .1f){
                
                if(!inputreceived){
                    //Debug.Log("not input received");
                    inputreceived = true;
                    if(canAcceptInput){
                        Debug.Log("handling input");
                        DialogManager.Instance.HandleInput();
                    }
                    
                }
                inputreceived = true;
                return;
            }
        }
        //Debug.Log("not returning for some reason");
        inputreceived = false;
        
        
    }

    /// <summary>
    /// set whether or not input can currently be accepted
    /// </summary>
    /// <param name="accept"></param>
    public void AcceptInput(bool accept){
        Debug.Log("changing input acceptance to " + accept);
        canAcceptInput = accept;
    }
}
