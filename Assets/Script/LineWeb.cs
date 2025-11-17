using UnityEngine;

public class LineWeb : MonoBehaviour
{
    public Transform webJoint;
    LineRenderer lineRenderer;
    Rigidbody2D rb;
    FixedJoint2D fixedJoint;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        rb = GetComponent<Rigidbody2D>();
        fixedJoint = GetComponent<FixedJoint2D>();
	}

    // Update is called once per frame
    void Update()
    {
        if (webJoint != null)
        {
            DrawRope(webJoint.position);
        }
    }

    public void DrawRope(Vector2 targetPosition)
    {
        if (lineRenderer == null)
            return;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, targetPosition);
    }

    public void ClearRope()
    {
        fixedJoint.connectedBody = rb;
        rb.linearDamping = 2.5f;
	}
}
