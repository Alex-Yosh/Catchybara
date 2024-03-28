using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : MonoBehaviour
{
    public static Player currPlayer { get; private set; }

    public int lifes;
    private bool isImmune;
    private float immunityTime;
    private float immuneDuration = 1f;
    private float changeColourTime;


    public Animator animator;
    PolygonCollider2D b_Collider;
    private SpriteRenderer sRenderer;

    private const int RegularTangerineValue = 1;
    private const int GoldenTangerineValue = 5;
    private const int RottenTangerineValue = -1;

    [SerializeField] private float horizontalBoundaryLength = 7f;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private int score;
    [SerializeField] private float baseSpeed;
    [SerializeField] private float horizontalInput;
    [SerializeField] private bool isFacingLeft;


    [SerializeField] private bool isDead;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip playerHitSound, playerRegularTangerineCatch, playerGoldenTangerineCatch, playerRottenTangerineCatch, playerFlowerCatch;

    private float playerWidth;


    private void Awake()
    {
        currPlayer = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        currPlayer.rb = GetComponent<Rigidbody2D>();
        currPlayer.b_Collider = GetComponent<PolygonCollider2D>();
        currPlayer.sRenderer = GetComponent<SpriteRenderer>();

        currPlayer.lifes = 3;
        score = 0;
        baseSpeed = 8f;
        horizontalInput = 0f;

        isFacingLeft = true;
        currPlayer.isDead = false;

        playerWidth = transform.GetComponent<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    void Update()
    {
        if (currPlayer.isImmune)
        {
            currPlayer.immunityTime += Time.deltaTime;
            Color tmp = currPlayer.sRenderer.color;
            if (currPlayer.immunityTime >= immuneDuration)
            {
                currPlayer.isImmune = false;

                tmp.a = 1f;
                currPlayer.sRenderer.color = tmp;
            } 
            else if (Time.timeSinceLevelLoad - currPlayer.changeColourTime > 0.1f)
            {
                if (tmp.a == 0.8f)
                {
                    tmp.a = 1f;
                }
                else
                {
                    tmp.a = 0.8f;
                }
                currPlayer.sRenderer.color = tmp;
                currPlayer.changeColourTime = Time.timeSinceLevelLoad;
            }
        } 

        if (currPlayer.isDead)
        {
            rb.velocity = new Vector2(0, 0);
        }
        else if (GameManager.instance.playGame == true)
        {
            horizontalInput = Input.GetAxisRaw("Horizontal");

            rb.velocity = new Vector2(horizontalInput, rb.velocity.y) * baseSpeed;

            BoundMovement();
            FlipFacingDirection();
        }
        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));

    }

    public void Revive()
    {
        currPlayer.animator.SetBool("Dead", false);
        if (currPlayer == null) return;

        currPlayer.isDead = false;
        currPlayer.score = 0;
        currPlayer.lifes = 3;
        currPlayer.isImmune = false;
        currPlayer.immunityTime = 0;
    }

    private void FlipFacingDirection()
    {
        /// Flips the sprite horizontally based on movement direction

        if ((!isFacingLeft && (horizontalInput < 0f)) || (isFacingLeft && (horizontalInput > 0f)))
        {
            isFacingLeft = !isFacingLeft;

            Vector3 displayScale = transform.localScale;
            displayScale.x *= -1f;
            transform.localScale = displayScale;
        }
    }

    private void BoundMovement()
    {
        Vector3 viewPos = transform.position;

        viewPos.x = Mathf.Clamp(viewPos.x, -horizontalBoundaryLength + (playerWidth / 2), horizontalBoundaryLength - (playerWidth / 2));
        transform.position = viewPos;
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (currPlayer.isDead == false)
        {
            if (other.tag == "Tangerine")
            {
                currPlayer.score += RegularTangerineValue;
                audioSource.PlayOneShot(playerRegularTangerineCatch);
            }
            else if (other.tag == "GoldenTangerine")
            {
                currPlayer.score += GoldenTangerineValue;
                audioSource.PlayOneShot(playerGoldenTangerineCatch);
            }
            else if (other.tag == "Stick")
            {
                TryDie();
            }
            else if (other.tag == "RottenTangerine")
            {
                currPlayer.score += RottenTangerineValue;
                audioSource.PlayOneShot(playerRottenTangerineCatch);
            } 
            else if (other.tag == "LifeFlower")
            {
                currPlayer.lifes = Mathf.Min(currPlayer.lifes + 1, 3);
                audioSource.PlayOneShot(playerFlowerCatch);
                GameManager.instance.updateLifeDisplay(currPlayer.lifes);
            }
        }
        

        if (currPlayer.isDead && GameManager.instance.playGame == false)
        {
            other.gameObject.GetComponent<FallingObject>().Splat();
        } 
        else if (other.tag != "Untagged")
        {
            Destroy(other.gameObject);
        }

        if (currPlayer.isDead && GameManager.instance.playGame == true)
        {
            GameManager.instance.GameOver(currPlayer.score);
        }
        else
        {
            GameManager.instance.UpdateScore(currPlayer.score);
        }
    }

    private void TryDie()
    {
        if (!currPlayer.isImmune)
        {
            audioSource.PlayOneShot(playerHitSound);
            currPlayer.lifes -= 1;
            GameManager.instance.updateLifeDisplay(currPlayer.lifes);

            if (currPlayer.lifes == 0)
            {
                currPlayer.isDead = true;
                animator.SetBool("Dead", true);
            }
            else
            {
                currPlayer.isImmune = true;
                currPlayer.immunityTime = 0;

                Color tmp = currPlayer.sRenderer.color;
                tmp.a = 0.8f;
                currPlayer.sRenderer.color = tmp;
                currPlayer.changeColourTime = Time.timeSinceLevelLoad;
            }
        }
    }

    public int GetLives()
    {
        return currPlayer.lifes;
    }

    public float GetPosition()
    {
        return currPlayer.transform.position.x;
    }

}
