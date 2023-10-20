using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomDialogNode : DialogNode
{
    [Serializable]
    public struct RandomDialogChoice{
        [Tooltip("the dialog node that this choice routes to")]
        public DialogNode randomNode;
        [Tooltip("the weight of this random event. Greater means more likely to occur\n set at 1 for true random")]
        public float nodeWeight;
    }
    [Header("Random node parameters")]
    [SerializeField, Tooltip("the list of all possible random conditions and weights")]
    private List<RandomDialogChoice> randomChoices;
    [Space]
    [Header("spawning nodes")]
    [SerializeField, Tooltip("the type of node to spawn")]private DialogNode.DialogType newNodeType;
    [SerializeField, Tooltip("the random weight of this node")]private float newNodeWeights = 1;
    // Start is called before the first frame update
    void Start()
    {
        if(randomChoices == null){
            randomChoices = new List<RandomDialogChoice>();
        }
    }


    /// <summary>
    /// Randomly selects one of the possible dialog nodes to go to next
    /// </summary>
    /// <returns>the randomly selected node</returns>
    public override DialogNode getNextNode()
    {
        if(randomChoices.Count == 0){
            return null;
        }
        float totalWeight = 0;
        foreach(RandomDialogChoice choice in randomChoices){
            totalWeight += choice.nodeWeight;
        }
        float randVal = UnityEngine.Random.Range(0, totalWeight);
        Debug.Log("Random value: " + randVal + " from " + totalWeight);
        float calc = 0;
        foreach(RandomDialogChoice choice in randomChoices){
            calc += choice.nodeWeight;
            if(calc >= randVal){
                return choice.randomNode;
            }
        }
        return randomChoices[0].randomNode;

    }


    //editor functions

    public void SpawnNewNode(){
        DialogNode thisNode = CreateNewNode(newNodeType);
        RandomDialogChoice newChoice = new RandomDialogChoice();
        newChoice.randomNode = thisNode;
        newChoice.nodeWeight = newNodeWeights;
        randomChoices.Add(newChoice);
        newNodeWeights = 1;
    }

}
