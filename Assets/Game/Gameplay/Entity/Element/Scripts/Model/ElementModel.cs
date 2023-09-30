using UnityEngine;

public class ElementModel
{
    private ELEMENT_TYPE type = ELEMENT_TYPE.EMPTY;
    private Vector2Int position = Vector2Int.zero;

    public ELEMENT_TYPE Type { get => type; set => type = value; }
    public Vector2Int Position { get => position; set => position = value; }

    public ElementModel(ELEMENT_TYPE type, Vector2Int position)
    {
        this.type = type;
        this.position = position;
    }
}
