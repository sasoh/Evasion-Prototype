using UnityEngine;

public class Opposition : MonoBehaviour
{
    [SerializeField] private GameObject imageObserving;
    [SerializeField] private GameObject imageThinking;
    [SerializeField] private GameObject imageSearching;
    
    public Node currentNode;
    public Node lastNode;
    public bool wasSeeingPlayer;
    
    public void SetCurrentNode(Node newCurrentNode, bool seesPlayer)
    {
        lastNode = currentNode;
        currentNode = newCurrentNode;
        transform.position = currentNode.transform.position;
        wasSeeingPlayer = seesPlayer;
        if (!seesPlayer) return;
        UpdateUI(true, false);
    }

    public void UpdateUI(bool isObserving, bool isSearching)
    {
        imageObserving.SetActive(isObserving);
        imageSearching.SetActive(isSearching);
        imageThinking.SetActive(!isSearching && !isObserving);
    }
}