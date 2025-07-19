using UnityEngine;

public class Move : MonoBehaviour
{
    public float maxSpeed;
    SpriteRenderer spriteRenderer;
    Rigidbody2D rigid;
    Animator anim;
    public float jumpPower;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        //Jump
        if (Input.GetButtonDown("Jump") && !anim.GetBool("isJumping"))
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            anim.SetBool("isJumping", true);
        }

        //Stop Speed
        if (Input.GetButtonUp("Horizontal"))
        {
            rigid.linearVelocity = new Vector2(rigid.linearVelocity.normalized.x * 0.5f,
                rigid.linearVelocity.y);
        }

        if (Input.GetButton("Horizontal"))
        {
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
        }

        //Animation
        if (Mathf.Abs(rigid.linearVelocity.x) < 0.3)
        {
            anim.SetBool("isWalking", false);
        }
        else
        {
            anim.SetBool("isWalking", true);
        }
    }

    void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");

        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        if (rigid.linearVelocity.x > maxSpeed) // Right Max Speed
        {
            rigid.linearVelocity = new Vector2(maxSpeed, rigid.linearVelocity.y);
        }
        else if (rigid.linearVelocity.x < maxSpeed * (-1)) // Right Max Speed
        {
            rigid.linearVelocity = new Vector2(maxSpeed * (-1), rigid.linearVelocity.y);
        }

        //Landing Platform

        /*Physics2D.Raycast(...)
            2D 물리 광선을 쏘는 함수야.
            보이지 않지만, 마치 레이저처럼 쏴서 무언가에 맞았는지 확인하는 기능.
            맞으면 RaycastHit2D라는 구조체를 리턴하고, 안 맞으면.collider가 null인 구조체를 리턴해.*/

        if (rigid.linearVelocity.y < 0)
        {
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));

            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));
            if (rayHit.collider != null)
            {
                if (rayHit.distance < 0.5f)
                {
                    Debug.Log(rayHit.collider.name);
                    anim.SetBool("isJumping", false);
                }

            }
        }

        /*Debug.DrawRay(rigid.position, Vector3.down * 0.5f, new Color(0, 1, 0));

        RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 0.5f, LayerMask.GetMask("Platform"));

        if (rayHit == true)
        {
            Debug.Log(rayHit.collider.name);
            anim.SetBool("isJumping", false);
        }
        else
        {
            anim.SetBool("isJumping", true);
        }*/


    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            //Attack
            if (rigid.linearVelocity.y < 0
                && transform.position.y > collision.transform.position.y)
            {
                OnAttack(collision.transform);
            }
            //Damaged
            else
                OnDamaged(collision.transform.position);
        }
        /*if(collision.gameObject.tag == "Enemy")
        {
            Debug.Log("플레이어가 맞았습니다!");
            OnDamaged(collision.transform.position); //적이 있는 위치 인자
        }*/
    }

    void OnDamaged(Vector2 targetPos)
    {
        //Change Layer (Immortal Active)
        gameObject.layer = 11;

        //View Alpha
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        //Reaction Force
        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(dirc,1) * 7,  ForceMode2D.Impulse);

        Invoke("offDamaged", 3);

        //Animation
        anim.SetTrigger("doDamaged");

        Invoke("offDamaged", 3);
    }

    void offDamaged()
    {
        gameObject.layer = 10;
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }

    void OnAttack(Transform enemy)
    {
        EnemyMove enemyMove = enemy.GetComponent<EnemyMove>();
        enemyMove.OnDamaged();
    }
}
