using UnityEngine;

public class ElementView : MonoBehaviour
{
    public Vector2Int Position { get; set; }

	#region POOLING
	public void Get()
	{
		gameObject.SetActive(true);
	}

    public void Release()
    {
        gameObject.SetActive(false);
    }
    #endregion
}
