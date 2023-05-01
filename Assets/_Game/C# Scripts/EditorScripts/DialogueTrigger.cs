using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] DialogueContainer _dialogueContainer;
    public string _GUID;
    public string _name;
    
    public void TriggerDialogue ()
    {
        FindObjectOfType<DialogueManager>().StartDialogue(_dialogueContainer, _dialogueContainer.DialogueNodeData[0]._Guid, _name); 
    }

    public void ContinueDialogue()
    {
        FindObjectOfType<DialogueManager>().StartDialogue(_dialogueContainer, _GUID, _name); 
    }
}
