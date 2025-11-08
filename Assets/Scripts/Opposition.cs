using UnityEngine;

public class Opposition : MonoBehaviour
{
    public Node currentNode;
    public Node lastNode;
    public bool wasSeeingPlayer;

    public void SetCurrentNode(Node newCurrentNode, bool seesPlayer)
    {
        lastNode = currentNode;
        currentNode = newCurrentNode;
        transform.position = currentNode.transform.position;
        wasSeeingPlayer = seesPlayer;
    }
}