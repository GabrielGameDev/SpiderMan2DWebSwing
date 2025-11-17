using System.Threading.Tasks;
using UnityEngine;

public class WebController : MonoBehaviour
{
    public HingeJoint2D webJoint;
	public LayerMask attachableLayers;
	public LineRenderer lineRenderer;
	public float webSpeed = 20f;

	public float launchPullForce = 20f;

	public float jumpForce = 10f;
	public LayerMask groundLayers;
	public float groundDistance = 0.5f;
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
		
	}

	async void AttachWeb()
	{
		//Debug.Break();
		Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero, 0f, attachableLayers);
		if (hit.collider == null)
			return;


		// Verifica se está no chão antes de pular
		RaycastHit2D groundHit = Physics2D.Raycast(transform.position, Vector2.down, groundDistance, groundLayers);
		if (groundHit.collider != null)
			rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

		await Awaitable.WaitForSecondsAsync(0.1f);

		HingeJoint2D newJoint = Instantiate(webJoint, mouseWorldPos, Quaternion.identity);
		LineRenderer newLine = Instantiate(lineRenderer);
		lineRenderer.transform.position = transform.position;
		await AnimateWebShot(mouseWorldPos, newLine);

		

		//DrawRope(mouseWorldPos, newLine);
		newJoint.connectedBody = newLine.GetComponent<Rigidbody2D>();
		newLine.GetComponent<FixedJoint2D>().connectedBody = rb;

		currentWebJoint = newJoint;
		currentLineRenderer = newLine;

	}

	async Awaitable AnimateWebShot(Vector2 targetAnchor, LineRenderer line)
	{
		Vector2 startPosition = transform.position;
		float totalDistance = Vector2.Distance(startPosition, targetAnchor);
		
		float totalTime = totalDistance / webSpeed;
		float elapsedTime = 0f;

		// Animação Frame a Frame
		while (elapsedTime < totalTime)
		{
			elapsedTime += Time.deltaTime;
			float t = elapsedTime / totalTime; 

			
			Vector2 webTipPosition = Vector2.Lerp(startPosition, targetAnchor, t);

			
			line.positionCount = 2; 
			line.SetPosition(0, startPosition); 
			line.SetPosition(1, webTipPosition); 

			//await Awaitable.FixedUpdateAsync();
			
			Vector2 pullDirection = (targetAnchor - (Vector2)transform.position).normalized;
			
			rb.AddForce(pullDirection * launchPullForce, ForceMode2D.Force);

			await Awaitable.NextFrameAsync(); 
		}

		// Garante que o desenho chegue no ponto exato no final
		line.SetPosition(1, targetAnchor);
		line.transform.position = transform.position;
	}

	private void DrawRope(Vector2 mouseWorldPos, LineRenderer newLine)
	{
		newLine.SetPosition(0, transform.position);
		newLine.SetPosition(1, mouseWorldPos);
		//newLine.transform.position = transform.position;
	}
}
