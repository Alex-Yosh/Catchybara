using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGenerator : MonoBehaviour
{
    enum FallingObjects
    {
        Tangerine,
        GoldenTangerine,
        Stick,
        RottenTagnerine,
        LifeFlower,
        LargerStick
    }

    const int StickProbability = 55;
    const int RottenProbability = 10;

    private int smallStickProbability = 100;

    //need to add to 35
    private int TangerineProbability = 30;
    private int GoldenProbability = 5;
    private int FlowerProbability = 0;

    private int stickCount = 0;
    const float followingStickSpeed = 3.5f;

    int totalToSpawn;


    [SerializeField]
    int maxToSpawn = 10;

    [SerializeField]
    GameObject[] droppedObjects;

    [SerializeField]
    Player currplayer;


    [Header("Bounderies")]
    [SerializeField] float maxLeftSpawn = -3f;
    [SerializeField] float maxRightSpawn = 3f;
    [SerializeField] float maxBelowSpawn = 3f;


    const float slowestSpawnTime = 3f;
    const float timeToGetToFastestSpawnTime = 60f;//in Seconds
    const float fastestSpawnTime = 0.3f;
    private float maxSpawnTime = 3f;


    private float minFallSpeed = 3; 
    private float maxFallSpeed = 5; 

    private float time;
    private float startTime;
    private bool firstSpawn;
    private int playerlifes; 


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.playGame == false)
        {
            //when game is not playing
            firstSpawn = true;
        }

        if (GameManager.instance.playGame == true && firstSpawn)
        {
            //when game starts
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
            TangerineProbability = 30;
            GoldenProbability = 5;
            FlowerProbability = 0;
            Invoke("SpawnObject", 0f);
            startTime = Time.timeSinceLevelLoad;
            firstSpawn = false;
        }

        if (!firstSpawn)
        {
            Invoke("ManageDifficulty", 0f);
            Invoke("ManageSpawnRatios", 0f);
        }
    }

    void SpawnObject()
    {
        if (firstSpawn)
            return;

        if (transform.childCount < totalToSpawn)
        {
            //spawn dropping objects
            float x = Random.Range(maxLeftSpawn, maxRightSpawn);
            float adjustment_y = Random.Range(maxBelowSpawn, 0);

            //random number from 1-100
            int randomNumber = Random.Range(0, 99);

            //speed of object
            float randomSpeed = Random.Range(minFallSpeed, maxFallSpeed);

            GameObject go = null;

            switch (randomNumber)
            {
                case int n when (n < GoldenProbability):
                    //golden
                    go = droppedObjects[((int)FallingObjects.GoldenTangerine)];
                    break;

                case int n when (n >= GoldenProbability
                && n < (GoldenProbability + StickProbability)):
                    //stick
                    stickCount += 1;

                    //one in every 4 sticks are overhead
                    if (stickCount%4 == 0)
                    {
                        x = currplayer.GetPosition();
                        randomSpeed = followingStickSpeed;
                    }


                    int randomNumberStick = Random.Range(0, 99);
                    if (randomNumberStick >= (100- smallStickProbability))
                    {
                        //small
                        go = droppedObjects[((int)FallingObjects.Stick)];
                    }
                    else
                    {
                        //large
                        go = droppedObjects[((int)FallingObjects.LargerStick)];
                    }
                    break;

                case int n when ((n >= GoldenProbability + StickProbability)
                && n < GoldenProbability + StickProbability + TangerineProbability):
                    //tangerine
                    go = droppedObjects[((int)FallingObjects.Tangerine)];
                    break;


                case int n when ((n >= GoldenProbability + StickProbability + TangerineProbability)
                && n < GoldenProbability + StickProbability + TangerineProbability+ RottenProbability):
                    //rotten
                    go = droppedObjects[(int)FallingObjects.RottenTagnerine];
                    break;

                case int n when (n >= 100-FlowerProbability):
                    //flower
                    go = droppedObjects[(int)FallingObjects.LifeFlower];
                    foreach (Transform child in transform)
                    {
                        //if any flowers on screen, make tangerine
                        if (child.tag == "LifeFlower")
                        {
                            go = droppedObjects[(int)FallingObjects.Stick];
                        }
                    }
                    break;
            }

            if (go != null)
            {
                go.GetComponent<FallingObject>().fallSpeed = randomSpeed;
                Instantiate(go, new Vector3(x, transform.position.y - adjustment_y), transform.rotation, transform);
            }

            float randomDelay = Random.Range(0f, maxSpawnTime);
            Invoke("SpawnObject", randomDelay);
        }
        else
        {
            Invoke("SpawnObject", 0f);
        }
    }

    void ManageDifficulty()
    {
        time = Time.timeSinceLevelLoad - startTime;

        //manage big/small sticks
        switch(time)
        {
            case float n when n >= 20 && n < 40:
                smallStickProbability = 85;
                break;

            case float n when n >= 40 && n < 60:
                smallStickProbability = 70;
                break;

            case float n when n >= 60:
                smallStickProbability = 65;
                break;
        }



        //one more item every 10 seconds
        int additionalItems = System.Convert.ToInt32(time/10f);
        if (totalToSpawn < maxToSpawn)
        {
            totalToSpawn = 5 + additionalItems;

        }

        //max spawn time decreases from 3 to 0.3
        if (timeToGetToFastestSpawnTime > time)
        {
            maxSpawnTime = slowestSpawnTime
                - ((slowestSpawnTime - fastestSpawnTime) * (time / timeToGetToFastestSpawnTime));

        }

        //fall speeds get higher as time goes on
        minFallSpeed = 3;
        maxFallSpeed = 5 + (time / 10);
    }


    void ManageSpawnRatios()
    {
        playerlifes = currplayer.GetLives();

        if (playerlifes == 3)
        {
            TangerineProbability = 30;
            GoldenProbability = 5;
            FlowerProbability = 0;
        }

        if (playerlifes == 2)
        {
            TangerineProbability = 29;
            GoldenProbability = 3;
            FlowerProbability = 3;
        }

        if (playerlifes == 1)
        {
            TangerineProbability = 26;
            GoldenProbability = 3;
            FlowerProbability = 6;
        }
    }

}
