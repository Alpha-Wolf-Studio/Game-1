using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Pool;

public class GridElementController : MonoBehaviour
{
    [SerializeField] private Vector2Int size = Vector2Int.one;
    [SerializeField] private ElementData[] elementsData = null;

    private Dictionary<ELEMENT_TYPE, ObjectPool<ElementView>> elementPoolsDict = null;
    private ElementModel[,] gridElements = null;
    private List<ElementView> elementViews = null;

    #region INITIALIZATION
    public void Init()
    {
        gridElements = new ElementModel[size.x, size.y];
        elementViews = new List<ElementView>();

        CreateElementPools();
    }

    private void CreateElementPools()
    {
        elementPoolsDict = new Dictionary<ELEMENT_TYPE, ObjectPool<ElementView>>();

        for (int i = 0; i < elementsData.Length; i++)
        {
            ElementData data = elementsData[i];
            ObjectPool<ElementView> pool = new ObjectPool<ElementView>(createFunc: () => CreateElementPrefab(data.Type), GetElementView, ReleaseElementView, DestroyElement);

            elementPoolsDict.Add(data.Type, pool);
        }
    }
    #endregion

    #region POOLING
    private ElementView CreateElementPrefab(ELEMENT_TYPE type)
    {
        return Instantiate(elementPoolsDict[type].Get());
    }

    private void GetElementView(ElementView elementView)
    {
        elementView.Get();
    }

    private void ReleaseElementView(ElementView elementView)
    {
        elementView.Release();
    }

    private void DestroyElement(ElementView elementView)
    {
        Destroy(elementView.gameObject);
    }
    #endregion
}
