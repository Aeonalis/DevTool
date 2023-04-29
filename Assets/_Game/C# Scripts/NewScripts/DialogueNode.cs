using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using UnityEngine.UIElements;
public class DialogueNode : Node
{
    public string _GUID;
    public string _dialogueText;
    public bool _entryPoint = false;
}
