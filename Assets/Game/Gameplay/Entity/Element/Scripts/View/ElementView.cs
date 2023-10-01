using System;
using System.Collections;

using UnityEngine;

public class ElementView : MonoBehaviour
{
    [SerializeField] private AnimationCurve curve = null;
    [SerializeField] private float movingTargetTime = 0f;

    private Func<Vector2Int, Vector3> onGetWorldPosition = null;

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

        StartCoroutine(MoveAnimation(transform.position, onGetWorldPosition(Position), movingTargetTime, onFinishMove));
    }

    public void Falling(float targetTime, Action onFinishFall = null)
    {
        StartCoroutine(MoveAnimation(transform.position, onGetWorldPosition(Position), targetTime, onFinishFall));
    }

    private IEnumerator MoveAnimation(Vector3 startPosition, Vector3 endPosition, float targetTime, Action onFinish = null)
    {
        float timer = 0f;

        while (timer < targetTime)
        {
            SetPosition(Vector3.Lerp(startPosition, endPosition, timer / targetTime));
            timer += Time.deltaTime;

            yield return null;
        }

        SetPosition(Vector3.Lerp(startPosition, endPosition, 1f));
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
