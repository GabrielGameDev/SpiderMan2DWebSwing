using System.Threading.Tasks;
using UnityEngine;

public class WebController : MonoBehaviour
{
    public HingeJoint2D webJoint;
	public LayerMask attachableLayers;
	public LineRenderer lineRenderer;

	public float jumpForce = 10f;
	Rigidbody2D rb;
	
	HingeJoint2D currentWebJoint;
	LineRenderer currentLineRenderer;

	private void Start()
	{
		rb = GetComponent<Rigidbody2D>();
	}

	private void Update()
	{
		if(Input.GetMouseButtonDown(0))
		{
			AttachWeb();
		}
		if(Input.GetMouseButtonDown(1) && currentWebJoint != null)
		{
			DettachWeb();
		}
		if (currentWebJoint != null)
		{
			DrawRope(currentWebJoint.transform.position, currentLineRenderer);
		}
	}

	void DettachWeb()
	{
		currentLineRenderer.GetComponent<FixedJoint2D>().connectedBody = null;
		currentLineRenderer = null;
		currentWebJoint = null;
		rb.AddForce(Vector2.up * jumpForce/2, ForceMode2D.Impulse);
		rb.AddForce(Vector2.right * jumpForce/2, ForceMode2D.Impulse);
	}

	async void AttachWeb()
	{
		Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero, 0f, attachableLayers);
		if (hit.collider == null)
			return;


		rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

		await Awaitable.WaitForSecondsAsync(0.2f);

		HingeJoint2D newJoint = Instantiate(webJoint, mouseWorldPos, Quaternion.identity);
		LineRenderer newLine = Instantiate(lineRenderer);
		currentWebJoint = newJoint;
		currentLineRenderer = newLine;

		DrawRope(mouseWorldPos, newLine);
		newJoint.connectedBody = newLine.GetComponent<Rigidbody2D>();
		newLine.GetComponent<FixedJoint2D>().connectedBody = rb;

	}

	private void DrawRope(Vector2 mouseWorldPos, LineRenderer newLine)
	{
		newLine.SetPosition(0, transform.position);
		newLine.SetPosition(1, mouseWorldPos);
	}
}
