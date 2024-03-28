using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroVideo : MonoBehaviour
{
    private bool videoFinished;
    [SerializeField] private float timePassed;

    // Start is called before the first frame update
    void Start()
    {
        videoFinished = false;
        timePassed = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!videoFinished && timePassed > 14)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            videoFinished = true;
        }

        timePassed += Time.deltaTime;
    }
}
