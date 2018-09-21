using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Hole : MonoBehaviour {


    public int holeValue = 0;
    public GameHelper GameHelperObj;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    void OnTriggerEnter(Collider other)
    {
        if (GameHelperObj.gameStatus == GameHelper.GameStatus.Play)
        {
            Destroy(other.gameObject);
            GameHelperObj.IncreaseScore(holeValue);
        }
    }

    void Reload()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
