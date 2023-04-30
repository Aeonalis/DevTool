using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using System.Linq;

public class GraphSaveUtility
{
    private DialogueGraphView _targetGraphView;
    private DialogueContainer _containerCache;

    private List<Edge> Edges => _targetGraphView.edges.ToList();
  
    private List<DialogueNode> Nodes => _targetGraphView.nodes.ToList().Cast<DialogueNode>().ToList();
 
    public static GraphSaveUtility GetInstance(DialogueGraphView targetGraphView)
    {
        return new GraphSaveUtility
        {
            _targetGraphView = targetGraphView
        };
    }

    public void SaveGraph(string _fileName)
    {
        if(!Edges.Any())
        {
            return;
        }

        var dialogueContainer = ScriptableObject.CreateInstance<DialogueContainer>();

        var connectedPorts = Edges.Where(x => x.input.node != null).ToArray();
        Debug.Log("connectecPorts" + connectedPorts.Length);
        for (var i = 0; i < connectedPorts.Length; i++)
        {
            var outputNode = connectedPorts[i].output.node as DialogueNode;
            var inputNode = connectedPorts[i].input.node as DialogueNode;

            dialogueContainer.NodeLinks.Add(new NodeLinkData 
            {
              _baseNodeGuid = outputNode._GUID,
              _portName = connectedPorts[i].output.portName,
              _targetNodeGuid = inputNode._GUID
            });
        }

        foreach (var dialogueNode in Nodes.Where(node => !node._entryPoint))
        {
            dialogueContainer.DialogueNodeData.Add(new DialogueNodeData
            {
                _Guid = dialogueNode._GUID,
                _dialogueText = dialogueNode._dialogueText,
                _position = dialogueNode.GetPosition().position
            });
        }
        if (!AssetDatabase.IsValidFolder("Assets/Resources"))
        {
            AssetDatabase.CreateFolder("Assets", "Resources");
        }

        AssetDatabase.CreateAsset(dialogueContainer, $"Assets/Resources/{_fileName}.asset");
        AssetDatabase.SaveAssets();
    }

    public void LoadGraph (string _fileName)
    {
        _containerCache = Resources.Load<DialogueContainer>(_fileName);

        if(_containerCache == null)
        {
            EditorUtility.DisplayDialog("File Not Found", "Target dialogue graph file does not exist!", "OK");
            return;
        }

        ClearGraph();
        CreateNodes();
        ConnectNodes();

    }
    private void ConnectNodes()
    {
        for (var i = 0; i < Nodes.Count; i++)
        {
            var connections = _containerCache.NodeLinks.Where(x => x._baseNodeGuid == Nodes[i]._GUID).ToList();
            for (var j = 0; j < connections.Count; j++)
            {
                var targetNodeGuid = connections[j]._targetNodeGuid;
                var targetNode = Nodes.First(x => x._GUID == targetNodeGuid);
                LinkNodes(Nodes[i].outputContainer[j].Q<Port>(), (Port)targetNode.inputContainer[0]);

                targetNode.SetPosition(new Rect(_containerCache.DialogueNodeData.First(x => x._Guid == targetNodeGuid)._position, _targetGraphView._defaultNodeSize));
            }
        }
    }

    private void LinkNodes(Port _output, Port _input)
    {
        var tempEdge = new Edge
        {
            output = _output,
            input = _input,
        };

        tempEdge?.input.Connect(tempEdge);
        tempEdge?.output.Connect(tempEdge);
        _targetGraphView.Add(tempEdge);


    }
    private void CreateNodes()
    {
        foreach (var nodeData in _containerCache.DialogueNodeData)
        {
            Debug.Log("debug1");
            var tempNode = _targetGraphView.CreateDialogueNode(nodeData._dialogueText);
            tempNode._GUID = nodeData._Guid;
            _targetGraphView.AddElement(tempNode);

            var nodePorts = _containerCache.NodeLinks.Where(x => x._baseNodeGuid == nodeData._Guid).ToList();
           
            //nodePorts.ForEach(x => _targetGraphView.AddChoicePort(tempNode, x._portName));
            foreach (var x in nodePorts)
            {
                Debug.Log("inside load foreach" + x);
                _targetGraphView.AddChoicePort(tempNode, x._portName);
            }
        }
    }
    private void ClearGraph()
    {
        Nodes.Find(x => x._entryPoint)._GUID = _containerCache.NodeLinks[0]._baseNodeGuid;

        foreach (var node in Nodes)
        {
            if (node._entryPoint)
            {
                continue;
            }
            Edges.Where(x => x.input.node == node).ToList().ForEach(edge => _targetGraphView.RemoveElement(edge));

            _targetGraphView.RemoveElement(node);
        }
    }

    


}
