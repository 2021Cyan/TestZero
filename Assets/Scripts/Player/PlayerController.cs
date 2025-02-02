using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Player Stats
    public float movePower = 0f;
    public float walkPower = 0f;
    public float dodgePower = 0f;
    public float jumpPower = 0f;
    public float level = 0f;
    public float exp = 0f;
    public float hp = 100f;
    public float resource = 0f;

    // Cooldown time in seconds
    public float dodgeCooldown = 0.1f;
    private float lastDodgeTime = 0f;

    private Rigidbody2D rb;
    private Animator anim;
    Vector3 movement;
    private int direction = 1;
    bool isJumping = false;
    bool isSliding = false;
    bool isRolling = false;

    private bool alive = true;
    private Camera maincam;
    private Vector3 mousePos;

    [SerializeField] public RandomizeSprites rs;

    void Start()
    {
        maincam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // Check if falling, increase gravity scale when falling
        if (rb.linearVelocity.y < 0)
        {
            rb.gravityScale = 5f;
        }
        else if (rb.linearVelocity.y > 0)
        {
            rb.gravityScale = 1f;
        }
        Restart();
        if (alive)
        {
            mousePos = maincam.ScreenToWorldPoint(Input.mousePosition);
            //Hurt();
            Dodge();
            Die();
            Jump();
            Move();
            Randomize();

        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        anim.SetBool("isJump", false);
    }

    void Move()
    {
        if (isRolling || isSliding) return; 

        Vector3 moveVelocity = Vector3.zero;
        anim.SetBool("isRun", false);
        anim.SetBool("isWalkBack", false);

        if (Input.GetAxisRaw("Horizontal") < 0)
        {
            // Mouse is on the left, character should face left
            if (mousePos.x < transform.position.x)
            {
                direction = -1;
                moveVelocity = Vector3.left;
                transform.localScale = new Vector3(direction, 1, 1);
                if (!anim.GetBool("isJump"))
                    anim.SetBool("isRun", true);
            }
            else
            {
                direction = 1;
                moveVelocity = Vector3.left;
                transform.localScale = new Vector3(direction, 1, 1);
                if (!anim.GetBool("isJump"))
                    anim.SetBool("isWalkBack", true);
            }
        }
        if (Input.GetAxisRaw("Horizontal") > 0)
        {
            if (mousePos.x < transform.position.x)
            {
                direction = -1;
                moveVelocity = Vector3.right;
                transform.localScale = new Vector3(direction, 1, 1);
                if (!anim.GetBool("isJump"))
                    anim.SetBool("isWalkBack", true);

            }
            else
            {
                direction = 1;
                moveVelocity = Vector3.right;
                transform.localScale = new Vector3(direction, 1, 1);
                if (!anim.GetBool("isJump"))
                    anim.SetBool("isRun", true);
            }
        }
        if (anim.GetBool("isWalkBack"))
        {
            transform.position += moveVelocity * walkPower * Time.deltaTime;
        }
        else
        {
            transform.position += moveVelocity * movePower * Time.deltaTime;
        }
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !anim.GetBool("isJump"))
        {
            isJumping = true;
            anim.SetBool("isJump", true);
        }
        if (!isJumping)
        {
            return;
        }

        rb.linearVelocity = Vector2.zero; // Stop any current motion before jumping

        Vector2 jumpVelocity = new Vector2(0, jumpPower);
        rb.AddForce(jumpVelocity, ForceMode2D.Impulse);

        isJumping = false;
    }

    void Dodge()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (isRolling || isSliding) return;

            if(Input.GetAxisRaw("Horizontal") < 0)
            {

            }
            Vector3 dodgeVelocity = Vector3.forward * direction;

            if (anim.GetBool("isWalkBack"))
            {
                isRolling = true;
                rb.linearVelocity = Vector2.zero;
                anim.SetTrigger("roll");
                transform.position += dodgeVelocity * dodgePower * Time.deltaTime;
                StartCoroutine(ResetDodge(anim.GetCurrentAnimatorStateInfo(0).length, "isDodging"));
            }
            if (anim.GetBool("isRun"))
            {
                rb.linearVelocity = Vector2.zero;
                anim.SetTrigger("slide");
                transform.position += dodgeVelocity * dodgePower * Time.deltaTime;
                StartCoroutine(ResetDodge(anim.GetCurrentAnimatorStateInfo(0).length, "isSliding"));
            }

        }
    }

    IEnumerator ResetDodge(float animationDuration, string actionType)
    {
        // Wait for the animation to finish
        yield return new WaitForSeconds(animationDuration);

        // Reset the appropriate flag
        if (actionType == "isDodging")
        {
            isRolling = false;
        }
        else if (actionType == "isSliding")
        {
            isSliding = false;
        }
    }

    void Hurt()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            anim.SetTrigger("hurt");
            if (direction == 1)
                rb.AddForce(new Vector2(-5f, 1f), ForceMode2D.Impulse);
            else
                rb.AddForce(new Vector2(5f, 1f), ForceMode2D.Impulse);
        }
    }

    void Randomize()
    {
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            rs.RandomizeParts();
        }
    }

    void Die()
    {
        if (hp <= 0)
        {
            anim.SetTrigger("die");
            alive = false;
        }
    }

    void Restart()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            anim.SetTrigger("idle");
            alive = true;
        }
    }

    // Increase EXP and handle level up
    public void OnEnemyKilled()
    {
        // Add EXP and resources
        exp += 20f;
        resource += 160f;
        Debug.Log("Enemy killed. current exp : " + exp);
        Debug.Log("Enemy killed. current resource: " + resource);

        // Check if EXP reaches 100 for a level up
        if (exp >= 100f)
        {
            LevelUp();
        }
    }

    // Levels up the character
    void LevelUp()
    {
        level++;         // Increase the level
        exp = 0f;        // Reset EXP after leveling up
        Debug.Log("Level Up! New Level: " + level);
    }

}
