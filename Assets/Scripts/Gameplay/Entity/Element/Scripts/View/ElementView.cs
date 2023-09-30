using UnityEngine;

public class ElementView : MonoBehaviour
{
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
