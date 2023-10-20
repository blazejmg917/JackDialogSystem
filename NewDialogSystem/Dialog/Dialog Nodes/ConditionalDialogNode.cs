using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionalDialogNode : DialogNode
{
    public enum ConditionType{
        VARIABLE,
        FLAG
    }
    public enum Operator{
        Greater,
        Less,
        GreaterEquals,
        LessEquals,
        Equals,
        NotEquals,
        True,
        False
    }
    [System.Serializable]
    public struct DialogCondition{
        [Tooltip("the id of the condition or flag you want to check for this conditional :)")]
        public string id;
        [Tooltip("what type of value you are checking")]
        public ConditionType conditionType;
        [Tooltip("the conditonal operator you want to check this variable or flag with \nUse True or False for flags, or any other option for variables")]
        public Operator conditional;
        [Tooltip("value you want to compare your variable with\nLeave blank for flags")]
        public int value;
        [Tooltip("the node you want to route to if this condition is true")]
        public DialogNode conditionalNode;
    }
    [System.Serializable]
    public struct DialogNodeCreator{
        [Tooltip("the id of the condition or flag you want to check for this conditional :)")]
        public string id;
        [Tooltip("what type of value you are checking")]
        public ConditionType conditionType;
        [Tooltip("the conditonal operator you want to check this variable or flag with \nUse True or False for flags, or any other option for variables")]
        public Operator conditional;
        [Tooltip("value you want to compare your variable with\nLeave blank for flags")]
        public int value;
        [Tooltip("the type of node that this should lead to")]
        public DialogNode.DialogType dialogType;
    }
    [Header("conditional dialog node properties")]
    [SerializeField, Tooltip("the default node to go to if none of the conditions are met")]
    private DialogNode defaultNode;
    [SerializeField, Tooltip("List of all conditionals to check for next dialog node selection")]
    private List<DialogCondition> conditions = new List<DialogCondition>();
    [Space]
    [Header("spawning nodes")]
    [SerializeField, Tooltip("info for the new node to spawn")]private DialogNodeCreator newNode = new DialogNodeCreator();

    void Awake(){
        Debug.Log(conditions.Count);
        foreach(DialogCondition option in conditions){
            if(option.conditionalNode == null){
                Debug.LogWarning("NULL NODE");
            }
            else{
                Debug.Log("node: " + option.conditionalNode.getDialog());
            }
        }
    }

    /// <summary>
    /// for a conditional node, run through every conditional check to determine which node should be displayed next
    /// </summary>
    /// <returns>the next dialog node</returns>
    public override DialogNode getNextNode()
    {
        
        //check each posible condition to see if it is true. If so, display that node, otherwise move onto the next
        foreach(DialogCondition option in conditions){
            switch(option.conditionType){

                //for a variable, check if the value comparison matches with the provided comparator type
                case ConditionType.VARIABLE:
                    int val;
                    if(DialogManager.GetVariable(option.id, out val)){
                        if(CompareValue(val, option.conditional, option.value)){
                            return option.conditionalNode;
                        }
                    }
                    break;

                case ConditionType.FLAG:
                    //for a flag, check if the flag is set to the proper status

                    bool flagVal;
                    if(DialogManager.GetFlag(option.id, out flagVal)){
                        //Debug.Log("checking flag with " + val + ", and " + option.conditional);
                        
                        if(option.conditional == Operator.True && flagVal){
                            return option.conditionalNode;
                        }
                        if(option.conditional == Operator.False && !flagVal){
                            return option.conditionalNode;
                        }
                    }
                        
                    break;
            }
        }
        //if not values returned true, select the default node
        return defaultNode;
        
    }
    /// <summary>
    /// Check whether or not a comparision between two values is true, given an Operator comparison type.
    /// Runs from right to left, such that (3, Greater, 2) would be evaluated as 3 > 2, and return true.
    /// </summary>
    /// <param name="value1">the left side of the comparison</param>
    /// <param name="comparison">the type of comparison to perform</param>
    /// <param name="value2">the right side of the comparison</param>
    /// <returns>the result of the comparison</returns>
    public bool CompareValue(int value1, Operator comparison, int value2){
        switch(comparison){
            case Operator.Greater:
                return value1 > value2;
            case Operator.GreaterEquals:
                return value1 >= value2;
            case Operator.Less:
                return value1 < value2;
            case Operator.LessEquals:
                return value1 <= value2;
            case Operator.Equals:
                return value1 == value2;
            case Operator.NotEquals:
                return value1 != value2;
            default:
                Debug.LogError("tried to use true/false conditional for non-boolean condition. returning false");
                return false;
        }
    }



    //editor functions
    public void SpawnNewNode(){
        DialogNode thisNode = CreateNewNode(newNode.dialogType);
        conditions.Add(SetupNode(newNode, thisNode));
        newNode = new DialogNodeCreator();
    }

    public DialogCondition SetupNode(DialogNodeCreator creatorVals, DialogNode setupNode){
        DialogCondition newCondition = new DialogCondition();
        newCondition.id = creatorVals.id;
        newCondition.value = creatorVals.value;
        newCondition.conditional = creatorVals.conditional;
        newCondition.conditionType = creatorVals.conditionType;
        newCondition.conditionalNode = setupNode;
        return newCondition;
    }
}
