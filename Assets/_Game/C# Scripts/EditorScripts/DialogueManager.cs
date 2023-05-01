using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

//using UnityEngine.UIElements;


public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI _button1Text;
    public TextMeshProUGUI _button2Text;
    public Button _button1;
    public Button _button2;
    public Animator _button1Animator;
    public Animator _button2Animator;
    public GameObject _choice1Panel;
    public GameObject _choice2Panel;
    public TextMeshProUGUI _dialogueText;
    public Animator _textBoxAnimator;
    private Queue <string> _sentences;
    public Animator _introDialogueAnimator;
    public Animator _introPortraitAnimator;
    public GameObject _introPanel;
    public float _typingSpeed;



    private bool buttonFlag = false;

    void Start()
    {
        _choice1Panel.SetActive(false);
        _choice2Panel.SetActive(false);
        _button2.gameObject.SetActive(false);
        _sentences = new Queue<string>();
    }

    public void StartDialogue (DialogueContainer _dialogue, string _guid, string _name)
    {
        _introDialogueAnimator.SetBool("IsOpen", true);
        _introPortraitAnimator.SetBool("IsOpen", true);
        _textBoxAnimator.SetBool("IsOpen", true);
        _button1Animator.SetBool("IsOpen", true);
        _choice1Panel.SetActive(false);
        _choice2Panel.SetActive(false);
        DialogueTrigger triggerComp = _button2.gameObject.GetComponent<DialogueTrigger>();
        triggerComp._GUID = null;
        DialogueTrigger triggerComp2 = _button1.gameObject.GetComponent<DialogueTrigger>();
        triggerComp2._GUID = null;
    


        _sentences.Clear();
        foreach (DialogueNodeData nodeData in _dialogue.DialogueNodeData)
        {
           
            if (nodeData._Guid == _guid)
            {
                _sentences.Enqueue(nodeData._dialogueText);
                break;
            }

        }

      

        foreach (NodeLinkData nodeData in _dialogue.NodeLinks)
        {
           
            if (nodeData._baseNodeGuid == _guid)
            {
                if (buttonFlag)
                {
                 
                    _button2Text.text = "SampleText";
                    _button2Text.text = nodeData._portName;
                    triggerComp._GUID = nodeData._targetNodeGuid;
                    triggerComp._name = nodeData._portName;
                    if (!triggerComp._name.Equals("Evelynn") && !triggerComp._name.Equals("Cassiel"))
                    {
                        triggerComp._name = _name;
                    }
                    
                }
                else
                {
                    _button1Text.text = "SampleText";
                    _button1Text.text = nodeData._portName;                   
                    triggerComp2._GUID = nodeData._targetNodeGuid;
                    triggerComp2._name = nodeData._portName;

                     if (!triggerComp2._name.Equals("Evelynn") && !triggerComp2._name.Equals("Cassiel"))
                     {
                         triggerComp2._name = _name;
                     }

                }
                buttonFlag = true;
            }
        }
       
        switch (_name)
        {
            case "Cassiel":
                _choice2Panel.SetActive(true);
                _introPanel.SetActive(false);
                break;
            case "Evelynn":
                _choice1Panel.SetActive(true);
                _introPanel.SetActive(false);
                break;
            default:
                break;

        }
        buttonFlag = false;
        if (triggerComp._GUID == null)
        {       
            _button2.gameObject.SetActive(false);         
        }
        else
        {
            _button2.gameObject.SetActive(true);
        }
        if (triggerComp2._GUID == null)
        {
            
        }
        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (_sentences.Count == 0)
        {
            EndDialogue();
            return;
        }
        _button1.interactable = false;
        _button2.interactable = false;
        string sentence = _sentences.Dequeue();
        
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));

    }
     IEnumerator TypeSentence (string sentence)
     {
         _dialogueText.text = "";
         foreach (char letter in sentence.ToCharArray())
         {
             _dialogueText.text += letter;
             yield return new WaitForSeconds(_typingSpeed);
         }
         _button1.interactable = true;
        _button2.interactable = true;
    }
    void EndDialogue()
    {
        Debug.Log("End of conversation.");
        _introDialogueAnimator.SetBool("IsOpen", false);
        _introPortraitAnimator.SetBool("IsOpen", false);
        _button1Animator.SetBool("IsOpen", false);
        _textBoxAnimator.SetBool("IsOpen", false);

    }
}
