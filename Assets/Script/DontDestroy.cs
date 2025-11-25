using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    public static DontDestroy instance;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}
}
