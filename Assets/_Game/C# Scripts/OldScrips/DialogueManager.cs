using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
//using UnityEngine.UIElements;


public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI _button1;
    public TextMeshProUGUI _button2;
    public TextMeshProUGUI _nameText;
    public TextMeshProUGUI _dialogueText;
    private Queue <string> _sentences;
    public Animator _introDialogueAnimator;
    public Animator _introPortraitAnimator;
    public Animator _continueAnimator;
    public Animator _textBoxAnimator;
    public Animator _choiceAnimator;
    public float _typingSpeed;
    public Button _continueBtn;
    public Button _continueBtn2;
    public GameObject _introPanel;
    public GameObject _choice01Panel;
    public GameObject _choice02Panel;


    private bool buttonFlag = false;

    void Start()
    {
        _choice01Panel.SetActive(false);
        _choice02Panel.SetActive(false);
        _continueBtn2.gameObject.SetActive(false);
        _sentences = new Queue<string>();
    }

    public void StartDialogue (DialogueContainer _dialogue, string _guid, string _name)
    {
        _introDialogueAnimator.SetBool("IsOpen", true);
        _introPortraitAnimator.SetBool("IsOpen", true);
        _textBoxAnimator.SetBool("IsOpen", true);
        _continueAnimator.SetBool("IsOpen", true);
        _choice01Panel.SetActive(false);
        _choice02Panel.SetActive(false);
        DialogueTrigger triggerComp = _continueBtn2.gameObject.GetComponent<DialogueTrigger>();
        triggerComp._GUID = null;
        DialogueTrigger triggerComp2 = _continueBtn.gameObject.GetComponent<DialogueTrigger>();
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
                Debug.Log("Evelynn".Equals(nodeData._portName));
                if (buttonFlag)
                {
                  
                    _button2.text = "SampleText";
                    _button2.text = nodeData._portName;
                    triggerComp._GUID = nodeData._targetNodeGuid;
                    triggerComp._name = nodeData._portName;
   
                    if (!triggerComp._name.Equals("Evelynn") && !triggerComp._name.Equals("Cassiel"))
                    {
                        triggerComp._name = _name;
                    }
                   

                }
                else
                {
                    _button1.text = "SampleText";
                    _button1.text = nodeData._portName;                   
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
                _choice02Panel.SetActive(true);
                _introPanel.SetActive(false);
                break;
            case "Evelynn":
                _choice01Panel.SetActive(true);
                _introPanel.SetActive(false);
                break;
            default:
                break;

        }
        buttonFlag = false;
        if (triggerComp._GUID == null)
        {       
            _continueBtn2.gameObject.SetActive(false);         
        }
        else
        {
            _continueBtn2.gameObject.SetActive(true);
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
        _continueBtn.interactable = false;
        _continueBtn2.interactable = false;
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
         _continueBtn.interactable = true;
        _continueBtn2.interactable = true;
    }
    void EndDialogue()
    {
        Debug.Log("End of conversation.");
        _introDialogueAnimator.SetBool("IsOpen", false);
        _introPortraitAnimator.SetBool("IsOpen", false);
        _continueAnimator.SetBool("IsOpen", false);
        _textBoxAnimator.SetBool("IsOpen", false);

    }
}
