using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Map : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject oppositionPrefab;
    [SerializeField] private GameObject mapNodesParent;
    [SerializeField] private Node startNode;
    [SerializeField] private Node[] oppositionStartNodes;
    [SerializeField] private Node endNode;

    private Player _player;
    private readonly OppositionController _oppositionController = new();
    private readonly Dictionary<Node, HashSet<Node>> _nodes = new();
    private int _turn;
    private bool _playerTracked;

    private void Start()
    {
        Setup();
        NextTurn();
    }

    private void Setup()
    {
        var mapNodes = mapNodesParent.GetComponentsInChildren<Node>();
        foreach (var node in mapNodes)
        {
            if (!_nodes.ContainsKey(node))
            {
                _nodes.Add(node, new HashSet<Node>());
            }

            var links = _nodes[node];
            foreach (var link in node.adjacent)
            {
                links.Add(link);

                if (!_nodes.ContainsKey(link))
                {
                    _nodes.Add(link, new HashSet<Node>());
                }

                _nodes[link].Add(node);
            }

            _nodes[node] = links;
        }

        var playerInstance = Instantiate(playerPrefab);
        if (playerInstance.TryGetComponent<Player>(out var player))
        {
            _player = player;
            _player.SetCurrentNode(startNode, _nodes[startNode]);
        }

        foreach (var node in oppositionStartNodes)
        {
            var oppositionInstance = Instantiate(oppositionPrefab);
            if (!oppositionInstance.TryGetComponent<Opposition>(out var o)) continue;
            _oppositionController.Add(o, node, OnCheckPlayerVisibility);
        }
    }

    private void NextTurn()
    {
        if (_player.currentNode == endNode && !_playerTracked)
        {
            Debug.Log("Destination reached!");
            return;
        }

        _turn++;

        if (_turn % 2 != 0)
        {
            _player.NextTurn(
                PlayerMove,
                _oppositionController.Opposition.Any(o => OnCheckPlayerVisibility(o.currentNode))
            );
        }
        else
        {
            _oppositionController.NextTurn(AdjacentForNode, OnCheckPlayerVisibility, NextTurn);
        }
    }

    private IReadOnlyCollection<Node> AdjacentForNode(Node n) => _nodes[n] ?? new HashSet<Node>();

    private void PlayerMove(Node next)
    {
        _player.SetCurrentNode(next, _nodes[next]);
        NextTurn();
    }

    private bool OnCheckPlayerVisibility(Node node)
    {
        // TODO: check along direction of looking
        return _nodes[node].Contains(_player.currentNode) || node == _player.currentNode;
    }
}