using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

public class DialogueGraph : EditorWindow
{
    private DialogueGraphView _graphView;
    private string _fileName = "New Narrative";

    [MenuItem("Graph/Dialogue Graph")]
    public static void OpenDialogueGraphWindow()
    {
        var window = GetWindow<DialogueGraph>();
        window.titleContent = new GUIContent(text: "Dialogue Graph");
    }

    private void OnEnable()
    {
        ConstructGraphView();
        GenerateToolbar();
        GenerateMiniMap();
        //GenerateBlackBoard();
        OnGUI();
    }

    /*private void GenerateBlackBoard()
    {
        var blackboard = new Blackboard(_graphView);
        blackboard.Add(new BlackboardSection { title = "Exposed Properties"});
        blackboard.addItemRequested = _blackboard => { _graphView.AddPropertyToBlackBoard(new ExposedProperty()); };
        blackboard.SetPosition(new Rect(10, 30, 200, 300));
        _graphView.Add(blackboard);
        _graphView.Blackboard = blackboard;


    }*/
    private void ConstructGraphView()
    {
        _graphView = new DialogueGraphView (this)
        {
            name = "Dialogue Graph"
        };

        _graphView.StretchToParentSize();
        rootVisualElement.Add(_graphView);
    }

    private void GenerateToolbar()
    {
        var toolbar = new Toolbar();

        var _fileNameTextField = new TextField(label: "File Name:");
        _fileNameTextField.SetValueWithoutNotify(_fileName);
        _fileNameTextField.MarkDirtyRepaint();
        _fileNameTextField.RegisterValueChangedCallback( evt  => _fileName = evt.newValue);
        toolbar.Add(_fileNameTextField);

        toolbar.Add( new Button( () => RequestDataOperation(true)) { text = "Save Data" });
        toolbar.Add( new Button( () => RequestDataOperation(false)) { text = "Load Data" });

        rootVisualElement.Add(toolbar);

    }
    
    private void GenerateMiniMap()
    {
        var miniMap = new MiniMap { anchored = true };
        miniMap.SetPosition(new Rect(this.position.width-210,30,200, 140));
        _graphView.Add(miniMap);
    }

    private void RequestDataOperation(bool _save)
    {
        if (string.IsNullOrEmpty(_fileName))
        {
            EditorUtility.DisplayDialog("Invalid file name!", "Please enter a valid file name.", ok: "OK");
            return;
        }

        var saveUtility = GraphSaveUtility.GetInstance(_graphView);
        if(_save)
        {
            saveUtility.SaveGraph(_fileName);
        }
        else
        {
            saveUtility.LoadGraph(_fileName);
        }
    }
    private void OnDisable()
    {
        rootVisualElement.Remove(_graphView);
    }
    private void OnGUI()
    {
        if (Event.current.rawType == EventType.Layout)
        {
            MiniMap _miniMap = _graphView.contentContainer.Q<MiniMap>();
            _miniMap.SetPosition(new Rect(this.position.width - 210, 30, 200, 140));
        }
    }
}

