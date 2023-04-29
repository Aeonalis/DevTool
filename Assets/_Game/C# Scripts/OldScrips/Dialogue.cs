using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue 
{
    [SerializeField] public string _name;
    //[SerializeField] public string _Playername;
    [TextArea(3, 7)]
    [SerializeField] public string[] _sentences;
    //[SerializeField] public string[] _names;
}
