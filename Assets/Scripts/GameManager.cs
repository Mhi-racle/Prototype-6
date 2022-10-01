using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager singleton;
    private GroundPiece[] allGroundPieces;
    private ParticleSystem explosionParicle;
    private BallController ballController;
    private AudioSource winAudio;
    void Start()
    {
        SetupNewLevel();
    }

    private void SetupNewLevel()
    {
        allGroundPieces = FindObjectsOfType<GroundPiece>();
        explosionParicle = FindObjectOfType<ParticleSystem>();
        ballController = GameObject.Find("Ball").GetComponent<BallController>();


        winAudio = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioSource>(); //get the audio game object for the win sound
        //sets the color of the particle system to that of the solve color in the ball controller script
        var main = explosionParicle.main;
        main.startColor = ballController.solveColor;
    }

    private void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
        }
        else if (singleton != this)
        {
            Destroy(gameObject);
            DontDestroyOnLoad(gameObject);
        }
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }
    private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        SetupNewLevel();
    }

    public void CheckComplete()
    {
        bool isFinished = true;

        for (int i = 0; i < allGroundPieces.Length; i++)
        {
            if (allGroundPieces[i].isColored == false)
            {
                isFinished = false;
                break;
            }
        }

        if (isFinished)
        {
            //Next Level
            StartCoroutine(NextLevel());
        }
    }

    IEnumerator NextLevel()
    {

        explosionParicle.Play();
        winAudio.Play();
        //load the new level after 1 second, i.e after the explosion particle system has played
        yield return new WaitForSeconds(2);

        //since there are 7 levels, the game would load the 1st level once the player is done with the 7th
        if (SceneManager.GetActiveScene().buildIndex == 6)
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

    }
}
