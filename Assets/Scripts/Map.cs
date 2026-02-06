using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Map : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject oppositionPrefab;
    [SerializeField] private GameObject destinationPrefab;
    [SerializeField] private GameObject mapNodesParent;
    [SerializeField] private Node startNode;
    [SerializeField] private Node[] oppositionStartNodes;
    [SerializeField] private Node destinationNode;
    [SerializeField] private GameObject victoryScreen;

    private Player _player;
    private readonly OppositionController _oppositionController = new();
    private readonly Dictionary<Node, HashSet<Node>> _nodes = new();
    private int _turn;

    private void Start()
    {
        Setup();
        NextTurn();
    }

    private void Setup()
    {
        victoryScreen.SetActive(false);
        SetupNodeLinks();
        SetupPlayer();
        SetupOpposition();
        SetupDestination();
    }

    private void SetupDestination()
    {
        var destinationInstance = Instantiate(destinationPrefab);
        destinationInstance.transform.position = destinationNode.transform.position;
    }

    private void SetupOpposition()
    {
        foreach (var node in oppositionStartNodes)
        {
            var oppositionInstance = Instantiate(oppositionPrefab);
            if (!oppositionInstance.TryGetComponent<Opposition>(out var o)) continue;
            _oppositionController.Add(o, node, OnCheckPlayerVisibility);
        }
    }

    private void SetupPlayer()
    {
        var playerInstance = Instantiate(playerPrefab);
        if (!playerInstance.TryGetComponent<Player>(out var player)) return;
        _player = player;
        _player.SetCurrentNode(startNode, _nodes[startNode]);
    }

    private void SetupNodeLinks()
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
    }

    private void NextTurn()
    {
        var playerSeen = _oppositionController.Opposition.Any(o => OnCheckPlayerVisibility(o.currentNode));
        if (CheckEndGame(playerSeen)) return;
        _turn++;

        if (_turn % 2 != 0)
        {
            _player.NextTurn(PlayerMove, playerSeen);
        }
        else
        {
            _oppositionController.NextTurn(AdjacentForNode, OnCheckPlayerVisibility, NextTurn);
        }
    }

    private bool CheckEndGame(bool playerSeen)
    {
        if (_player.currentNode != destinationNode || playerSeen) return false;
        victoryScreen.SetActive(true);
        return true;
    }

    private IReadOnlyCollection<Node> AdjacentForNode(Node n) => _nodes[n] ?? new HashSet<Node>();

    private void PlayerMove(Node next)
    {
        _player.SetCurrentNode(next, _nodes[next]);
        NextTurn();
    }

    private bool OnCheckPlayerVisibility(Node node)
    {
        var adjacentNodes = AdjacentForNode(node);
        if (adjacentNodes.Contains(_player.currentNode) || node == _player.currentNode) return true;

        // Prevent infinite loops while searching.
        var maxIterationDepth = 99;
        
        foreach (var adjacent in adjacentNodes)
        {
            var direction = (adjacent.transform.position - node.transform.position).normalized;
            var potential = new HashSet<Node>(AdjacentForNode(adjacent));
            potential.Remove(adjacent);
            while (potential.Count > 0)
            {
                maxIterationDepth--;
                if (maxIterationDepth <= 0)
                {
                    Debug.LogError("Max iteration depth reached, bailing out");
                    break;
                }

                var p = potential.First();
                potential.Remove(p);
                
                var potentialDirection = (p.transform.position - adjacent.transform.position).normalized;
                if (!AreCollinear(direction, potentialDirection)) continue;
                if (p == _player.currentNode) return true;

                var ap = AdjacentForNode(p);
                foreach (var a in ap)
                {
                    var aDirection = (a.transform.position - p.transform.position).normalized;
                    if (!AreCollinear(direction, aDirection)) continue;
                    potential.Add(a);
                }
            }
        }

        return false;
    }

    private static bool AreCollinear(Vector3 direction, Vector3 potentialDirection)
    {
        const float collinearityCoefficient = 0.95f;
        return Vector3.Dot(direction, potentialDirection) > collinearityCoefficient;
    }

    public void RestartGame() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
}