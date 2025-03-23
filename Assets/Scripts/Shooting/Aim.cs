using UnityEngine;
using UnityEngine.SceneManagement;

public class Aim : MonoBehaviour
{
    [SerializeField] Transform arm;  // The arm to rotate
    [SerializeField] Transform head; // The head to rotate
    [SerializeField] bool tracking;  // head and arm tracking
    private PlayerController playerController;
    private Animator animator;
    private Vector3 startingSize;
    private InputManager _input;
    private AudioManager _audio;
    private Vector3 mousePos;

    // For ghost trail 
    private bool isGhost = false;
    private bool hasTracked = false;

    void Start()
    {        
        startingSize = transform.localScale;
        playerController = PlayerController.Instance;
        animator = GetComponent<Animator>();
        _input = InputManager.Instance;
        _audio = AudioManager.Instance;
        tracking = SceneManager.GetActiveScene().name != "MainMenu";
    }

    void LateUpdate()
    {
        if (!playerController.IsAlive())
        {
            return;
        }
        if (tracking)
        {
            PlayerHeadTracking();
            PlayerAim();
        }
    }
    void PlayerHeadTracking()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("RollBack"))
        {
            return;
        }

        // Get the mouse position in world space (set z to 0 for 2D)
        
        if (MenuManager.IsPaused)
        {
            _input.SetMouseInput(MenuManager.BeforePausePosition);
        }

        mousePos = Camera.main.ScreenToWorldPoint(_input.MouseInput);
        mousePos.z = 0f;

        // Calculate the direction vector from the head to the mouse position
        Vector3 head_direction = mousePos - head.position;

        // Calculate the angle to rotate the head towards the mouse (in degrees)
        float head_angle = Mathf.Atan2(head_direction.y, head_direction.x) * Mathf.Rad2Deg;

        // Check if the character is flipped (scale.x < 0) and adjust accordingly
        if (transform.localScale.x < 0)
        {
            head_angle = head_angle += 180f;
        }


        // // Limit head rotation angle
        if (head_angle >= 45 && head_angle <= 90)
        {
            head.rotation = Quaternion.Euler(0, 0, 45);
        }
        else if (head_angle >= -90 && head_angle <= -45)
        {
            head.rotation = Quaternion.Euler(0, 0, -45);
        }
        else if (head_angle >= 270 && head_angle <= 315)
        {
            head.rotation = Quaternion.Euler(0, 0, 315);
        }
        else if (head_angle >= -135 && head_angle <= -90)
        {
            head.rotation = Quaternion.Euler(0, 0, -45);
        }
        else if (head_angle <= 135 && head_angle >= 90)
        {
            head.rotation = Quaternion.Euler(0, 0, 45);
        }
        else
        {
            head.rotation = Quaternion.Euler(0, 0, head_angle);
        }
        // Debug.Log("MousePos.x: " + mousePos.x + " transform.position.x: " + transform.position.x);
        // Flip the character based on the mouse position (left or right)
        if (mousePos.x < transform.position.x)
        {
            // Mouse is on the left, character should face left
            if (transform.localScale.x > 0) // Only flip if not already flipped
            {
                transform.localScale = new Vector3(-startingSize.x, startingSize.y, startingSize.z);
            }
        }
        else
        {
            // Mouse is on the right, character should face right
            if (transform.localScale.x < 0) // Only flip if not already flipped
            {
                transform.localScale = new Vector3(startingSize.x, startingSize.y, startingSize.z);
            }
        }
    }
    void PlayerAim()
    {
        // Get the mouse position in world space (set z to 0 for 2D)
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(_input.MouseInput);
        mousePos.z = 0f;

        // Calculate the direction vector from the arm to the mouse position
        Vector3 arm_direction = mousePos - arm.position;

        // Calculate the angle to rotate the arm towards the mouse (in degrees)
        float arm_angle = Mathf.Atan2(arm_direction.y, arm_direction.x) * Mathf.Rad2Deg;

        // Check if the character is flipped (scale.x < 0) and adjust accordingly
        if (transform.localScale.x < 0)
        {
            arm_angle = arm_angle += 180f; // Invert the angle when the character is flipped
        }

        // Apply the calculated rotation to the arm with an offset
        arm.rotation = Quaternion.Euler(0, 0, arm_angle);

        // Flip the character based on the mouse position (left or right)
        if (mousePos.x < transform.position.x)
        {
            // Mouse is on the left, character should face left
            if (transform.localScale.x > 0) // Only flip if not already flipped
            {
                transform.localScale = new Vector3(-startingSize.x, startingSize.y, startingSize.z);
            }
        }
        else
        {
            // Mouse is on the right, character should face right
            if (transform.localScale.x < 0) // Only flip if not already flipped
            {
                transform.localScale = new Vector3(startingSize.x, startingSize.y, startingSize.z);
            }
        }
    }
    public void SetTracking(bool value)
    {
        tracking = value;
    }
}
