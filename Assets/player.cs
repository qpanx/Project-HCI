using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class player : MonoBehaviour
{
    public Text coinText;
    public int currentCoin = 0;
    public int maxHealth = 3;

    public Text health;
    public Animator animator;
    private Rigidbody2D rb;
    public float jumpHeight = 15f;
    public bool isGround = true;

    private float movement;
    public float moveSpeed = 5f;
    private bool facingRight = true;


    public Transform attackPoint;
    public float attackRadius =1f;

    public LayerMask attackLayer;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

        health.text = maxHealth.ToString();
        coinText.text = currentCoin.ToString();
        if (maxHealth <= 0)
        {
            Die();
        }

        
        movement = Input.GetAxis("Horizontal");

        //شكل الشخصية يتغير حسب الاتجاه
        if (movement < 0f && facingRight)
        {
            transform.eulerAngles = new Vector3(0f, -180f, 0f);
            facingRight = false;
        }
        else if (movement > 0f && facingRight == false)
        {
            transform.eulerAngles = new Vector3(0f, 0f, 0f);
            facingRight = true;
        }


        if (Input.GetKey(KeyCode.Space) && isGround)
        {
            Jump();
            isGround = false;
            animator.SetBool("Jump", true);
        }



        if (Mathf.Abs(movement) > .1f)
        { 
            animator.SetFloat("Run", 1f);
        }
        else if (movement < .1f)
        {
            animator.SetFloat("Run", 0f);
        }


        if (Input.GetMouseButtonDown(0)) {
            animator.SetTrigger("Attack");
        }
    }


    private void FixedUpdate()
    {
        transform.position += new Vector3(movement, 0f, 0f) * Time.fixedDeltaTime * moveSpeed;
    }

    void Jump()
    {
        rb.AddForce(new Vector2(0f, jumpHeight), ForceMode2D.Impulse);
    }

   

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGround = true;
            animator.SetBool("Jump",false);
        }
    }

    public void Attack()
    {
        Collider2D collInfo = Physics2D.OverlapCircle(attackPoint.position, attackRadius, attackLayer);

        if (collInfo != null)
        {
            if (collInfo.gameObject.GetComponent<Box>() != null)
            {
                collInfo.gameObject.GetComponent<Box>().TakeDamage(1);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
        {
            return;
        }
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }

    public void TakeDamage(int damage)
    {
        maxHealth -= damage;
        animator.SetTrigger("Hurt");

        if (maxHealth < 0)
        {
            Die();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Coin")
        {
            currentCoin ++;
            other.gameObject.transform.GetChild(0).GetComponent<Animator>().SetTrigger("Collected");
            Destroy(other.gameObject, 1f);
        }

        if (other.gameObject.tag == "VictoryPoint")
        {
            FindObjectOfType<CA>().LoadLevel();
        }
    }


    void Die()
    {
       FindObjectOfType<GameManager>().isGameActive = false;
       Destroy(this.gameObject, 0.5f);
    }


}
