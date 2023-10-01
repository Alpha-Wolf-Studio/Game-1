using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Pool;

using Newtonsoft.Json;

public class GridElementController : MonoBehaviour
{
    [SerializeField] private ElementData[] elementsData = null;
    [SerializeField] private SpawnRandomElementsHandler spawnRandomElementsHandler = null;

    [Header("------GRID CONFIGURATION-------")]
    [SerializeField] private Vector2Int size = Vector2Int.one;
    [SerializeField] private float cellDistance = 0f;
    [SerializeField] private Transform gridHolder = null;
    [SerializeField] private Transform poolHolder = null;
    [SerializeField] private GameObject tilePrefab;

    [Header("------TESTING-------")]
    [SerializeField] private bool applyOverride = false;
    [SerializeField] private TextAsset overrideJsonLevel = null;

    private Dictionary<ELEMENT_TYPE, ObjectPool<ElementView>> elementPoolsDict = null;
    private ElementModel[,] gridElements = null;
    private List<ElementView> elementViews = null;

    private Action<bool> onFinishLevel = null;

    #region INITIALIZATION
    public void Init(Action<bool> onFinishLevel)
    {
        elementViews = new List<ElementView>();

        this.onFinishLevel = onFinishLevel;

        CreateElementPools();
        CreateElementModels();
        CreateTilesModels();
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

    private void CreateTilesModels()
    {
        for (int i = 0; i < gridElements.GetLength(0); i++)
        {
            for (int j = 0; j < gridElements.GetLength(1); j++)
            {
                Instantiate(tilePrefab, new Vector3(i*2, 0, j*2),Quaternion.identity);
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
            GetWorldPosition, CheckGridStatus);
    }
    #endregion

    #region SPAWN
    public void StartLevel(string jsonLevel)
    {
        if (applyOverride)
        {
            LoadLevel(overrideJsonLevel.text);
            return;
        }

        LoadLevel(jsonLevel);
    }

    private void LoadLevel(string jsonLevel)
    {
        LevelData level = JsonConvert.DeserializeObject<LevelData>(jsonLevel);

        for (int i = 0; i < level.tileList.Count; i++)
        {
            ELEMENT_TYPE elementType = level.tileList[i].element;

            if (elementType != ELEMENT_TYPE.EMPTY)
            {
                Vector2Int pos = level.tileList[i].gridPos;
                ElementModel spawnElement = new ElementModel(elementType, pos);

                SpawnElement(spawnElement);
            }
        }
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
        SetElement(ELEMENT_TYPE.EMPTY, new Vector2Int(pos.x, pos.y));
    }

    public void UpdatePlayerElementMove()
    {
        CheckGridStatus();
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

    public ELEMENT_TYPE GetResultOfCombination(ELEMENT_TYPE elementHandled, ELEMENT_TYPE elementToMerge)
    {
        foreach (ElementData data in elementsData)
        {
            if (data.Type == elementHandled)
            {
                foreach (ElementCombination combination in data.Combinations)
                {
                    if (elementToMerge == combination.element2)
                    {
                        return combination.result;
                    }
                }
            }
        }
        return ELEMENT_TYPE.INVALID;
    }

    private void CheckGridStatus()
    {
        List<ElementModel> elements = GetElementsWithCondition(
            condition: (element) =>
            {
                return element.Type == ELEMENT_TYPE.EMPTY || element.Type == ELEMENT_TYPE.INVALID;
            });

        if (elements.Count == 0)
        {
            EndLevel(false);
        }
    }

    private void EndLevel(bool winPlayer)
    {
        onFinishLevel?.Invoke(winPlayer);
        spawnRandomElementsHandler.ToggleTimer(false);
    }

    private void SetElement(ELEMENT_TYPE type, Vector2Int pos)
    {
        gridElements[pos.x, pos.y] = new ElementModel(type, pos);
    }
    #endregion

    #region MOVING
    public bool CanMoveElement(Vector2Int originalPos, Vector2Int nextPos)
    {
        return (IsValidPosition(nextPos) && GetResultOfCombination(gridElements[originalPos.x, originalPos.y].Type, gridElements[nextPos.x, nextPos.y].Type) != ELEMENT_TYPE.INVALID);
    }

    public void MoveElement(Vector2Int originalPos, Vector2Int nextPos)
    {
        SetElement(GetResultOfCombination(gridElements[originalPos.x, originalPos.y].Type, gridElements[nextPos.x, nextPos.y].Type), new Vector2Int(nextPos.x, nextPos.y));
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
