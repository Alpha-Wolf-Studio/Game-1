using UnityEngine;

[CreateAssetMenu(fileName = "Element_", menuName = "ScriptableObjects/Elements", order = 1)]
public class ElementData : ScriptableObject
{
    [SerializeField] private ELEMENT_TYPE type = ELEMENT_TYPE.EMPTY;
    [SerializeField] private GameObject prefab = null;

    public ELEMENT_TYPE Type { get => type; set => type = value; }
    public GameObject Prefab { get => prefab; set => prefab = value; }
}
