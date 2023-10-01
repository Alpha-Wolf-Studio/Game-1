using System;
using System.Collections.Generic;

using UnityEngine;

public class SpawnRandomElementsHandler : MonoBehaviour
{
    [SerializeField] private float respawnTimeTarget = 0f;
    [SerializeField] private int respawnAmount = 0;
    [SerializeField] private float respawnPosY = 0f;
    [SerializeField] private float fallingElementTimeTarget = 0f;

    private ElementData[] respawnElements = null;

    private Func<Predicate<ElementModel>, List<ElementModel>> onGetElementsWithCondition = null;
    private Func<ElementModel, ElementView> onSpawnElement = null;
    private Func<Vector2Int, Vector3> onGetWorldPosition = null;
    private Action onCheckGridStatus = null;

    private bool startTimer = false;
    private float timer = 0f;

    private void Update()
    {
        UpdateRespawnTimer();
    }

    public void Init(ElementData[] respawnElements, Func<Predicate<ElementModel>, List<ElementModel>> onGetElementsWithCondition, 
        Func<ElementModel, ElementView> onSpawnElement, Func<Vector2Int, Vector3> onGetWorldPosition, Action onCheckGridStatus)
    {
        this.respawnElements = respawnElements;
        this.onGetElementsWithCondition = onGetElementsWithCondition;
        this.onSpawnElement = onSpawnElement;
        this.onGetWorldPosition = onGetWorldPosition;
        this.onCheckGridStatus = onCheckGridStatus;

        startTimer = true;
    }

    public void ToggleTimer(bool status)
    {
        startTimer = status;
    }

    private void UpdateRespawnTimer()
    {
        if (!startTimer)
        {
            return;
        }

        timer += Time.deltaTime;
        if (timer > respawnTimeTarget)
        {
            ToggleTimer(false);

            RespawnElements();
            RestartRespawnTimer();
        }
    }

    private void RestartRespawnTimer()
    {
        timer = 0f;
        ToggleTimer(true);
    }

    private void RespawnElements()
    {
        List<ElementModel> elementsToReplace = onGetElementsWithCondition(
            (element) =>
            {
                return element.Type == ELEMENT_TYPE.EMPTY;
            });

        for (int i = 0; i < respawnAmount; i++)
        {
            if (elementsToReplace.Count == 0)
            {
                break;
            }

            int randomIndex = UnityEngine.Random.Range(0, elementsToReplace.Count);
            ElementModel element = elementsToReplace[randomIndex];

            randomIndex = UnityEngine.Random.Range(0, respawnElements.Length);
            ElementData data = respawnElements[randomIndex];

            element.Type = data.Type;

            ElementView spawnElement = onSpawnElement.Invoke(element);
            spawnElement.SetPosition(onGetWorldPosition(element.Position) + new Vector3(0f, respawnPosY, 0f));
            spawnElement.Falling(fallingElementTimeTarget);

            elementsToReplace.Remove(element);
        }

        onCheckGridStatus?.Invoke();
    }
}
