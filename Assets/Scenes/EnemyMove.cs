using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    Rigidbody2D rigid;
    public int nextMove;
    Animator anim;
    SpriteRenderer spriteRanderer;
    BoxCollider2D boxcollider;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRanderer = GetComponent<SpriteRenderer>();
        Think();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        //Move
        rigid.linearVelocity = new Vector2(nextMove, rigid.linearVelocity.y);

        //Platform Check
        Vector2 frontvec = new Vector2(rigid.position.x + nextMove * 0.2f,
            rigid.position.y);

        Debug.DrawRay(frontvec, Vector3.down, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(frontvec, Vector3.down,
            1, LayerMask.GetMask("Platform"));
        if (rayHit.collider == null)
        {
            Turn();
        }
    }

    void Think()
    {
        //Set Next Active
        nextMove = Random.Range(-1, 2);

        //Sprite Animation
        anim.SetInteger("WarkSpeed", nextMove);

        //Flip Sprite
        if (nextMove != 0)
            spriteRanderer.flipX = nextMove == 1;

        //Recursive
        float nextThinkTime = Random.Range(2f, 4f);
        Invoke("Think", nextThinkTime);
    }

    void Turn()
    {
        nextMove = nextMove * -1;
        spriteRanderer.flipX = nextMove == 1;

        CancelInvoke();
        Think();

    }

    public void OnDamaged()
    {
        //Sprite Alpha
        spriteRanderer.color = new Color(1, 1, 1, 0.4f);

        //Sprite Flip Y
        spriteRanderer.flipY = true;

        //Collider Disable
        boxcollider.enabled = true;

        //Die Effect Jump
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);

        //Destroy
        Invoke("DeActive", 5);

    }

    void DeActive()
    {
        gameObject.SetActive(false);
    }
}
