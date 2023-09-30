using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

public class GridElementController : MonoBehaviour
{
    [SerializeField] private Vector2Int size = Vector2Int.one;
    [SerializeField] private float cellDistance = 0f;
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

    #region SPAWN
    private void SpawnInitialElements()
    {

    }

    private void SpawnElement(ELEMENT_TYPE type, Vector2Int position)
    {
        gridElements[position.x, position.y] = new ElementModel(type, position);

        ElementView elementView = elementPoolsDict[type].Get();
    }
    #endregion

    #region AUXILIAR
    public bool CanMoveElement(Vector2Int originalPos, Vector2Int nextPos)
    {
        ElementModel elem = gridElements[nextPos.x, nextPos.y];

        return elem.Type == ELEMENT_TYPE.EMPTY;
    }

    public void UpdatePlayerElementMove()
    {

    }

    private Vector2 GetWorldPosition(Vector2Int position)
    {
        return new Vector2(position.x * cellDistance, position.y * cellDistance);
    }
    #endregion

    #region POOLING
    private ElementView CreateElementPrefab(ELEMENT_TYPE type)
    {
        ElementView elementView = Instantiate(elementPoolsDict[type].Get()).GetComponent<ElementView>();
        elementView.Init(GetWorldPosition);

        return elementView;
    }

    private void GetElementView(ElementView elementView)
    {
        elementView.Get();
        elementViews.Add(elementView);
    }

    private void ReleaseElementView(ElementView elementView)
    {
        elementView.Release();
        elementViews.Remove(elementView);
    }

    private void DestroyElement(ElementView elementView)
    {
        Destroy(elementView.gameObject);
    }
    #endregion
}
