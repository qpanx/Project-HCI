
using UnityEngine;

public class Box : MonoBehaviour
{
    public int maxHealth = 5;
    public bool facingLeft = true;
    public float moveSpped = 2f;
    public Transform checkPoint;
    public float distace = 1f;
    public LayerMask layerMask;
    public bool inRange = false;
    public Transform player;
    public float attackRange = 10f;
    public float retrieveDistance = 2.5f;
    public float chaseSpeed = 4f;
    public Animator animator;

    public Transform attackPoint;
    public float attackRadius = 1f;

    public LayerMask attackLayer;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (FindAnyObjectByType<GameManager>().isGameActive == false)
        {
            return;
        }


        if (maxHealth <= 0)
        {
            Die();
        }

        if (Vector2.Distance(transform.position, player.position) <= attackRange)
        {
            inRange = true;
        }
        else
        {
            inRange = false;
        }
        
        if (inRange)
        {

            if(player.position.x > transform.position.x && facingLeft == true)
            {
                transform.eulerAngles = new Vector3(0, -180, 0);  //تغيرر شكل العدو الاتجاه اليمين
                facingLeft = false;
            }
            else if(player.position.x < transform.position.x && facingLeft == false)
            {
                transform.eulerAngles = new Vector3(0, 0, 0);  //تغيرر شكل العدو الاتجاه اليسار
                facingLeft = true;
            }

            if( Vector2.Distance(transform.position, player.position) > retrieveDistance)
            {
                animator.SetBool("Attack1", false);
                transform.position = Vector2.MoveTowards(transform.position, player.position, chaseSpeed * Time.deltaTime);
            }
            else
            {
                animator.SetBool("Attack1", true);
            }
        }
        else
        {
          transform.Translate(Vector2.left * Time.deltaTime * moveSpped);

        RaycastHit2D hit = Physics2D.Raycast(checkPoint.position, Vector2.down, distace, layerMask);

        if (hit == false && facingLeft)
        {
            transform.eulerAngles = new Vector3(0, -180, 0);
            facingLeft = false;
        }
        else if(hit == false && facingLeft == false) {
            transform.eulerAngles = new Vector3(0, 0, 0);
            facingLeft = true;
        }  
        }

        
    }


    public void Attack()
    {
       Collider2D collInfo = Physics2D.OverlapCircle(attackPoint.position, attackRadius, attackLayer);

         if (collInfo != null)
         {
            if(collInfo.gameObject.GetComponent<player>() != null)
            {
                collInfo.gameObject.GetComponent<player>().TakeDamage(1);
            }
         }

    }

    public void TakeDamage(int damage)
    {
        maxHealth -= damage;
        animator.SetTrigger("Hurt");

        if (maxHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(this.gameObject, 0.5f);
    }

    private void OnDrawGizmosSelected()
    {
        if (checkPoint == null)
        {
            return;
        }
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(checkPoint.position, Vector2.down * distace);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        if (attackPoint == null)
        {
            return;
        }


        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}
