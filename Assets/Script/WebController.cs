using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class WebController : MonoBehaviour
{
    public HingeJoint2D webJoint;
	public LayerMask attachableLayers;
	public LineWeb lineWeb;
	public float webSpeed = 20f;

	public float launchPullForce = 20f;
	public float releaseImpulseMultiplier = 1.5f;

	public float jumpForce = 10f;
	public LayerMask groundLayers;
	public float groundDistance = 0.5f;
	Rigidbody2D rb;
	
	HingeJoint2D currentWebJoint;
	LineWeb currentLineRenderer;

	Animator animator;
	bool onGround = false;

	public static UnityAction<int> OnWebAttached;
	public static UnityAction OnFall;

	int websUsed = 0;
	
	bool canAttachWeb = true;
	public void SetCanAttachWeb(bool canAttach)
	{
		canAttachWeb = canAttach;
	}

	private void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
	}

	private void Update()
	{
		if (!canAttachWeb)
			return;

		if (Input.GetMouseButtonDown(0))
		{
			AttachWeb();
		}
		if(Input.GetMouseButtonDown(1) && currentWebJoint != null)
		{
			DettachWeb(true);
		}
		//if (currentWebJoint != null)
		//{
		//	DrawRope(currentWebJoint.transform.position, currentLineRenderer);
		//}

		// Verifica se está no chão
		RaycastHit2D groundHit = Physics2D.Raycast(transform.position, Vector2.down, groundDistance, groundLayers);
		onGround = groundHit.collider != null;
		animator.SetBool("onGround", onGround);

	}

	void DettachWeb(bool impulse)
	{
		if (currentLineRenderer == null)
			return;
		
		FixedJoint2D joint = currentLineRenderer.GetComponent<FixedJoint2D>();
		if (joint != null)
				joint.connectedBody = null;
				

		if (impulse)
		{
			Vector2 currentMomentum = rb.linearVelocity;
			rb.AddForce(currentMomentum * releaseImpulseMultiplier, ForceMode2D.Impulse);
		}		

	
		currentLineRenderer.ClearRope();
		currentLineRenderer = null;
		currentWebJoint = null;	
		

	}

	async void AttachWeb()
	{
		//Debug.Break();

		if (currentWebJoint != null)
			DettachWeb(true);
		
		animator.SetTrigger("web");
		Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero, 0f, attachableLayers);
		if (hit.collider == null)
			return;


		// Verifica se está no chão antes de pular
		if (onGround)
			rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

		await Awaitable.WaitForSecondsAsync(0.1f);
		websUsed++;
		OnWebAttached?.Invoke(websUsed);
		HingeJoint2D newJoint = Instantiate(webJoint, mouseWorldPos, Quaternion.identity);
		LineWeb newLine = Instantiate(lineWeb);
		lineWeb.transform.position = transform.position;
		lineWeb.DrawRope(transform.position);
		await AnimateWebShot(mouseWorldPos, newLine);
		
		newJoint.connectedBody = newLine.GetComponent<Rigidbody2D>();
		newLine.GetComponent<FixedJoint2D>().connectedBody = rb;

		currentWebJoint = newJoint;
		currentLineRenderer = newLine;
		currentLineRenderer.webJoint = currentWebJoint.transform;

	}

	async Awaitable AnimateWebShot(Vector2 targetAnchor, LineWeb line)
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

			
			Vector2 webTipPosition = Vector2.Lerp(transform.position, targetAnchor, t);

			line.transform.position = transform.position;
			line.DrawRope(webTipPosition);

			//line.positionCount = 2; 
			//line.SetPosition(0, startPosition); 
			//line.SetPosition(1, webTipPosition); 

			//await Awaitable.FixedUpdateAsync();
			
			Vector2 pullDirection = (targetAnchor - (Vector2)transform.position).normalized;
			
			rb.AddForce(pullDirection * launchPullForce, ForceMode2D.Force);

			await Awaitable.NextFrameAsync(); 
		}

		// Garante que o desenho chegue no ponto exato no final
		line.DrawRope(targetAnchor);
		line.transform.position = transform.position;
		
	}


	private void OnCollisionEnter2D(Collision2D collision)
	{		
		DettachWeb(false);	
		OnFall?.Invoke();
	}
}
