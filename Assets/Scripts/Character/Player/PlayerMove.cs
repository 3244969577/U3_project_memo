using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMove : MonoBehaviour
{
    private Rigidbody2D rb;
    private float speed = 5f;
    private Vector2 velocity;
    private bool isMoving;
    private bool moveable = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        velocity = Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        isMoving = velocity != Vector2.zero;
        
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		this.UpdatePlayerRotation(mousePosition);

        if (isMoving)
        {
            // 检查是否可以移动
            if (moveable)
            {
                rb.MovePosition(rb.position + velocity * speed * Time.fixedDeltaTime);
            }
        }
    }

    private void UpdatePlayerRotation(Vector2 mousePosition)
	{
		Vector2 playerPos = rb.transform.position;
		Vector2 lookDirection = mousePosition - playerPos;

		if (lookDirection.x > 0)
		{
			this.transform.localEulerAngles = new Vector3(0, 0, 0);
		}
		else
		{
			this.transform.localEulerAngles = new Vector3(0, 180, 0);
		}
	}

    public void SetMoveable(bool moveable)
    {
        this.moveable = moveable;
    }
}
