using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI _nameText;
    public TextMeshProUGUI _dialogueText;
    private Queue <string> _sentences;
    public Animator _dialogueAnimator;
    public Animator _portraitAnimator;
    public Animator _continueAnimator;
    public float _typingSpeed;
    public Button _continueBtn;

    void Start()
    {
        _sentences = new Queue<string>();
        //StartCoroutine(TypeSentence(sentence));
    }

    public void StartDialogue (Dialogue _dialogue)
    {
        _dialogueAnimator.SetBool("IsOpen", true);
        _portraitAnimator.SetBool("IsOpen", true);
        _continueAnimator.SetBool("IsOpen", true);


        _sentences.Clear();

        _nameText.text = _dialogue._name;
        foreach(string sentence in _dialogue._sentences)
        {
            _sentences.Enqueue(sentence);
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
     }
    void EndDialogue()
    {
        Debug.Log("End of conversation.");
        _dialogueAnimator.SetBool("IsOpen", false);
        _portraitAnimator.SetBool("IsOpen", false);
        _continueAnimator.SetBool("IsOpen", false);

    }
}
