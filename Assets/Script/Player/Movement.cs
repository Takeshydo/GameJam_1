using System.Collections;
using UnityEditor;
using UnityEngine;


public class Movement:MonoBehaviour
{
    //DATA for actions
    public float speed;
    public float jumpForce;
    public float acceleration = 2f;
        //dash parameter
    public float dashPower = 7f;
    public float dashTimer;
    public float dashCooldown;
    private bool canDash = true;
    private bool isDashing;
        //tp parameter
    private float tpOffset = 0.7f;
    private float lastDirection = 1f;
     
    //DATA of Status
    public bool isGrounded;
    public bool isTPset;
    
    //DATA for Object
    private Rigidbody2D rb;
    public GameObject TPMarkPrefabs;
    private GameObject currentTPMark;
    
    private void Start()
    {
       rb = GetComponent<Rigidbody2D>();
    }
    
    private void Update()
    {
        if(isDashing) return; //Stop all input during dash
        
        float direction = Input.GetAxisRaw("Horizontal");

        if (direction != 0)
        {
            lastDirection = direction;
            Move(direction);
        }
        
            
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && direction != 0)
        {
            StartCoroutine(Dash(direction));
        }

        if (Input.GetKeyDown(KeyCode.Q)) 
        {
            if (!isTPset)
            {
                TPSet();
            }
            else
            {
                TPAction();
            }
        }
        
    }
    

    private void Move(float direction)
    {
        if (!isDashing)
        {
            float targetSpeed = direction * speed;
            float newSpeed = Mathf.Lerp(rb.linearVelocity.x, targetSpeed, acceleration * Time.deltaTime);
            rb.linearVelocity = new Vector2(newSpeed, rb.linearVelocity.y);
        }
    }

    private void Jump()
    {
        isGrounded = false;
        rb.AddForce(Vector2.up * jumpForce,  ForceMode2D.Impulse);
    }

    private IEnumerator Dash(float direction)
    {
        canDash = false;
        isDashing = true;
        
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        
        rb.linearVelocity= new Vector2(direction * dashPower, 0f);
        yield return new WaitForSeconds(dashTimer);
        
        rb.gravityScale = originalGravity;
        isDashing = false;
        
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
    
    
    
    //Ground Detection 
    private void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.CompareTag("Ground")) isGrounded = true;
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("Ground")) isGrounded = false;
    }
    
    //TP 

    public void TPSet()
    { 
        Vector3 directionVector = new Vector3(lastDirection, 0, 0);
        
        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionVector, tpOffset, LayerMask.GetMask("Ground"));

        Vector3 spawnPosition;

        if (hit.collider != null)
        {
            spawnPosition = hit.point;
        }
        else
        {
            spawnPosition = transform.position + directionVector * tpOffset;
        }
        
        currentTPMark = Instantiate(TPMarkPrefabs, spawnPosition, Quaternion.identity); 
        isTPset = true;
    }

    public void TPAction()
    {
        transform.position = currentTPMark.transform.position;
        Destroy(currentTPMark);
        isTPset = false;
        rb.linearVelocity = Vector2.zero;
    }
}
