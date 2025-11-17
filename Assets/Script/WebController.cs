using UnityEngine;

public class WebController : MonoBehaviour
{
    public HingeJoint2D webJoint;
	public LayerMask attachableLayers;

	private void Update()
	{
		if(Input.GetMouseButtonDown(0))
		{
			AttachWeb();
		}
	}

	void AttachWeb()
	{
		Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero, 0f, attachableLayers);
		if (hit.collider == null)
			return;
		HingeJoint2D newJoint = Instantiate(webJoint, mouseWorldPos, Quaternion.identity);
	}
}
