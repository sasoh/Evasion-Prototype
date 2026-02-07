using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Evasion
{
    public class Player : MonoBehaviour
    {
        public Node currentNode;
        [SerializeField] private GameObject directionButtonPrefab;
        [SerializeField] private Button wait;
        [SerializeField] private GameObject uiCanvas;
        [SerializeField] private GameObject trackedPanel;
        [SerializeField] private GameObject carImagePanel;

        private readonly HashSet<Node> _adjacentNodes = new();
        private readonly List<GameObject> _directionButtons = new();
        private Vector3 _currentPosition;

        public void SetCurrentNode(Node newCurrentNode, HashSet<Node> adjacentNodes, bool isInit = false)
        {
            CarAnimator.Animate(
                transform,
                carImagePanel,
                currentNode ? currentNode.transform.position : newCurrentNode.transform.position,
                newCurrentNode.transform.position,
                isInit
            );
            currentNode = newCurrentNode;
            _currentPosition = currentNode.transform.position;
            _adjacentNodes.Clear();
            _adjacentNodes.UnionWith(adjacentNodes);
        }

        public void NextTurn(Action<Node> onNextNode, bool isTracked)
        {
            trackedPanel.SetActive(isTracked);
            
            foreach (var button in _directionButtons)
            {
                Destroy(button);
            }

            wait.onClick.RemoveAllListeners();

            foreach (var node in _adjacentNodes)
            {
                if (node == null || !node.gameObject.activeInHierarchy) continue;
                var newButton = Instantiate(directionButtonPrefab, uiCanvas.transform);
                _directionButtons.Add(newButton);
                if (!newButton.TryGetComponent<Button>(out var b)) continue;
                b.onClick.AddListener(() => onNextNode(node));
                
                var targetDirection = (node.transform.position - _currentPosition).normalized;
                var facingDirection = newButton.transform.up; // current sprite points upwards
                var angle = Vector3.SignedAngle(targetDirection, facingDirection, Vector3.back);
                newButton.transform.rotation = Quaternion.Euler(0, 0, angle);
                newButton.transform.position = carImagePanel.transform.position + newButton.transform.up / 3;
            }

            wait.onClick.AddListener(() => onNextNode(currentNode));
        }
    }
}