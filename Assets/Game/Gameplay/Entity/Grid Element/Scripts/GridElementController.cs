using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Pool;

public class GridElementController : MonoBehaviour
{
    [SerializeField] private ElementData[] elementsData = null;
    [SerializeField] private SpawnRandomElementsHandler spawnRandomElementsHandler = null;

    [Header("------GRID CONFIGURATION-------")]
    [SerializeField] private Vector2Int size = Vector2Int.one;
    [SerializeField] private float cellDistance = 0f;
    [SerializeField] private Transform gridHolder = null;
    [SerializeField] private Transform poolHolder = null;

    private Dictionary<ELEMENT_TYPE, ObjectPool<ElementView>> elementPoolsDict = null;
    private ElementModel[,] gridElements = null;
    private List<ElementView> elementViews = null;

    #region INITIALIZATION
    public void Init()
    {
        elementViews = new List<ElementView>();

        CreateElementPools();
        CreateElementModels();
        InitSpawnRandomElementsHandler();
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

    private void CreateElementModels()
    {
        gridElements = new ElementModel[size.x, size.y];

        for (int i = 0; i < gridElements.GetLength(0); i++)
        {
            for (int j = 0; j < gridElements.GetLength(1); j++)
            {
                SetEmptyElement(new Vector2Int(i, j));
            }
        }
    }

    private void InitSpawnRandomElementsHandler()
    {
        ElementData[] respawnElements = elementsData.Where(e => e.CanRespawn).ToArray();
        spawnRandomElementsHandler.Init(respawnElements, GetElementsWithCondition,
            onSpawnElement: (elementModel) =>
            {
                return SpawnElement(elementModel, true);
            },
            GetWorldPosition);
    }
    #endregion

    #region SPAWN
    public void SpawnInitialElements()
    {
        //Testing
        ElementModel element1 = new ElementModel(ELEMENT_TYPE.WATER, new Vector2Int(0, 0));
        ElementModel element2 = new ElementModel(ELEMENT_TYPE.FIRE, new Vector2Int(1, 0));
        ElementModel element3 = new ElementModel(ELEMENT_TYPE.WIND, new Vector2Int(0, 1));
        ElementModel element4 = new ElementModel(ELEMENT_TYPE.DIRT, new Vector2Int(1, 1));

        SpawnElement(element1);
        SpawnElement(element2);
        SpawnElement(element3);
        SpawnElement(element4);
    }

    private ElementView SpawnElement(ElementModel elementModel, bool respawn = false)
    {
        gridElements[elementModel.Position.x, elementModel.Position.y] = elementModel;

        ElementView elementView = elementPoolsDict[elementModel.Type].Get();
        elementView.name = string.Format("Element {0} {1}", elementModel.Position.x, elementModel.Position.y);
        elementView.Position = elementModel.Position;

        if (!respawn)
        {
            elementView.SetPosition(GetWorldPosition(elementModel.Position));
        }

        return elementView;
    }
    #endregion

    #region AUXILIAR
    private void SetEmptyElement(Vector2Int pos)
    {
        gridElements[pos.x, pos.y] = new ElementModel(ELEMENT_TYPE.EMPTY, new Vector2Int(pos.x, pos.y));
    }

    public void UpdatePlayerElementMove()
    {

    }

    private Vector3 GetWorldPosition(Vector2Int position)
    {
        return new Vector3(position.x * cellDistance, 0f, position.y * cellDistance);
    }

    private List<ElementModel> GetElementsWithCondition(Predicate<ElementModel> condition)
    {
        List<ElementModel> elements = new List<ElementModel>();

        for (int i = 0; i < gridElements.GetLength(0); i++)
        {
            for (int j = 0; j < gridElements.GetLength(1); j++)
            {
                ElementModel element = gridElements[i, j];

                if (condition == null || condition(element))
                {
                    elements.Add(element);
                }
            }
        }

        return elements;
    }
    #endregion

    #region MOVING
    public bool CanMoveElement(Vector2Int originalPos, Vector2Int nextPos)
    {
        if (!IsValidPosition(nextPos))
        {
            return false;
        }

        ElementModel elem = gridElements[nextPos.x, nextPos.y];

        return elem.Type == ELEMENT_TYPE.EMPTY;
    }

    public void MoveElement(Vector2Int originalPos, Vector2Int nextPos)
    {
        gridElements[nextPos.x, nextPos.y] = new ElementModel(gridElements[originalPos.x, originalPos.y]);
        SetEmptyElement(originalPos);
    }

    private bool IsValidPosition(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < size.x && pos.y >= 0 && pos.y < size.y;
    }
    #endregion

    #region POOLING
    private ElementView CreateElementPrefab(ELEMENT_TYPE type)
    {
        ElementData elementData = elementsData.ToList().Find(e => e.Type == type);
        ElementView elementView = Instantiate(elementData.Prefab).GetComponent<ElementView>();
        elementView.Init(GetWorldPosition);

        return elementView;
    }

    private void GetElementView(ElementView elementView)
    {
        elementView.Get();
        elementViews.Add(elementView);

        elementView.transform.SetParent(gridHolder);
    }

    private void ReleaseElementView(ElementView elementView)
    {
        elementView.Release();
        elementViews.Remove(elementView);

        elementView.transform.SetParent(poolHolder);
    }

    private void DestroyElement(ElementView elementView)
    {
        Destroy(elementView.gameObject);
    }
    #endregion
}
