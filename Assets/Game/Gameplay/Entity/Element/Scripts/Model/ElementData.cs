using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Element_", menuName = "ScriptableObjects/Elements", order = 1)]
public class ElementData : ScriptableObject
{
    [SerializeField] private ELEMENT_TYPE type = ELEMENT_TYPE.EMPTY;
    [SerializeField] private GameObject prefab = null;
    [SerializeField] private bool canRespawn = false;
    [SerializeField] private List<ElementCombination> combinations = new List<ElementCombination>();

    public List<ElementCombination> Combinations { get => combinations;}
    public bool CanRespawn { get => canRespawn; }
    public GameObject Prefab { get => prefab; }
    public ELEMENT_TYPE Type { get => type; set => type = value; }
}

[System.Serializable]
public class ElementCombination
{
    public ELEMENT_TYPE element2;
    public ELEMENT_TYPE result;
}
