using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogVarsHandler : MonoBehaviour
{
    [System.Serializable]public struct dialogVarInitializer{
        [Tooltip("variable id")]public string varId;
        [Tooltip("variable initial value")]public int initialVarValue;
    }
    [System.Serializable]public struct dialogFlagInitializer{
        [Tooltip("flag id")]public string flagId;
        [Tooltip("flag initial value")]public bool initialFlagValue;
    }
    private Dictionary<string,int> dialogVarDict = new Dictionary<string, int>();
    private Dictionary<string,bool> dialogFlagDict = new Dictionary<string, bool>();

    [SerializeField, Tooltip("list of dialog variables to initialize at game start")]private List<dialogVarInitializer> initialVariables = new List<dialogVarInitializer>();
    [SerializeField, Tooltip("list of dialog flags to initialize at game start")]private List<dialogFlagInitializer> initialFlags = new List<dialogFlagInitializer>();
    // Start is called before the first frame update
    void Awake()
    {
        foreach(dialogVarInitializer initialVar in initialVariables){
            dialogVarDict[initialVar.varId] = initialVar.initialVarValue;
        }
        foreach(dialogFlagInitializer initialFlag in initialFlags){
            dialogFlagDict[initialFlag.flagId] = initialFlag.initialFlagValue;
        }
    }

    /// <summary>
    /// Updates a variable in the dictionary
    /// </summary>
    /// <param name="variable">the id of the variable to update</param>
    /// <param name="value">the new value of the variable</param>
    /// <param name="onlyUpdateExisting">if set to true, will only try to update existing values and will return false otherwise \nOtherwise, will add new entry if it doesn't exist</param>
    /// <returns>false if onlyUpdateExisting is true and the value doesn't exist. true otherwise</returns>
    public bool UpdateVariable(string variable, int value, bool onlyUpdateExisting = false){
        if(onlyUpdateExisting && !dialogVarDict.ContainsKey(variable)){
            return false;
        }
        dialogVarDict[variable] = value;
        return true;
    }

    /// <summary>
    /// attempts to get a variable's value from the dictionary
    /// </summary>
    /// <param name="variable">the id of the variable to search for</param>
    /// <param name="value">the out field to get the value of the variable</param>
    /// <returns>Whether or not the variable exists</returns>
    public bool GetVariable(string variable, out int value){
        value = -1;
        try{
            value = dialogVarDict[variable];
            return true;
        }
        catch(KeyNotFoundException e){
            return false;
        }
    }

    /// <summary>
    /// Updates a flag in the dictionary
    /// </summary>
    /// <param name="flag">the id of the flag to update</param>
    /// <param name="value">the new value of the flag</param>
    /// <param name="onlyUpdateExisting">if set to true, will only try to update existing flags and will return false otherwise \nOtherwise, will add new entry if it doesn't exist</param>
    /// <returns>false if onlyUpdateExisting is true and the value doesn't exist. true otherwise</returns>
    public bool UpdateFlag(string flag, bool value, bool onlyUpdateExisting = false){
        if(onlyUpdateExisting && !dialogFlagDict.ContainsKey(flag)){
            return false;
        }
        dialogFlagDict[flag] = value;
        return true;
    }

    /// <summary>
    /// attempts to get a flag's value from the dictionary
    /// </summary>
    /// <param name="flag">the id of the flag to search for</param>
    /// <param name="value">the out field to get the value of the flag</param>
    /// <returns>Whether or not the flag exists</returns>
    public bool GetFlag(string flag, out bool value){
        value = false;
        try{
            value = dialogFlagDict[flag];
            return true;
        }
        catch(KeyNotFoundException e){
            return false;
        }
    }
}
