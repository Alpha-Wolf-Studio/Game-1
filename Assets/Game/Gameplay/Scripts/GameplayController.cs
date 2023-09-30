using UnityEngine;

public class GameplayController : MonoBehaviour
{
    [SerializeField] private PlayerController playerController = null;
    [SerializeField] private GridElementController gridElementController = null;

    void Start()
    {
        playerController.Init(gridElementController.UpdatePlayerElementMove, gridElementController.CanMoveElement);
        gridElementController.Init();
    }
}
