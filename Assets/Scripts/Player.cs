using System;
using System.Collections.Generic;
using Pixelplacement;
using UnityEngine;
using UnityEngine.Events;
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
    private Vector3 _currentPosition;

    public void SetCurrentNode(Node newCurrentNode, HashSet<Node> adjacentNodes, bool isInit = false)
    {
        currentNode = newCurrentNode;
        Tween.Position(
            transform,
            currentNode.transform.position,
            isInit ? 0.0f : AnimationProperties.MovementTweenDuration,
            0.0f
        );
        _currentPosition = currentNode.transform.position;
        _adjacentNodes.Clear();
        _adjacentNodes.UnionWith(adjacentNodes);
    }

    public void NextTurn(Action<Node> onNextNode, bool isTracked)
    {
        trackedPanel.SetActive(isTracked);
        
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
            UnityAction toNextNode = () => onNextNode(node);
            if (position.x > _currentPosition.x)
            {
                right.onClick.AddListener(toNextNode);
                right.gameObject.SetActive(true);
            }
            else if (position.x < _currentPosition.x)
            {
                left.onClick.AddListener(toNextNode);
                left.gameObject.SetActive(true);
            }
            else if (position.y > _currentPosition.y)
            {
                up.onClick.AddListener(toNextNode);
                up.gameObject.SetActive(true);
            }
            else if (position.y < _currentPosition.y)
            {
                down.onClick.AddListener(toNextNode);
                down.gameObject.SetActive(true);
            }
        }
        
        wait.onClick.AddListener(() => onNextNode(currentNode));
    }
}