using System.Collections;
using System.Threading;
using UnityEngine;

public class GhostFollow : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    public float moveSpeed = 7f;
    public Animator animator;
    private bool Kill;
    public PlayerMovement player;
    public Transform Player;
    public Transform Hub;
    public float mSpeed;
    public float timer = 3f;
    private float onScreen;

    // Floating variables
    public float floatSpeed = 0.5f; // Speed of floating
    public float floatHeight = 0.5f; // Height of floating
    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
    }

    // Update is called once per frame
    void Update()
    {
        if (!player.collided)
        {
            onScreen += Time.deltaTime;
            if (onScreen >= timer) 
            {
                Vector3 direction = Hub.position - transform.position;
                direction.Normalize();
                transform.position += direction * mSpeed * Time.deltaTime;
            }
        }
        if (player.collided) 
        {
            Vector3 direction = Player.position - transform.position;
            direction.Normalize();
            transform.position += direction * mSpeed * Time.deltaTime;
            onScreen = 0f;
        }
        // Floating behavior
        float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        if (Kill)
        {
            StartCoroutine(StopAnimatorAfterDelay(0.5f));
        }
    }

    private void OnCollisionEnter2D(Collision2D other) 
    {
        if (other.gameObject.CompareTag("Player"))
        {
            animator.SetBool("isKilling", true);
            Kill = true;
            moveSpeed = 0f;
        }
    }

    IEnumerator StopAnimatorAfterDelay(float delay)
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);
        animator.SetBool("isKilling", false);
    }
}
