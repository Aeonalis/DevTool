using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using UnityEngine.UIElements;
public class DialogueGraphView : GraphView
{
    private readonly Vector2 _defaultNodeSize = new Vector2(x: 150, y: 200);
   public DialogueGraphView()
    {
        styleSheets.Add(Resources.Load<StyleSheet>(path: "Dialogue"));
        SetupZoom(ContentZoomer.DefaultMinScale,ContentZoomer.DefaultMaxScale);

        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        var _grid = new GridBackground();
        Insert(index: 0, _grid);
        _grid.StretchToParentSize();


        AddElement(GenerateEntryPointNode());
    }

    public override List<Port> GetCompatiblePorts (Port _startPort, NodeAdapter _nodeAdapter)
    {
        var compatiblePorts = new List<Port>();
        ports.ForEach(funcCall: (port) =>
        {
            if (_startPort!= port && _startPort.node!= port.node && _startPort.direction != port.direction)
            {
                compatiblePorts.Add(port);
            }
        });


        return compatiblePorts;
    }
    private Port GeneratePort(DialogueNode node, Direction portDirection, Port.Capacity capacity = Port.Capacity.Single)
    {
        return node.InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(float));
    }
    private DialogueNode GenerateEntryPointNode()
    {
        var node = new DialogueNode
        {
            title = "START",
            _GUID = System.Guid.NewGuid().ToString(),
            _dialogueText = "ENTRYPOINT",
            _entryPoint = true

        };

        var _generatedPort = GeneratePort(node, Direction.Output);
        _generatedPort.portName = "Next";
        node.outputContainer.Add(_generatedPort);

        node.RefreshExpandedState();
        node.RefreshPorts();

        node.SetPosition(new Rect(x: 100, y: 200, width: 100, height: 150));
        return node;
    }

    public void CreateNode(string _nodeName)
    {
        AddElement(CreateDialogueNode(_nodeName));
    }

    public DialogueNode CreateDialogueNode(string _nodeName)
    {
        var _dialogueNode = new DialogueNode
        {
            title = _nodeName,
            _dialogueText = _nodeName,
            _GUID = System.Guid.NewGuid().ToString()
        };

        var _inputPort = GeneratePort(_dialogueNode, Direction.Input, Port.Capacity.Multi);
        _inputPort.portName = "Input";
        _dialogueNode.inputContainer.Add(_inputPort);

        var _button = new Button(clickEvent: () => { AddChoicePort(_dialogueNode); });
        _button.text = "New Choice";
        _dialogueNode.titleContainer.Add(_button);


        _dialogueNode.RefreshExpandedState();
        _dialogueNode.RefreshPorts();
        _dialogueNode.SetPosition(new Rect(position: Vector2.zero, _defaultNodeSize));

        return _dialogueNode;
    }

    private void AddChoicePort (DialogueNode _dialogueNode)
    {
        var generatedPort = GeneratePort(_dialogueNode, Direction.Output);

        var outputPortCount = _dialogueNode.outputContainer.Query(name: "connector").ToList().Count;
        generatedPort.portName = $"Choice {outputPortCount}";

        _dialogueNode.outputContainer.Add(generatedPort);
        _dialogueNode.RefreshPorts();
        _dialogueNode.RefreshExpandedState();
    }
}
