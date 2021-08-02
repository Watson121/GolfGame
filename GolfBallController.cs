using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// The main Golf Ball Controller Class.
/// This Class controls what happens to the ball such as what direction it will go in.
/// It also controls the camera as well
/// </summary>
public class GolfBallController : MonoBehaviour
{

    #region VARIABLES

    /// <summary>
    /// Player Camera - Main Player camera that will rotate around the ball
    /// Ball - The Ball that will move around the level, and what the camera will follow
    /// Ball Rigidbody - This is needed for ball physics
    /// </summary>
    [Header("Player Camera & Golf Ball")]
    public Camera playerCamera;
    public Transform ball;
    private Rigidbody ballRigidbody;

    private Vector3 previousCamPosition;
    private float zoomLevel;

    /// <summary> USER INTERFACE - THIS SHOULD BE MOVED TO A SEPERATE SCRIPT
    /// Power Slider - The Amount of Power that the ball will be moved by.
    /// Win Text - Win Text that appears when the ball reaches the win area.
    /// Putt Text - Text that displays number of times that the ball has beem putted.
    /// </summary>
    [Header("User Interface")]
    public Slider powerSlider;
    public TextMeshProUGUI winText;
    public TextMeshProUGUI restartText;
    public TextMeshProUGUI puttText;
    public TextMeshProUGUI powerText;

    /// <summary>
    /// List of audio clips
    /// </summary>
    [Header("Audio Clips")]
    public AudioClip golfPutt;
    private AudioSource golfBallAudioSource;

    //Number of times the player has putted the ball
    private uint puttCounter;
    private bool playerHasWon;

    //Checkpoint
    private Transform checkpoint;


    float timer = 2.0f;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        SettingUpBall();
        SettingUpUserInterface();
    }

    /// <summary>
    /// Setting up the Golf Ball.
    /// </summary>
    private void SettingUpBall()
    {
        zoomLevel = -10;
        playerHasWon = false;
        ballRigidbody = ball.gameObject.GetComponent<Rigidbody>();
        golfBallAudioSource = ball.gameObject.GetComponent<AudioSource>();
        playerCamera = Camera.main;

        //Setting up intial checkpoint
        checkpoint = GameObject.FindGameObjectWithTag("StartingPoint").transform.GetChild(0);

    }

    /// <summary>
    /// Setting up User Interface for the Player
    /// </summary>
    private void SettingUpUserInterface()
    {
        powerSlider = GameObject.Find("PowerSlider").GetComponent<Slider>();
        winText = GameObject.Find("WinText").GetComponent<TextMeshProUGUI>();
        restartText = GameObject.Find("RestartText").GetComponent<TextMeshProUGUI>();
        puttText = GameObject.Find("PuttCounter").GetComponent<TextMeshProUGUI>();
        powerText = GameObject.Find("PowerCounter").GetComponent<TextMeshProUGUI>();

        winText.enabled = false;
        restartText.enabled = false;

        SettingPuttCounter();
        SettingPowerText();
    }


    // Update is called once per frame
    void Update()
    {

        //Controlling the Power Level
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            BallPowerLevel(10);
        }

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            BallPowerLevel(-10);
        }

        //Hitting the ball
        if (Input.GetKeyDown(KeyCode.Space))
        {
            HitBall();
        }

        CameraControl();

        //Restarting the game
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (playerHasWon) { SceneManager.LoadScene(0); }
            else if (!playerHasWon)
            {
                ResetingBallPosition();
            }
        }

        //Updating Camera Position so it follows the Ball
        if (playerHasWon == false)
        {
            playerCamera.transform.position = ball.position;
            playerCamera.transform.Translate(new Vector3(0, 0, zoomLevel));
        }
        else if (playerHasWon)
        {

            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                restartText.enabled = true;
            }
        }
    }


    //Function responsible for controlling rotation and zoom of Camera
    void CameraControl()
    {

        #region Camera Rotation

        if (Input.GetMouseButtonDown(0)) previousCamPosition = playerCamera.ScreenToViewportPoint(Input.mousePosition);

        if (Input.GetMouseButton(0))
        {
            Vector3 direction = previousCamPosition - playerCamera.ScreenToViewportPoint(Input.mousePosition);

            float yRot = direction.y * 180;

            playerCamera.transform.Rotate(new Vector3(1, 0, 0), direction.y * 180);
            playerCamera.transform.Rotate(new Vector3(0, 1, 0), direction.x * 180, Space.World);

            Vector3 currentRotation = playerCamera.transform.localRotation.eulerAngles;
            currentRotation.x = Mathf.Clamp(currentRotation.x, 10, 80);
            currentRotation.z = 0;
            playerCamera.transform.localRotation = Quaternion.Euler(currentRotation);

            previousCamPosition = playerCamera.ScreenToViewportPoint(Input.mousePosition);

        }

        #endregion

        #region Camera Zoom

        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            zoomLevel += 1.0f;
            playerCamera.transform.position = ball.position;

            zoomLevel = Mathf.Clamp(zoomLevel, -10, -1);          
        }

        if(Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            zoomLevel -= 1.0f;
            playerCamera.transform.position = ball.position;

            zoomLevel = Mathf.Clamp(zoomLevel, -10, -1);
        }

        #endregion

    }

    #region Controlling the Ball

    //Setting the Ball Power Level
    void BallPowerLevel(int powerLevel)
    {
        powerSlider.value += powerLevel * Time.deltaTime;
        SettingPowerText();
    }

    //Hitting the Ball
    void HitBall()
    {
        ballRigidbody.AddForce(playerCamera.transform.forward * powerSlider.value, ForceMode.Impulse);
        powerSlider.value = 0;
        puttCounter++;

        golfBallAudioSource.PlayOneShot(golfPutt);

        SettingPuttCounter();
    }

    //Reseting the ball position
    void ResetingBallPosition()
    {
        ball.position = checkpoint.position;
        ballRigidbody.velocity = Vector3.zero;
    }

    #endregion

    #region Triggers

    private void OnTriggerEnter(Collider other)
    {

        if(other.tag == "WinArea")              //Ball has entered the win area
        {
            Debug.Log("You have won!!!");
            winText.enabled = true;
            playerHasWon = true;
        }
  

    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "LevelBounds")
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else if (other.tag == "WaterPiece")      //Ball has fallen into water
        {
            ResetingBallPosition();
        }
    }


    #endregion

    #region Player's User Interface

    //Updating the Putt Counter
    void SettingPuttCounter()
    {
        puttText.text = "Putt Counter: " + puttCounter;
    }

    void SettingPowerText()
    {
        powerText.text = ((int)powerSlider.value).ToString();
    }

    #endregion

    #region SETTER FUNCTIONS

    //Setting up Checkpoint
    public void SetCheckPoint(Transform newCheckpoint)
    {
        checkpoint = newCheckpoint;
    }

    #endregion

    #region RETURN FUNCTIONS

    public uint GetPuttCount()
    {
        return puttCounter;
    }

    #endregion


}
