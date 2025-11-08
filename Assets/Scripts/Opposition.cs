using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Opposition : MonoBehaviour
{
    public Node currentNode;
    private readonly HashSet<Node> _adjacentNodes = new();
    private bool _didWait = false;
    
    public void SetCurrentNode(Node newCurrentNode, HashSet<Node> adjacentNodes)
    {
        currentNode = newCurrentNode;
        transform.position = currentNode.transform.position;
        _adjacentNodes.Clear();
        _adjacentNodes.UnionWith(adjacentNodes);
    }
    
    public void NextTurn(Action<Node> onNextNode, Func<Node, bool> onCheckPlayerVisibility)
    {
        if (onCheckPlayerVisibility(currentNode))
        {
            _didWait = false;
            onNextNode(currentNode);
            Debug.Log("Seeing player, skip turn...");
        }
        else
        {
            if (!_didWait)
            {
                _didWait = true;
                onNextNode(currentNode);
                Debug.Log("Thinking, skip turn...");
            }
            else
            {
                // _didWait = false;
                var randomDirection = _adjacentNodes.ToArray()[Random.Range(0, _adjacentNodes.Count)];
                Debug.Log($"Going towards {randomDirection}");
                onNextNode(randomDirection);
            }
        }
    }
}