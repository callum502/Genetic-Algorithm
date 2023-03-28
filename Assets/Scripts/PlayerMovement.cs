using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer sprite;
    [SerializeField] private int speed = 5;
    private BoxCollider2D boxCollider;

    public DNA dna;
    bool hasBeenInited = false;
    public bool hasFinished = false;
    public bool reachedtarget = false;
    int geneIndex = 0;
    int nextMove;
    public Vector2 target;
    public int stepCount;
    private Vector2 spawnpoint;
    bool dead;

    private enum State {idle, running, jumping, falling}
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();

        //get end position from scene
        target = GameObject.Find("End").transform.position;
    }

    //init player with dna
    public void initPlayer(DNA new_dna, Vector2 _spawnpoint)
    {
        dna = new_dna;
        nextMove = 1;
        hasBeenInited = true;
        spawnpoint = _spawnpoint;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!hasFinished && !dead)
        {
            //if reached end of genes set finished
            if (geneIndex == dna.genes.Count)
            {
                hasFinished = true;
            }

            //case statment gene index
            switch (dna.genes[geneIndex])
            {
                case 0:
                    //dont move
                    break;
                case 1:
                    //left
                    rb.velocity = new Vector2(-speed, rb.velocity.y);
                    break;
                case 2:
                    //right
                    rb.velocity = new Vector2(speed, rb.velocity.y);
                    break;
                case 3:
                    //jump
                    if (isGrounded())
                    {
                        rb.velocity = new Vector3(0, 12, 0);
                    }
                    break;
            }
            updateState(dna.genes[geneIndex]);
            geneIndex++;
            stepCount++;

            //death check
            if (transform.position.y < -10)
            {
                hasFinished = true;
                //gameObject.SetActive(false);
                dead = true;
            }
        }
    }

    void updateState(float geneIndex)
    {
        State state = State.idle;
        if (geneIndex == 1)
        {
            sprite.flipX = true;
            state = State.running;
        }
        else if (geneIndex == 2)
        {
            sprite.flipX = false;
            state = State.running;
        }

        if (rb.velocity.y > 0.01f)
        {
            state = State.jumping;
        }
        else if (rb.velocity.y < -0.01f)
        {
            state = State.falling;
        }

        animator.SetInteger("state", (int)state);
    }

    bool isGrounded()
    {
        return Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.down, 0.1f, LayerMask.GetMask("Ground"));
    }

    public float fitness
    {
        get
        {
            //get distance between transform.position.x & target.x


            return 1 - Mathf.Abs(transform.position.x - target.x)/ Mathf.Abs(spawnpoint.x - target.x);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Trap")
        {
            //kill player
            hasFinished = true;
            dead = true;
            rb.velocity = Vector2.zero;

        }
        if (collision.gameObject.tag == "Finish")
        {
            //load level
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}