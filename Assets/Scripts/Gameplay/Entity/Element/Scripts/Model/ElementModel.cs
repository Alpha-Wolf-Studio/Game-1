using UnityEngine;

public class ElementModel
{
    private ELEMENT_TYPE type = ELEMENT_TYPE.EMPTY;
    private Vector2Int position = Vector2Int.zero;
    private int power = 1;

    public ELEMENT_TYPE Type { get => type; set => type = value; }
    public Vector2Int Position { get => position; set => position = value; }
    public int Power { get => power; set => power = value; }

    public ElementModel(ELEMENT_TYPE type, Vector2Int position, int power)
    {
        this.type = type;
        this.position = position;
        this.power = power;
    }
}
