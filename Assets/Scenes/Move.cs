using UnityEngine;

public class Move : MonoBehaviour
{
    public AudioClip audioJump;
    public AudioClip audioAttack;
    public AudioClip audioDamaged;
    public AudioClip audioItem;
    public AudioClip audioDie;
    public AudioClip audioFinish;
    AudioSource audioSource;

    public GameManager gameManager;
    public float maxSpeed;
    SpriteRenderer spriteRenderer;
    Rigidbody2D rigid;
    Animator anim;
    public float jumpPower;
    BoxCollider2D boxcollider;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        boxcollider = GetComponent<BoxCollider2D>();
        audioSource = GetComponent<AudioSource>();
    }

    void PlaySound(string action)
    {
        switch (action)
        {
            case "JUMP":
                audioSource.clip = audioJump;
                audioSource.Play();
                break;

            case "ATTACK":
                audioSource.clip = audioAttack;
                audioSource.Play();
                break;

            case "DAMAGED":
                audioSource.clip = audioDamaged;
                audioSource.Play();
                break;

            case "ITME":
                audioSource.clip = audioItem;
                audioSource.Play();
                break;

            case "DIE":
                audioSource.clip = audioDie;
                audioSource.Play();
                break;

            case "FINISH":
                audioSource.clip = audioFinish;
                audioSource.Play();
                break;

        }
    }

    private void Update()
    {
        //Jump
        if (Input.GetButtonDown("Jump") && !anim.GetBool("isJumping"))
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            anim.SetBool("isJumping", true);
            PlaySound("JUMP");
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
        //Health Down
        gameManager.HealthDown();

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

        PlaySound("DAMAGED");
    }

    void offDamaged()
    {
        gameObject.layer = 10;
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }

    void OnAttack(Transform enemy)
    {
        //point
        gameManager.stagePoint += 100;

        //Reaction Force
        rigid.AddForce(Vector2.up *10, ForceMode2D.Impulse);

        //Enemy Die
        EnemyMove enemyMove = enemy.GetComponent<EnemyMove>();
        enemyMove.EnemyOnDamaged();

        PlaySound("ATTACK");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Item")
        {

            //point
            bool isBronze = collision.gameObject.name.Contains("Bronze");
            bool isSilver = collision.gameObject.name.Contains("Silver");
            bool isGold = collision.gameObject.name.Contains("Gold");

            if(isBronze)
                gameManager.stagePoint += 50;

            if(isSilver)
                gameManager.stagePoint += 100;

            if(isGold)
                gameManager.stagePoint += 300;

            //Deactive Item
            collision.gameObject.SetActive(false);

            PlaySound("ITEM");
        }
        else if(collision.gameObject.tag == "Finish")
        {
            //Next Stage
            gameManager.NextStage();
            PlaySound("FINISH");
        }
    }

    public void OnDie()
    {
        //Sprite Alpha
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        //Sprite Flip Y
        spriteRenderer.flipY = true;

        //Collider Disable
        boxcollider.enabled = false;

        //Die Effect Jump
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);

        PlaySound("DIE");
    }

    public void VelocityZero()
    {
        rigid.linearVelocity = Vector2.zero;
    }
}
