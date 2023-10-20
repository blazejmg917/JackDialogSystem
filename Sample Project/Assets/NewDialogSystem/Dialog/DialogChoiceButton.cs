using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogChoiceButton : MonoBehaviour
{
    [SerializeField, Tooltip("the dialog choice that will be selected when this button is checked")]
    private DialogNode dialogChoice;
    [SerializeField, Tooltip("the dialog node script that the choice is being made from")]
    private ChoiceDialogNode choiceNode;
    [SerializeField, Tooltip("the text for this button")]

    
    private TMPro.TMP_Text choiceText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /// <summary>
    /// Set up this dialog choice button to hold a specific dialog choice
    /// </summary>
    /// <param name="node">the dialog node this button holds</param>
    /// <param name="choices">the currently displaying dialog node that contains the choice being made</param>
    /// <param name="text">the text to display on this button</param>
    public void AssignChoice(DialogNode node, ChoiceDialogNode choices, string text){
        dialogChoice = node;
        choiceNode = choices;
        choiceText.text = text;
    }

    /// <summary>
    /// run when this button is clicked: calls for this button's node to be the next node to display, and continues dialog
    /// </summary>
    public void SelectChoice(){
        Debug.Log("selected choice " + choiceText.text);
        choiceNode.PickDialogChoice(dialogChoice);
        DialogManager.Instance.MakeDialogChoice(dialogChoice);
    }
}
