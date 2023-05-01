using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using UnityEngine.UIElements;
using System;

[Serializable]
public class DialogueNode : Node
{
    public string _GUID;
    public string _dialogueText;
    public string _name = "Start";
    public bool _entryPoint = false;
}
