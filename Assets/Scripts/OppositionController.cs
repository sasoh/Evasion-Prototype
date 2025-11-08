using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class OppositionController
{
    public readonly List<Opposition> Opposition = new();

    public void Add(
        Opposition opposition,
        Node node,
        Func<Node, bool> onCheckPlayerVisibility
    )
    {
        opposition.SetCurrentNode(node, onCheckPlayerVisibility(node));
        Opposition.Add(opposition);
    }

    public void NextTurn(
        Func<Node, IReadOnlyCollection<Node>> adjacentForNode,
        Func<Node, bool> onCheckPlayerVisibility,
        Action onTurnComplete
    )
    {
        foreach (var o in Opposition)
        {
            if (onCheckPlayerVisibility(o.currentNode))
            {
                o.wasSeeingPlayer = true;
                Debug.Log("Seeing player, skip turn...");
                break;
            }

            if (o.wasSeeingPlayer)
            {
                o.wasSeeingPlayer = false;
                Debug.Log("Considering next move");
                break;
            }

            // move to a random position as long as it's not where we came from
            var adjacentNodes = new HashSet<Node>(adjacentForNode(o.currentNode));
            if (adjacentNodes.Count > 1)
            {
                // adjacentNodes.Remove(o.lastNode);
            }
            var randomDirection = adjacentNodes.ToArray()[Random.Range(0, adjacentNodes.Count)];
            o.SetCurrentNode(
                randomDirection,
                onCheckPlayerVisibility(randomDirection)
            );
            Debug.Log($"Going towards {randomDirection}");
        }

        onTurnComplete();
    }
}