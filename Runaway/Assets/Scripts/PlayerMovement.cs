using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public float jumpHeight = 10f; // Adjust the jump height as needed
    [SerializeField] private Rigidbody2D rb;
    private bool isGrounded;
    private bool candoublejump;
    [SerializeField] private Animator animator;
    public GameObject failmenu;
    public Button failmenuDefault;
    public float moveSpeed = 7f;
    public GameMenu gameMenu;
    public CapsuleCollider2D capsuleCollider;
    public bool Dead = false;
    public bool collided = false;
    public VolumeProfile gameVolumeProfile;
    public VolumeProfile uiVolumeProfile;
    public ScoreManager scoreManager;

    public Volume volume;
    public bool stopInput = false;

    void Update()
    {
        // Check if the player dies
        if (Dead)
        {
            // Stop the animator from playing any further animations after a delay
            StartCoroutine(StopAnimatorAfterDelay(0.5f)); // Adjust the delay as needed
        }

        if (!stopInput) 
        {
            if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump(jumpHeight);
            candoublejump = true;
        }
        else if (Input.GetButtonDown("Jump") && candoublejump)
        {
            Jump(jumpHeight);
            candoublejump = false;
        }

        if (Input.GetButton("Slide") && isGrounded) 
        {
            animator.SetBool("isSliding", true);
            if (capsuleCollider != null)
            {
                capsuleCollider.direction = CapsuleDirection2D.Horizontal;
                // Set the new size of the collider
                capsuleCollider.size = new Vector2(3.63647f, 1.976593f); // Change the size values as needed

                // Set the new center offset of the collider
                capsuleCollider.offset = new Vector2(0.08165684f, -0.8676774f); // Change the offset values as needed
            }
        }
        if (Input.GetButtonUp("Slide"))
        {
            animator.SetBool("isSliding", false);
            if (capsuleCollider != null)
            {
                capsuleCollider.direction = CapsuleDirection2D.Vertical;
                // Set the new size of the collider
                capsuleCollider.size = new Vector2(2.109974f, 5.07f); // Change the size values as needed

                // Set the new center offset of the collider
                capsuleCollider.offset = new Vector2(-0.05666754f, 0f); // Change the offset values as needed
            }
        }
        }

        // Update isJumping based on the grounded state
        animator.SetBool("isJumping", !isGrounded);
        rb.velocity = new Vector2(moveSpeed,rb.velocity.y);
    }

    private void Jump(float height)
    {
        float jumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(Physics2D.gravity.y) * height);
        rb.velocity = new Vector2(rb.velocity.x, jumpVelocity);
    }

    private void OnCollisionEnter2D(Collision2D other) 
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            candoublejump = false; // Reset double jump ability when grounded
        }

        if (other.gameObject.CompareTag("Groundavoid"))
        {
            collided = true;
            foreach (ContactPoint2D contact in other.contacts)
            {
                Vector2 normal = contact.normal;

                // Determine the side of collision based on the normal
                if (normal == Vector2.up)
                {
                    isGrounded = true;
                }
                else if (normal == Vector2.down)
                {
                    isGrounded = false;
                }
                else if (normal == Vector2.left)
                {
                    moveSpeed = 0;
                    isGrounded = true;
                }
                else if (normal == Vector2.right)
                {
                    moveSpeed = 0;
                    isGrounded = true;
                }
                else
                {
                    moveSpeed = 0;
                    isGrounded = true;
                }
            }
        }

        if (other.gameObject.CompareTag("Deathzone"))
        {
            scoreManager.stopCount = true;
            scoreManager.SaveScore();
            stopInput = true;
            collided = true;
            StartCoroutine(AnimatorAfterDelay(0.5f));
            StartCoroutine(MenuDelay(1f));
        }

    }

     IEnumerator StopAnimatorAfterDelay(float delay)
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);
        
        // Disable the animator
        animator.enabled = false;
    }

    IEnumerator AnimatorAfterDelay(float delay)
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);
        
        animator.SetBool("isDead", true);
        Dead = true;
        moveSpeed = 0f;
    }

    IEnumerator MenuDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        volume.profile = uiVolumeProfile;
        gameMenu.menumode = 4;
        failmenu.SetActive(true);
        failmenuDefault.Select();
    }

    private void OnCollisionExit2D(Collision2D other) 
    {
        if (other.gameObject.CompareTag("Ground")) 
        {
            isGrounded = false;
            candoublejump = true; // Reset double jump ability when grounded
        }

        if (other.gameObject.CompareTag("Groundavoid"))
        {
            collided = false;
            moveSpeed = 7f;
            isGrounded = false;
            candoublejump = true; // Reset double jump ability when grounded
        }
    }
}
