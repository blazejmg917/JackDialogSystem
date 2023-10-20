using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public abstract class DialogNode : MonoBehaviour
{
    public enum DialogType{
        BASIC,
        CHOICE,
        RANDOM,
        CONDITIONAL
    }
    public enum VarChangeTypes{
        ADD,
        SUBTRACT,
        SET_EQUAL
    }
    [Serializable]
    public struct VarChange{
        [Tooltip("The string id of the variable/Quest you want to change")]
        public string varId;
        [Tooltip("what operation you want to apply to this variable/quest status")]
        public VarChangeTypes operationType;
        [Tooltip("the value you want to add/subtract/set equal to")]
        public int changeAmount;
    }

    [Serializable]
    public struct FlagChange{
        [Tooltip("The string id of the flag you want to change")]
        public string flagId;
        [Tooltip("whether you want to set this flag to true or false")]
        public bool flagVal;
    }
    [Serializable]
    public class DialogEvent : UnityEvent{};
    [Header("default dialog node parameters")]
    [SerializeField, Tooltip("The text contained within this dialog node")]
    protected string contents;
    [SerializeField, Tooltip("a special text box this dialog should be displayed in. leave null if not used")]
    protected TMP_Text textBox;
    [SerializeField, Tooltip("a special typing rate (characters/second). leave at 0 or negative if default should be used")]
    protected float charPerSec = -1;
    [SerializeField, Tooltip("whether you can use standard input to move on from this dialog. \nShould always be set to true unless you have another way to continue (like a button)")]
    protected bool continueOnInput = true;
    [Space]
    [SerializeField, Tooltip("mark true if you want this dialog to pass to the next dialog without requiring input once it is finished")]
    protected bool autoContinue = false;
    [SerializeField, Tooltip("how long of a delay there should be before automatically continuing to the next piece of text. If 0 will continue immediately (very hard to read)")]
    private float continueDelayTime = 0;
    [Space]
    [SerializeField, Tooltip("mark this value as true if this node is simply a dummy node that should not get its own text box\nCan be used to change variables/flags or to decide which dialog to show for conditionals/random choices\ndo not use for dialog choices, as the choices will not be displayed and text will stall.")]
    protected bool isDummy;
    [Header("events settings")]
    [SerializeField, Tooltip("events that are run once this dialog is completed. these are fired at the START of dialog" )]
    protected DialogEvent dialogStartEvent = new DialogEvent();
    [SerializeField, Tooltip("events that are run once this dialog is completed. these are fired at the END of dialog ")]
    protected DialogEvent dialogEndEvent = new DialogEvent();
    
    void Start(){
        
    }
    

    

    

    /// <summary>
    /// check if this node is a dummy node and its text display should be skipped
    /// </summary>
    /// <returns></returns>
    public bool IsDummy(){
        return isDummy;
    }
    /// <summary>
    /// get the length of a continue delay
    /// </summary>
    /// <returns>the length this node should delay before continuing</returns>
    public float GetContinueDelay(){
        return continueDelayTime;
    }

    /// <summary>
    /// check if this node should allow you to continue to the next node on input
    /// </summary>
    /// <returns></returns>
    public virtual bool ContinueOnInput(){
        
        return continueOnInput;
    }

    /// <summary>
    /// checks if this node will automatically continue to the next node when completed
    /// </summary>
    /// <returns></returns>
    public bool AutoContinue(){
        return autoContinue;
    }
    
    /// <summary>
    /// determines what node to display next when this node is completed
    /// </summary>
    /// <returns></returns>
    public abstract DialogNode getNextNode();

    /// <summary>
    /// get the text of this dialog node
    /// </summary>
    /// <returns></returns>
    public virtual string getDialog(){
        return contents;
    }

    /// <summary>
    /// get the special text box if there is one
    /// </summary>
    /// <returns></returns>
    public TMP_Text GetSpecialTextBox(){
        return textBox;
    }

    /// <summary>
    /// get the special text speed if there is one
    /// </summary>
    /// <returns></returns>
    public float GetSpecialTextSpeed(){
        return charPerSec;
    }

    /// <summary>
    /// checks if there are any special behaviors at the end of the text scroll
    /// </summary>
    /// <param name="objectsToDisplay">the list of all objects to display at the end of dialog. Only use this variable's contents if the method returns true</param>
    /// <param name="runEvents">whether or not to run events assigned with the end of dialog</param>
    /// <returns>whether there are any objects to display **CURRENTLY ALWAYS FALSE**</returns>
    public virtual bool OnEndTextScroll(out GameObject[] objectsToDisplay, bool runEvents = true){
        objectsToDisplay = null;
        if(runEvents){
            dialogEndEvent.Invoke();
        }
        return false;
    }

    /// <summary>
    /// checks if there are any special behaviors at the start of the text scroll
    /// </summary>
    /// <param name="objectsToDisplay">the list of all objects to display at the start of dialog. Only use this variable's contents if the method returns true</param>
    /// <param name="runEvents">whether or not to run events assigned with the start of dialog</param>
    /// <returns>whether there are any objects to display **CURRENTLY ALWAYS FALSE**</returns>
    public virtual bool OnStartTextScroll(out GameObject[] objectsToDisplay, bool runEvents = true){
        objectsToDisplay = null;
        if(runEvents){
            dialogStartEvent.Invoke();
        }
        return false;
    }

    /// <summary>
    /// checks if the continue text should be displayed. will typically be true for all nodes but choice nodes
    /// </summary>
    /// <returns></returns>
    public virtual bool DisplayContinueText(){
        return true;
    }


    //Editor function
    /// <summary>
    /// Creates a new node of the given type
    /// </summary>
    /// <param name="dialogType">the type of dialog node to create</param>
    /// <returns>the newly created node</returns>
    public DialogNode CreateNewNode(DialogType dialogType){
        switch(dialogType){
            case DialogNode.DialogType.CONDITIONAL:
                return gameObject.AddComponent<ConditionalDialogNode>();
                break;
            case DialogNode.DialogType.CHOICE:
                return gameObject.AddComponent<ChoiceDialogNode>();
                break;
            case DialogNode.DialogType.RANDOM:
                return gameObject.AddComponent<RandomDialogNode>();
                break;
            default:
                return gameObject.AddComponent<BasicDialogNode>();
                break;
        }
    }
}
