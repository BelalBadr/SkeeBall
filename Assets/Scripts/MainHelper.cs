using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainHelper : MonoBehaviour {

    public GameObject MainCanvas, LeaderboardCanvas;
    public Text[] ScoresText;
	// Use this for initialization
	void Start () 
    {
        ToMain();
	}


    private void Update()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if(LeaderboardCanvas.activeInHierarchy)
                {
                    ToMain();
                }
            }
        }
    }

    public void ToMain()
    {
        MainCanvas.SetActive(true);
        LeaderboardCanvas.SetActive(false);
    }
	public void ToGame()
    {
        MainCanvas.SetActive(false);
        LeaderboardCanvas.SetActive(false);

        SceneManager.LoadScene("Game");
    }
    public void ToLeaderboard()
    {
        LeaderboardCanvas.SetActive(true);
        MainCanvas.SetActive(false);

        // Display high scores!
        for (int i = 0; i < Leaderboard.EntryCount; ++i)
        {
            var entry = Leaderboard.GetEntry(i);
            ScoresText[i].text = "Score: " + entry.score;
        }
    }
}
