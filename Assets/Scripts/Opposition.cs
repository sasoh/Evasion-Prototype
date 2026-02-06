using Pixelplacement;
using UnityEngine;

public class Opposition : MonoBehaviour
{
    [SerializeField] private GameObject imageObserving;
    [SerializeField] private GameObject imageThinking;
    [SerializeField] private GameObject imageSearching;
    
    public Node currentNode;
    public Node lastNode;
    public bool wasSeeingPlayer;
    
    public void SetCurrentNode(Node newCurrentNode, bool seesPlayer, bool isInit = false)
    {
        lastNode = currentNode;
        currentNode = newCurrentNode;
        Tween.Position(
            transform,
            currentNode.transform.position,
            isInit ? 0.0f : AnimationProperties.MovementTweenDuration,
            0.0f
        );
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