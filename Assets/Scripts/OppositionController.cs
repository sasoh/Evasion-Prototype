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
        var playerSeen = false;
        foreach (var o in Opposition.Where(o => onCheckPlayerVisibility(o.currentNode)))
        {
            o.wasSeeingPlayer = true;
            playerSeen = true;
        }

        if (playerSeen)
        {
            foreach (var o in Opposition)
            {
                o.UpdateUI(true, false);
            }
         
            Debug.Log("Player visible, waiting.");   
            onTurnComplete();
            return;
        }

        var sawPlayer = Opposition.Where(o => o.wasSeeingPlayer).ToArray();
        if (sawPlayer.Any())
        {
            foreach (var os in sawPlayer)
            {
                os.wasSeeingPlayer = false;
                Debug.Log("Considering next move.");
                foreach (var o in Opposition)
                {
                    o.UpdateUI(false, true);
                }
            }
        }
        else
        {
            var foundPlayer = false;
            foreach (var o in Opposition)
            {
                o.UpdateUI(false, true);
                
                var adjacentNodes = new HashSet<Node>(adjacentForNode(o.currentNode));
                if (adjacentNodes.Count > 1)
                {
                    adjacentNodes.Remove(o.lastNode);
                }

                var randomDirection = adjacentNodes.ToArray()[Random.Range(0, adjacentNodes.Count)];
                o.SetCurrentNode(
                    randomDirection,
                    onCheckPlayerVisibility(randomDirection)
                );
                Debug.Log($"Going towards {randomDirection}.");
                foundPlayer = onCheckPlayerVisibility(randomDirection);
                if (foundPlayer) break;
            }

            if (foundPlayer)
            {
                foreach (var o in Opposition)
                {
                    o.UpdateUI(true, false);
                }
            }
        }

        onTurnComplete();
    }
}