using System;

using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private ElementView elementSelected = null;
    private int score = 0;
    private bool inputEnabled = false;

    private Action onFinishMove = null;
    private Action<Vector2Int, Vector2Int> onMoveElement = null;
    private Func<Vector2Int, Vector2Int, bool> onCanMoveElement = null;

    private void Update()
    {
        UpdateInputs();
    }

    public void Init(Action onFinishMove, Action<Vector2Int, Vector2Int> onMoveElement, Func<Vector2Int, Vector2Int, bool> onCanMoveElement)
    {
        this.onFinishMove = onFinishMove;
        this.onMoveElement = onMoveElement;
        this.onCanMoveElement = onCanMoveElement;
    }

    private void SelectElement(ElementView elementView)
    {
        elementSelected = elementView;
    }

    private void UnselectElement()
    {
        elementSelected = null;
    }

    private void MoveElement(Vector2Int direction)
    {
        Vector2Int originalPos = elementSelected.Position;
        Vector2Int nextPos = originalPos + direction;

        if (onCanMoveElement(originalPos, nextPos))
        {
            ToggleInput(false);

            onMoveElement?.Invoke(originalPos, nextPos);
            elementSelected.Move(nextPos,
                onFinishMove: () =>
                {
                    ToggleInput(true);
                    onFinishMove?.Invoke();
                });
        }
    }

    #region INPUTS
    private void UpdateInputs()
    {
        if (!inputEnabled)
        {
            return;
        }

        if (TrySelectElement(out ElementView elementView))
        {
            SelectElement(elementView);
        }

        if (TryGetDirectionMoveElement(out Vector2Int direction))
        {
            MoveElement(direction);
        }
    }

    private bool TrySelectElement(out ElementView view)
    {
        view = null;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                ElementView elementView = hit.transform.gameObject.GetComponent<ElementView>();
                if (elementView != null)
                {
                    view = elementView;
                    return true;
                }
            }

            UnselectElement();
        }

        return false;
    }

    private bool TryGetDirectionMoveElement(out Vector2Int direction)
    {
        direction = Vector2Int.zero;

        if (elementSelected == null)
        {
            return false;
        }

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            direction = Vector2Int.up;
        }

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            direction = Vector2Int.left;
        }

        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            direction = Vector2Int.down;
        }

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            direction = Vector2Int.right;
        }

        return direction != Vector2Int.zero;
    }

    public void ToggleInput(bool state)
    {
        inputEnabled = state;
    }
    #endregion
}