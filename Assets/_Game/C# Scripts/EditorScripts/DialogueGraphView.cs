using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using UnityEngine.UIElements;
using System.Linq;
public class DialogueGraphView : GraphView
{
    public readonly Vector2 _defaultNodeSize = new Vector2(x: 150, y: 200);
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

        node.capabilities &= ~Capabilities.Movable;
        node.capabilities &= ~Capabilities.Deletable;


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

        _dialogueNode.styleSheets.Add(Resources.Load<StyleSheet>("Node"));

        var _button = new Button(clickEvent: () => { AddChoicePort(_dialogueNode); });
        _button.text = "New Choice";
        _dialogueNode.titleContainer.Add(_button);

        var textField = new TextField(string.Empty);
        textField.RegisterValueChangedCallback(evt =>
        {
            _dialogueNode._dialogueText = evt.newValue;
            _dialogueNode.title = evt.newValue;

        });

        textField.SetValueWithoutNotify(_dialogueNode.title);
        _dialogueNode.mainContainer.Add(textField);


        _dialogueNode.RefreshExpandedState();
        _dialogueNode.RefreshPorts();
        _dialogueNode.SetPosition(new Rect(position: Vector2.zero, _defaultNodeSize));

        return _dialogueNode;
    }

    public void AddChoicePort (DialogueNode _dialogueNode, string overridenPortName ="")
    {
        var generatedPort = GeneratePort(_dialogueNode, Direction.Output);

        var oldLabel = generatedPort.contentContainer.Q<Label>("type");
        generatedPort.contentContainer.Remove(oldLabel);

        var outputPortCount = _dialogueNode.outputContainer.Query(name: "connector").ToList().Count;

        var choicePortName = string.IsNullOrEmpty(overridenPortName) ? $"Choice {outputPortCount + 1}" : overridenPortName;

        var textField = new TextField
        {
            name = string.Empty,
            value = choicePortName
        };

        textField.RegisterValueChangedCallback(evt => generatedPort.portName = evt.newValue);
        generatedPort.contentContainer.Add(new Label(" "));
        generatedPort.contentContainer.Add(textField);
        var deleteButton = new Button(() => RemovePort(_dialogueNode, generatedPort))
        {
            text = "X"
        };
        generatedPort.contentContainer.Add(deleteButton);



        generatedPort.portName = choicePortName;
        _dialogueNode.outputContainer.Add(generatedPort);
        _dialogueNode.RefreshPorts();
        _dialogueNode.RefreshExpandedState();
    }

    private void RemovePort(DialogueNode _dialogueNode, Port _generatedPort)
    {
        var targetEdge = edges.ToList().Where(x => x.output.portName == _generatedPort.portName && x.output.node == _generatedPort.node);

        if (targetEdge.Any())
        {
            var _edge = targetEdge.First();
            _edge.input.Disconnect(_edge);
            RemoveElement(targetEdge.First());
        }

        _dialogueNode.outputContainer.Remove(_generatedPort);
        _dialogueNode.RefreshPorts();
        _dialogueNode.RefreshExpandedState();


    }
}
