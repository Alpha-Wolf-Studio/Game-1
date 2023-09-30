using System;
using System.Collections;

using UnityEngine;

public class ElementView : MonoBehaviour
{
    [SerializeField] private AnimationCurve curve = null;
    [SerializeField] private float movingTargetTime = 0f;

    private Func<Vector2Int, Vector2> onGetWorldPosition = null;

    public Vector2Int Position { get; set; }

    public void Init(Func<Vector2Int, Vector2> onGetWorldPosition)
    {
        this.onGetWorldPosition = onGetWorldPosition;
    }

    #region MOVING
    public void Move(Vector2Int direction, Action onFinishMove = null)
    {
        Position += direction;

        StartCoroutine(MoveAnimation(transform.position, onGetWorldPosition(Position), onFinishMove));
    }

    private IEnumerator MoveAnimation(Vector2 startPosition, Vector2 endPosition, Action onFinishMove = null)
    {
        float timer = 0f;

        while (timer < movingTargetTime)
        {
            transform.position = Vector2.Lerp(startPosition, endPosition, timer / movingTargetTime);

            yield return null;
        }

        transform.position = Vector2.Lerp(startPosition, endPosition, 1f);
        onFinishMove?.Invoke();
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
