using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject oppositionPrefab;
    [SerializeField] private GameObject mapNodesParent;
    [SerializeField] private Node startNode;
    [SerializeField] private Node oppositionStartNode;
    [SerializeField] private Node endNode;

    private Player _player;
    private Opposition _opposition;
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
        // map setup
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
        
        // spawn player
        var playerInstance = Instantiate(playerPrefab);
        if (playerInstance.TryGetComponent<Player>(out var player))
        {
            _player = player;
            _player.SetCurrentNode(startNode, _nodes[startNode]);
        }

        // // spawn opposition
        var oppositionInstance = Instantiate(oppositionPrefab);
        if (oppositionInstance.TryGetComponent<Opposition>(out var opposition))
        {
            _opposition = opposition;
            _opposition.SetCurrentNode(oppositionStartNode, _nodes[oppositionStartNode]);
        }
    }

    private void NextTurn()
    {
        // TODO: End -- check if player reached destination & is untracked.
        if (_player.currentNode == endNode && !_playerTracked)
        {
            Debug.Log("Win!");
            return;
        }

        _turn++;
        
        if (_turn % 2 != 0)
        {
            _player.NextTurn(PlayerMove, OnCheckPlayerVisibility(_opposition.currentNode));
        }
        else
        {
            _opposition.NextTurn(OppositionMove, OnCheckPlayerVisibility);
        }
    }

    private void OppositionMove(Node next)
    {
        _opposition.SetCurrentNode(next, _nodes[next]);
        NextTurn();
    }

    private void PlayerMove(Node next)
    {
        _player.SetCurrentNode(next, _nodes[next]);
        NextTurn();
    }
    
    private bool OnCheckPlayerVisibility(Node node)
    {
        return _nodes[node].Contains(_player.currentNode);    
    }
}