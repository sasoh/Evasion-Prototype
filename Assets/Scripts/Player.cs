using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public Node currentNode;
    [SerializeField] private Button up;
    [SerializeField] private Button down;
    [SerializeField] private Button left;
    [SerializeField] private Button right;
    [SerializeField] private Button wait;
    [SerializeField] private GameObject trackedPanel;

    private readonly HashSet<Node> _adjacentNodes = new();

    public void SetCurrentNode(Node newCurrentNode, HashSet<Node> adjacentNodes)
    {
        currentNode = newCurrentNode;
        transform.position = currentNode.transform.position;
        _adjacentNodes.Clear();
        _adjacentNodes.UnionWith(adjacentNodes);
    }

    public void NextTurn(Action<Node> onNextNode, bool isTracked)
    {
        trackedPanel.SetActive(isTracked);
        
        // TODO: Set button visibility depending on the possible directions.
        up.onClick.RemoveAllListeners();
        down.onClick.RemoveAllListeners();
        left.onClick.RemoveAllListeners();
        right.onClick.RemoveAllListeners();
        wait.onClick.RemoveAllListeners();
        
        up.gameObject.SetActive(false);
        down.gameObject.SetActive(false);
        left.gameObject.SetActive(false);
        right.gameObject.SetActive(false);

        // TODO: Maybe replace with a dynamic button placement (on top of each edge or sth).
        foreach (var node in _adjacentNodes)
        {
            var position = node.transform.position;
            if (position.x > transform.position.x)
            {
                right.onClick.AddListener(() => onNextNode(node));
                right.gameObject.SetActive(true);
            }
            else if (position.x < transform.position.x)
            {
                left.onClick.AddListener(() => onNextNode(node));
                left.gameObject.SetActive(true);
            }
            else if (position.y > transform.position.y)
            {
                up.onClick.AddListener(() => onNextNode(node));
                up.gameObject.SetActive(true);
            }
            else if (position.y < transform.position.y)
            {
                down.onClick.AddListener(() => onNextNode(node));
                down.gameObject.SetActive(true);
            }
        }
        
        wait.onClick.AddListener(() => onNextNode(currentNode));
    }
}