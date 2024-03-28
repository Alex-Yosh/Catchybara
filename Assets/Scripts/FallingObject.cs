using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingObject : MonoBehaviour
{
    public int minDelay;
    public int maxDelay;
    public float despawnDelay = 2f;

    public float fallSpeed;

    //growing for
    private float growingFor = 0.7f; //as a percentage

    [SerializeField] float Ybounds = -3f;

    private bool isMoving = false;
    private bool isLanded = false;
    private bool isFading = false;
    private float delay = 0f;
    private float initTime;

    private float landedTime;

    public Sprite landed;
    public SpriteRenderer spriteRenderer;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip landing;


    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        transform.localScale = new Vector3(0, 0);

        var rand = new System.Random();
        delay = (float)(rand.Next(minDelay, maxDelay) + rand.NextDouble());
        initTime = Time.timeSinceLevelLoad;

        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y <= Ybounds)
        {
            Splat();
        }

        if (isMoving == false)
        {
            
            float multiplierScale = ((Time.timeSinceLevelLoad - initTime) / delay) + (1 - growingFor);
            if (multiplierScale <= 1)
                transform.localScale = new Vector3(multiplierScale, multiplierScale);
            else
            {
                transform.localScale = new Vector3(1, 1);
            }
        }


        isMoving = Time.timeSinceLevelLoad - initTime > delay;
        if (isMoving && !isLanded)
        {
            transform.Translate(Vector3.down * fallSpeed * Time.deltaTime, Space.World);
        }

        if (isLanded)
        {
            this.gameObject.tag = "Untagged";
            if (isFading == false)
            {
                StartCoroutine("FadeOut");
                isFading = true;
                audioSource.PlayOneShot(landing);
            }
            Invoke("ObjectLanded", despawnDelay);
        }
    }
    
    public void Splat()
    {
        spriteRenderer.sprite = landed;
        isLanded = true;
        landedTime = Time.timeSinceLevelLoad;
    }

    IEnumerator FadeOut()
    {
        SpriteRenderer sr = this.GetComponent<SpriteRenderer>();
        Color objectColor = sr.material.color;

        for (float f = 1f; f>0f; f -= 0.05f)
        {
            objectColor.a = f;
            sr.material.color = objectColor;
            yield return new WaitForSeconds(despawnDelay / 20f);
        }
    }

    void ObjectLanded()
    { 
        Destroy(this.gameObject);
    }

}
