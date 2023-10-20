using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class DialogManager : MonoBehaviour
{
    [System.Serializable]
    public class DialogStartEvent : UnityEvent{};
    [System.Serializable]
    public class DialogEndEvent : UnityEvent{};
    [Header("Other important script components")]
    [SerializeField, Tooltip("the typewriter component. Responsible for actually writing the text")]
    private Typewriter typewriter;
    [SerializeField, Tooltip("the input handler component. Responsible for handling input to continue dialog")]
    private DialogInputHandler inputHandler;
    [SerializeField, Tooltip("the variable handler component. Responsible for handling conditional dialog")]
    private DialogVarsHandler varsHandler;
    public DialogVarsHandler VarsHandler{
        get{
            return varsHandler;
        }
    }
    
    [Header("dialog UI Objects")]
    [Tooltip("the overarching game object for dialog windows. used to turn on/off display")]
    public GameObject dialogObject;
    [Tooltip("Text box for dialog")]
    public TMP_Text dialogText;
    [Tooltip("Text box to say to press button to continue")]
    public TMP_Text continueText;
    [Space]
    [Tooltip("Text box for speaker name")]
    public TMP_Text speakerNameText;
    [Tooltip("Panel for speaker name")]
    public GameObject speakerNamePanel;
    [Tooltip("The panel that will hold any dialog choice buttons")]
    public GameObject choicePanel;
    [Tooltip("the base button to use for creating dialog choices")]
    public GameObject choiceButton;
    [SerializeField, Tooltip("any currently displayed extra objects")]
    public List<GameObject> displayedObjects = new List<GameObject>();
    [Header("dialog parameters")]
    [SerializeField, Tooltip("the current dialog node being processed. Here for editor reference only")]
    private DialogNode currentDialog;
    [SerializeField, Tooltip("if dialog is currently being displayed")]
    private bool displayingDialog = false;
    [SerializeField, Tooltip("if the current dialog is being used still")]
    private bool currentDialogRunning = false;
    [SerializeField, Tooltip("if the dialog manager is currently handling input and should not try to deal with any further input for the moment")]
    private bool handlingInput = false;
    [SerializeField, Tooltip("if the dialog manager can recieve any input currently")]
    private bool canTakeInput = false;

    private DialogNode starterDialog = null;

    [SerializeField, Tooltip("The event to play when starting dialog")]
    private DialogStartEvent dialogStart = new DialogStartEvent();
    [SerializeField, Tooltip("The event to play when starting dialog")]
    private DialogEndEvent dialogEnd = new DialogEndEvent();


    private IEnumerator waitDelayCoroutine;


    //for singleton
    private static DialogManager instance;

    public static DialogManager Instance
    {
        get{
            if(instance == null){
                instance = FindObjectOfType<DialogManager>();
                if(instance == null){
                    GameObject obj = new GameObject();
                    instance = obj.AddComponent<DialogManager>();
                }
            }
            return instance;
        }
    }

    /// <summary>
    /// Updates the given variable. Quick reference to Instance.VarsHandler.UpdateVariable
    /// </summary>
    /// <param name="variable">the id of the variable to update</param>
    /// <param name="value">the value to set for the variable</param>
    /// <param name="onlyUpdateExisting">will only allow existing variables to be updated, prevents new variable creation</param>
    /// <returns>false if failed to update variable</returns>
    public static bool UpdateVariable(string variable, int value, bool onlyUpdateExisting = false){
        return Instance.VarsHandler.UpdateVariable(variable,value,onlyUpdateExisting);
    }
    /// <summary>
    /// tries to get a variable. Quick reference to Instance.VarsHandler.GetVariable
    /// </summary>
    /// <param name="variable">the id of the variable to get</param>
    /// <param name="value">the return value of the variable</param>
    /// <returns>whether the variable existed or not</returns>
    public static bool GetVariable(string variable, out int value){
        return Instance.VarsHandler.GetVariable(variable, out value);
    }
    /// <summary>
    /// Updates the given flag. Quick reference to Instance.VarsHandler.UpdateFlag
    /// </summary>
    /// <param name="flag">the id of the flag to update</param>
    /// <param name="value">the value to set for the variable</param>
    /// <param name="onlyUpdateExisting">will only allow existing flags to be updated, prevents new flag creation</param>
    /// <returns>false if failed to update flag</returns>
    public static bool UpdateFlag(string flag, bool value, bool onlyUpdateExisting = false){
        return Instance.VarsHandler.UpdateFlag(flag,value,onlyUpdateExisting);
    }
    /// <summary>
    /// tries to get a flag. Quick reference to Instance.VarsHandler.GetFlag
    /// </summary>
    /// <param name="flag">the id of the flag to get</param>
    /// <param name="value">the return value of the flag</param>
    /// <returns>whether the flag existed or not</returns>
    public static bool GetFlag(string flag, out bool value){
        return Instance.VarsHandler.GetFlag(flag, out value);
    }


    // Start is called before the first frame update
    void Start()
    {
        if(!typewriter){
            typewriter = GetComponent<Typewriter>();
            if(!typewriter){
                typewriter = gameObject.AddComponent<Typewriter>();
                Debug.LogWarning("NO TYPEWRITER COMPONENT FOUND IN DIALOG MANAGER. CREATING NEW TYPEWRITER COMPONENT, BUT SOME VALUES MAY BE INCORRECT. \nPLEASE ADD A TYPEWRITER COMPONENT TO THE DIALOG MANAGER");
            }
        }
        if(!typewriter.TextBoxSet()){
            typewriter.setDefaultTextbox(dialogText);
        }
        continueText.gameObject.SetActive(false);
        if(speakerNamePanel){
            speakerNamePanel.SetActive(false);
        }
        if(dialogObject){
            dialogObject.SetActive(false);
        }
        if(!inputHandler){
            inputHandler = gameObject.GetComponent<DialogInputHandler>();
            if(!inputHandler){
                Debug.LogError("DIALOG MANAGER HAS NO INPUT HANDLER");
            }
        }
        if(!varsHandler){
            varsHandler = gameObject.GetComponent<DialogVarsHandler>();
            if(!varsHandler){
                Debug.LogError("DIALOG MANAGER HAS NO VARIABLE HANDLER");
            }
        }
    }



    /// <summary>
    /// Start a dialog from a particular dialog node. Only to be called at the very beginning of dialog
    /// </summary>
    /// <param name="thisDialog">the starting node for this dialog</param>
    /// <param name="speakerName">the name of the speaker to display</param>
    /// <returns>whether or not this dialog is succesfully displayed</returns>
    public bool StartDialog(DialogNode thisDialog, string speakerName){
        speakerNameText.text = speakerName;
        speakerNamePanel.SetActive(true);
        return StartDialog(thisDialog);
    }

    /// <summary>
    /// Start a dialog from a particular dialog node. Only to be called at the very beginning of dialog
    /// </summary>
    /// <param name="thisDialog">the starting node for this dialog</param>
    /// <returns>whether or not this dialog is succesfully displayed</returns>
    public bool StartDialog(DialogNode thisDialog){
        if(currentDialog){
            Debug.Log("already displaying dialog");
            return false;
        }
        if(!thisDialog){
            Debug.LogWarning("Dialog is null, cancelling call");
            return false;
        }
        dialogStart.Invoke();
        Debug.Log("dialog manager has received dialog");
        displayingDialog = true;
        dialogObject.SetActive(true);
        inputHandler.AcceptInput(true);
        HandleDialog(thisDialog);
        return true;
    }

    /// <summary>
    /// Handle a particular dialog node. Should be called once per node, as soon as the node becomes the current node to display.
    /// </summary>
    /// <param name="thisDialog">the next dialog to display. Will end dialog if this is null</param>
    private void HandleDialog(DialogNode thisDialog){
        //hide the continue dialog text
        continueText.gameObject.SetActive(false);
        
        currentDialog = thisDialog;
        if(thisDialog == null){
            Debug.Log("end of dialog reached");
            EndDialog();
            return;
        }
        
        if(!gameObject.activeSelf){
            EndDialog();
            Debug.LogWarning("dialog manager has been deactivated, cancelling dialog");
            return;
        }
        //if this dialog is a dummy node, then it has no dialog to display and should be skipped. Handle the next node
        if(thisDialog.IsDummy()){
            HandleDialog(thisDialog.getNextNode());
            return;
        }
        //display the dialog's text
        StartCoroutine("DisplayDialog");

    }

    /// <summary>
    /// Immediately move on to the next dialog node
    /// </summary>
    private void GoToNextDialog(){
        currentDialogRunning = true;
        ClearDisplayedObjects();
        HandleDialog(currentDialog.getNextNode());

    }

    /// <summary>
    /// display the dialog held by the current dialog node through the typewriter text system
    /// </summary>
    /// <returns></returns>
    IEnumerator DisplayDialog(){
        currentDialogRunning = true;
        //if there is no current dialog node, don't try to display anything
        if(currentDialog == null){
            yield break;
        }
        //check if there are any start of dialog node display objects
        GameObject[] startDisplays;
        if(currentDialog.OnStartTextScroll(out startDisplays)){
            //TODO; Display start displays
            Debug.LogWarning("start displays not yet handled");
        }
        canTakeInput = true;
        //start the typewriter system and wait for it to complete
        yield return StartCoroutine(typewriter.Typewrite(currentDialog.GetSpecialTextBox(), currentDialog.getDialog(), currentDialog.GetSpecialTextSpeed()));

        //check if there are any end of dialog node display objects
        GameObject[] endDisplays;
        if(currentDialog.OnEndTextScroll(out endDisplays)){
            //TODO; Display end displays
            Debug.LogWarning("start displays not yet handled");
        }
        currentDialogRunning = false;
        //if the current dialog is set to continue automatically without waiting for player input, move on to the next dialog node
        if(currentDialog && currentDialog.AutoContinue()){
            if(currentDialog.GetContinueDelay() > 0){
                waitDelayCoroutine = WaitDelay(currentDialog.GetContinueDelay());
                canTakeInput = true;
                yield return waitDelayCoroutine;
                waitDelayCoroutine = null;
            }
            GoToNextDialog();
        }
        else if (!currentDialog){
            Debug.LogWarning("running dialog changed to null, cancelling dialog");
            EndDialog();
        }
        //else, display the continue text option if applicable, and mark input as acceptable
        else{
            canTakeInput = true;
            if(currentDialog && currentDialog.DisplayContinueText()){
                continueText.gameObject.SetActive(true);
            }
        }
    }

    private IEnumerator WaitDelay(float delay){
        while(delay > 0){
            delay -= Time.deltaTime;
            yield return null;
        }
    }


    /// <summary>
    /// ends the current dialog as it reaches its natural end
    /// </summary>
    public void EndDialog(bool invokeEvents = true){
        currentDialog = null;
        displayingDialog = false;
        handlingInput = false;
        currentDialogRunning = false;
        ClearDisplayedObjects();
        dialogObject.SetActive(false);
        inputHandler.AcceptInput(false);
        if(invokeEvents){
            dialogEnd.Invoke();
        }
        if(speakerNamePanel){
            speakerNamePanel.SetActive(false);
        }
    }

    /// <summary>
    /// cancels the current dialog midway through and closes the dialog window. requires a few extra steps, but should be safe. use more sparingly
    /// </summary>
    
    public void CancelDialog(){
        typewriter.StopText();
        StopAllCoroutines();
        EndDialog();
    }

    /// <summary>
    /// Clear all current display objects
    /// </summary>
    public void ClearDisplayedObjects(){
        if(displayedObjects == null){
            return;
        }
        for(int i = displayedObjects.Count - 1; i >= 0; i--){
            GameObject obj = displayedObjects[i];
            displayedObjects.Remove(obj);
            Destroy(obj);
        }
    }

    /// <summary>
    /// handle incoming input during dialog
    /// </summary>
    public void HandleInput(){
        if(handlingInput){
            return;
        }
        handlingInput = true;
        if(displayingDialog && canTakeInput){
            //if the text is still being printed, have the typewriter skip typing and instantly display all text
            if(currentDialogRunning){
                Debug.Log("skipping text");
                typewriter.SkipText();
                canTakeInput = false;
            }
            //else, move on to the next dialog node
            else{
                if(currentDialog.ContinueOnInput()){
                    if(currentDialog.AutoContinue() && waitDelayCoroutine != null){
                        StopCoroutine(waitDelayCoroutine);
                    }
                    Debug.Log("continuing to next dialog");
                    canTakeInput = false;
                    GoToNextDialog();
                }
            }
        }
        handlingInput = false;
    }

    /// <summary>
    /// Called when a dialog choice is made. Continues to the next dialog node
    /// </summary>
    /// <param name="node"></param>
    public void MakeDialogChoice(DialogNode node){
        GoToNextDialog();
    }

    /// <summary>
    /// Display all dialog choices for a choice node
    /// </summary>
    /// <param name="choices">the list of all choices to display</param>
    /// <param name="node">the choice node that holds these choices</param>
    public void DisplayChoices(List<ChoiceDialogNode.DialogChoice> choices, ChoiceDialogNode node){
        continueText.gameObject.SetActive(false);
        //for each choice, make a button with that choice assigned to it to display to the player
        foreach(ChoiceDialogNode.DialogChoice choice in choices){
            GameObject choiceObj = Instantiate(choiceButton, choicePanel.transform);
            choiceObj.GetComponent<DialogChoiceButton>().AssignChoice(choice.choiceNode, node, choice.choiceText);
            displayedObjects.Add(choiceObj);
            choiceObj.SetActive(true);
        }
    }

    /// <summary>
    /// returns if the dialog manager is currently busy displaying input
    /// </summary>
    /// <returns></returns>
    public bool IsBusy(){
        return currentDialog;
    }
}
