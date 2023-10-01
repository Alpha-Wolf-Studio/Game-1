using System;
using System.Collections;

using UnityEngine;

public class ElementView : MonoBehaviour
{
    [SerializeField] private AnimationCurve curve = null;
    [SerializeField] private float movingTargetTime = 0f;
    [SerializeField] private float moveDelay = 0f;
    [SerializeField] private float endJumpDelay = 0f;
    [SerializeField] private Animator animator = null;

    private Func<Vector2Int, Vector3> onGetWorldPosition = null;

    private readonly int startJumpKey = Animator.StringToHash("start_jump");
    private readonly int loopJumpKey = Animator.StringToHash("loop_jump");
    private readonly int endJumpKey = Animator.StringToHash("end_jump");

    public Vector2Int Position { get; set; }

    public void Init(Func<Vector2Int, Vector3> onGetWorldPosition)
    {
        this.onGetWorldPosition = onGetWorldPosition;
    }

    public void Spawn()
    {

    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    #region MOVING
    public void Move(Vector2Int nextPos, Action onFinishMove = null)
    {
        Position = nextPos;

        animator?.SetTrigger(startJumpKey);
        StartCoroutine(MoveAnimation(transform.position, onGetWorldPosition(Position), movingTargetTime, moveDelay, onFinish: onFinishMove));
    }

    public void Falling(float targetTime, Action onFinishFall = null)
    {
        animator?.SetTrigger(loopJumpKey);
        StartCoroutine(MoveAnimation(transform.position, onGetWorldPosition(Position), targetTime, onFinish: onFinishFall));
    }

    private IEnumerator MoveAnimation(Vector3 startPosition, Vector3 endPosition, float targetTime, float delay = 0f, Action onFinish = null)
    {
        float timer = 0f;

        yield return new WaitForSeconds(delay);

        while (timer < targetTime)
        {
            SetPosition(Vector3.Lerp(startPosition, endPosition, timer / targetTime));
            timer += Time.deltaTime;

            yield return null;
        }

        SetPosition(Vector3.Lerp(startPosition, endPosition, 1f));
        animator?.SetTrigger(endJumpKey);

        yield return new WaitForSeconds(endJumpDelay);

        onFinish?.Invoke();
    }
    #endregion

    #region POOLING
    public void Get()
	{
		gameObject.SetActive(true);
	}

    public void Release()
    {
        gameObject.SetActive(false);
    }
    #endregion
}
