using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameHelper : MonoBehaviour {

    private Vector3 throwSpeed = new Vector3(0, 0, 1150);// We can change this depends on our game requirement 
    public GameObject BallReference;  // Ball prefabe
    private Vector3 ballPos = new Vector3(0, 0.38f, -11.41f);
    public bool BallOnStage = false;// Helper value to detetct that the player can shoot again
    public int Score;//value to store the player score
    public Text ScoreText; //score text UI


    public Text CountDownText;// Countdown text start at 3
    public Image PauseBG;// Pause bg to use in first countdown

    public Text TimerText;// Timer start at 30
    private int timerValue;//Timer variable 

    public Transform CameraObj; // Hold camera obj to get distance 

    private GameObject currentBallObj;// Current Ball to get distance so the player can shoot again

    //Bar helper variables
    private float fillAmount;
    public float PowerBarSpeed = .5f;
    public Image PowerBar,PowerBarBg;
    enum BarAnimation
    {
        Idle, ToRight, ToLeft
    }
    private BarAnimation barAnimation;

    // Game Over variables
    public Text ResultGameOver;
    public GameObject GameOverBut;

    // Game status  
    public enum GameStatus
    {
        CountDown, Play, GameOver
    }
    public GameStatus gameStatus;
    void Awake()
    {
       
    }

    // Use this for initialization
    void Start () 
    {
        gameStatus = GameStatus.CountDown;
        PauseBG.enabled = false;

        ResultGameOver.text = "";
        GameOverBut.SetActive(false);

        HidePowerBar();
        timerValue = 30;
        TimerText.text = "Timer :" + timerValue;


        ClearScore();

        StartCoroutine(CountDown());

    }
	private void ClearScore()
    {
        Score = 0;
        ScoreText.text = "Score :" + Score;   
    }
    private IEnumerator TimerCount()
    {
        yield return new WaitForSecondsRealtime(1.0f);

        timerValue--;

        if (timerValue <= 0)
        {
            timerValue = 0;//Game Over
            GameOverGame();
        }
        else
            StartCoroutine(TimerCount());

        TimerText.text = "Timer :" + timerValue;

    }
    private IEnumerator CountDown()
    {
        CountDownText.enabled = true;
        PauseBG.enabled = true;
        CountDownText.text = "3";
        yield return new WaitForSecondsRealtime(1.0f);
        CountDownText.text = "2";
        yield return new WaitForSecondsRealtime(1.0f);
        CountDownText.text = "1";
        yield return new WaitForSecondsRealtime(1.0f);
        PauseBG.enabled = false;
        CountDownText.enabled = false;


        //Start the game
        gameStatus = GameStatus.Play;
        StartCoroutine(TimerCount());


    }
    public void IncreaseScore(int holeValue)
    {
        BallOnStage = false;
        Score += holeValue;
        ScoreText.text = "Score :" + Score;
    }

    private void GameOverGame()
    {
        PauseBG.enabled = true;
        gameStatus = GameStatus.GameOver;

        GameOverBut.SetActive(true);

        if (Leaderboard.HighestScore(Score))
        {
            Debug.Log("HISET");
            ResultGameOver.text = "Amazing! You got the highest score!";
            GameOverBut.transform.GetChild(0).GetComponent<Text>().text = "Go back";
            GameOverBut.GetComponent<Button>().onClick.AddListener(() => LoadMain());
        }
        else
            if (Leaderboard.LowestScore(Score))
        {
            ResultGameOver.text = "Your score is low. Try again";
            GameOverBut.transform.GetChild(0).GetComponent<Text>().text = "Try again";
            GameOverBut.GetComponent<Button>().onClick.AddListener(() => ResetGame());
        }
        else
        {
            ResultGameOver.text = "Try again.";
            GameOverBut.transform.GetChild(0).GetComponent<Text>().text = "Try again";
            GameOverBut.GetComponent<Button>().onClick.AddListener(() => ResetGame());
        }

        Leaderboard.Record(Score);

    }

    private void ResetGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }
    private void LoadMain()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
    }
    private void HidePowerBar()
    {
        PowerBarBg.enabled = false;
        PowerBar.enabled = false;
    }
    private void ShowPowerBar()
    {
        barAnimation = BarAnimation.ToRight;
        fillAmount = 0;
        PowerBar.GetComponent<Image>().fillAmount = fillAmount;
        PowerBarBg.enabled = true;
        PowerBar.enabled = true;
    }
    private void PrebareShoot(Vector2 pos)
    {
        if (!BallOnStage)
        {
            ShowPowerBar();
        }
    }
    private void Shoot(Vector2 pos)
    {
        if (!BallOnStage)
        {
            ballPos.x = Random.Range(-0.35f, 0.22f);
            Debug.Log(ballPos);

            Vector3 realWorldPos = Camera.main.ScreenToWorldPoint(pos);
            ballPos.x = realWorldPos.x;


            Debug.Log(ballPos);

            GameObject ball = Instantiate(BallReference, ballPos, transform.rotation) as GameObject;

            float force = (fillAmount * 50);
            throwSpeed.z += force;

            ball.GetComponent<Rigidbody>().AddForce(throwSpeed);

            currentBallObj = ball;
            BallOnStage = true;

            HidePowerBar();
        }
    }
    private void CheckBallDistance()
    {
        if (currentBallObj == null)//only to make sure that the ball obj is not null
            return;


        Vector3 heading = new Vector3(currentBallObj.transform.position.x - CameraObj.position.x,
                                      currentBallObj.transform.position.y - CameraObj.position.y,
                                      currentBallObj.transform.position.z - CameraObj.position.z);


        //this gives us the distance inward from the plane of the "screen", and it is also a bit more efficient in a frame update.
        float distance = Vector3.Dot(heading, CameraObj.forward);


        if (distance > 3.5f)  // We can change these value depends on what we need 
        {
            BallOnStage = false;
        }

    }
    void FixedUpdate()
    {
        if (gameStatus == GameStatus.Play)
        {
            if (BallOnStage)
                CheckBallDistance();
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (gameStatus == GameStatus.Play)
        {
            UpdateBar();

            if (Application.platform == RuntimePlatform.OSXEditor)//Test purpose 
            {
                if (Input.GetMouseButtonDown(0))
                {
                    PrebareShoot(Input.mousePosition);
                }
                if (Input.GetMouseButtonUp(0))
                {
                    Shoot(Input.mousePosition);
                }
            }
            else
            {
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);

                    if (touch.phase == TouchPhase.Began)
                    {
                        PrebareShoot(touch.position);
                    }

                    if (touch.phase == TouchPhase.Ended)
                    {
                        Shoot(touch.position);
                    }
                }
            }
        }
    }

    void UpdateBar()
    {
        if (!PowerBar.enabled)
            return;
        
        switch (barAnimation)
        {
            case BarAnimation.ToLeft:
                fillAmount -= (PowerBarSpeed * Time.fixedDeltaTime);
                break;

            case BarAnimation.ToRight:
                fillAmount += (PowerBarSpeed * Time.fixedDeltaTime) - (fillAmount * (PowerBarSpeed / 55));
                //.01
//                Debug.Log(fillAmount);
                break;
        }

        if (fillAmount >= .9f && barAnimation == BarAnimation.ToRight)
        {
            barAnimation = BarAnimation.ToLeft;
        }
        else
            if (fillAmount <= .1 && barAnimation == BarAnimation.ToLeft)
                barAnimation = BarAnimation.ToRight;


        PowerBar.GetComponent<Image>().fillAmount = fillAmount;
    }
}