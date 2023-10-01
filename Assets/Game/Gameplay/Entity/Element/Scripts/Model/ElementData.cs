using UnityEngine;

[CreateAssetMenu(fileName = "Element_", menuName = "ScriptableObjects/Elements", order = 1)]
public class ElementData : ScriptableObject
{
    [SerializeField] private ELEMENT_TYPE type = ELEMENT_TYPE.EMPTY;
    [SerializeField] private GameObject prefab = null;
    [SerializeField] private bool canRespawn = false;

    public ELEMENT_TYPE Type { get => type; }
    public GameObject Prefab { get => prefab; }
    public bool CanRespawn { get => canRespawn; }
}
