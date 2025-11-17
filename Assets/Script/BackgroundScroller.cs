using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    public Transform leftBg, rightBg;
    float backgroundSizeX;
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        backgroundSizeX = GetComponent<BoxCollider2D>().size.x;		
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			if(collision.transform.position.x < transform.position.x)
			{
				rightBg.transform.position += Vector3.left * (backgroundSizeX * 3);
			}
			else
			{
				leftBg.transform.position += Vector3.right * (backgroundSizeX * 3);
			}
		}
	}
}
