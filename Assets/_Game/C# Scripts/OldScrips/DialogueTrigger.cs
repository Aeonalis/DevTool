using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    

    [SerializeField] public string _dialogueContainer;
    public string _GUID;
    public string _name;
    
    public void TriggerDialogue ()
    {
      
        DialogueContainer _containerCache = Resources.Load<DialogueContainer>(_dialogueContainer);
        Debug.Log("DialogueNode" + _containerCache.DialogueNodeData[0]._Guid);
        FindObjectOfType<DialogueManager>().StartDialogue(_containerCache, _containerCache.DialogueNodeData[0]._Guid, _name); 
    }

    public void ContinueDialogue()
    {
        DialogueContainer _containerCache = Resources.Load<DialogueContainer>(_dialogueContainer);
        FindObjectOfType<DialogueManager>().StartDialogue(_containerCache, _GUID, _name); 

    }
}
