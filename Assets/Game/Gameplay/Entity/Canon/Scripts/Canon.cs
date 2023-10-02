using UnityEngine;

public class Canon : MonoBehaviour
{
    [SerializeField] private Animator animator = null;

    private readonly int shootKey = Animator.StringToHash("shoot");

    public void Shoot()
    {
        animator.SetTrigger(shootKey);
    }
}