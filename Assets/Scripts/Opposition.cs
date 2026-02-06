using UnityEngine;

public class Opposition : MonoBehaviour
{
    [SerializeField] private GameObject imageObserving;
    [SerializeField] private GameObject imageThinking;
    [SerializeField] private GameObject imageSearching;
    [SerializeField] private GameObject carImagePanel;
    
    public Node currentNode;
    public Node lastNode;
    public bool wasSeeingPlayer;
    
    public void SetCurrentNode(Node newCurrentNode, bool seesPlayer, bool isInit = false)
    {
        CarAnimator.Animate(
            transform,
            carImagePanel,
            currentNode? currentNode.transform.position : newCurrentNode.transform.position,
            newCurrentNode.transform.position,
            isInit
        );
        lastNode = currentNode;
        currentNode = newCurrentNode;
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