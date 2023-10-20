using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoiceDialogNode : DialogNode
{
    [Serializable]
    public struct DialogChoice{
        [Tooltip("the text that will be displayed for this dialog option")]
        public string choiceText;
        [Tooltip("the dialog node that will be met after choosing this node")]
        public DialogNode choiceNode;
    }
    [Header("Choice-based dialog node settings")]
    [SerializeField, Tooltip("the list of all dialog choices to be displayed after this dialog node. \nBe sure that continue on input has been disabled for these to be of use")]
    private List<DialogChoice> dialogChoices;
    [SerializeField, Tooltip("the dialog choice that has been selected by the player")]
    private DialogNode dialogChoiceNode = null;
    [SerializeField, Tooltip("will normally automatically set continue on click to false. mark this false to override that")]
    private bool overrideContinueOnClick = true;
    [Space]
    [Header("spawning nodes")]
    [SerializeField, Tooltip("what type of node to spawn")]private DialogNode.DialogType spawnedDialogType;
    [SerializeField, Tooltip("the text that should be added to this dialog's option's button")]private string spawnedChoiceText;
    [SerializeField, Tooltip("If marked true, can make multiple nodes with the same test. \nIf marked false, can only spawn next node with unique text")]private bool allowDuplicateChoice = false;
    [SerializeField, Tooltip("If marked true, can make node with empty string")]private bool allowEmptyChoice = false;
    [SerializeField, Tooltip("If marked true and duplicate nodes are allowed, will delete duplicate node")]private bool deleteDuplicate = false;

    // Start is called before the first frame update
    void Awake()
    {
        if(dialogChoices == null){
            dialogChoices = new List<DialogChoice>();
        }
        if(overrideContinueOnClick){
            continueOnInput = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override bool ContinueOnInput(){
        return continueOnInput && !overrideContinueOnClick;
    }

    /// <summary>
    /// for a choice node, returns whichever dialog node has been selected in dialog
    /// </summary>
    /// <returns>the dialog choice that was associated with the selected dialog response</returns>
    public override DialogNode getNextNode()
    {
        return dialogChoiceNode;
    }

    /// <summary>
    /// choose the given dialog node as the next node in dialog
    /// </summary>
    /// <param name="node">the dialog node to set as the next node</param>
    public void ChooseNode(DialogNode node){
        dialogChoiceNode = node;
    }

    /// <summary>
    /// pick a dialog choice from a button choice, also calls the event to end this current dialog node and move onto the next
    /// </summary>
    /// <param name="choice">the dialog choice associated with the button that was clicked</param>
    public void PickDialogChoice(DialogNode choice){
        ChooseNode(choice);
        dialogEndEvent.Invoke();
        //notify dialog manager that a choice has been made
    }

    /// <summary>
    /// return a list of all possible dialog choices
    /// </summary>
    /// <returns>all choices</returns>
    public List<DialogChoice> getChoices(){
        return dialogChoices;
    }

    /// <summary>
    /// for a choice dialog node, displays the dialog choices, then potentially still runs any events associated with this dialog node's end
    /// </summary>
    /// <param name="objectsToDisplay">returns any ui objects that should be displayed at the end of this dialog, if there are any. Only use this field's contents if the method returns true </param>
    /// <param name="runEvents">whether or not any events should be run at the end of this node</param>
    /// <returns>whether or not there are any ui objects to display</returns>
    public override bool OnEndTextScroll(out GameObject[] objectsToDisplay, bool runEvents = false)
    {
        DialogManager.Instance.DisplayChoices(dialogChoices, this);
        return base.OnEndTextScroll(out objectsToDisplay, runEvents);
    }
    /// <summary>
    /// for a choice node, the continue text will never be displayed
    /// </summary>
    /// <returns>always false</returns>
    public override bool DisplayContinueText(){
        return false;
    }

    public void SpawnNewNode(){
        if(!allowEmptyChoice && spawnedChoiceText == ""){
            Debug.LogError("TRYING TO SPAWN NEW EMPTY CHOICE NODE, MUST MARK ALLOWEMPTYCHOICE TRUE TO DO SO");
            return;
        }

        DialogNode oldNode = null;
        DialogChoice oldChoice = new DialogChoice();
        foreach(DialogChoice choice in dialogChoices){
                if(choice.choiceText == spawnedChoiceText){
                    oldNode = choice.choiceNode;
                    oldChoice = choice;
                }
            }

        if(!allowDuplicateChoice && oldNode){
            Debug.LogError("TRYING TO SPAWN NEW CHOICE NODE, BUT CHOICE " + spawnedChoiceText + " ALREADY EXISTS, MUST MARK ALLOWDUPLICATECHOICE TRUE TO ALLOW");
            return;
            
        }
        DialogNode newNode = CreateNewNode(spawnedDialogType);
        

        if(oldNode && deleteDuplicate){
            DestroyImmediate(oldNode);
            dialogChoices.Remove(oldChoice);
        }

        DialogChoice newChoice = new DialogChoice();
        newChoice.choiceText = spawnedChoiceText;
        newChoice.choiceNode = newNode;
        dialogChoices.Add(newChoice);
        allowDuplicateChoice = false;
        allowEmptyChoice = false;
        deleteDuplicate = false;
    }
}
