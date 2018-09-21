using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour {

	// Use this for initialization
	void Start () 
    {
        StartCoroutine(DestroyMe());
	}

    IEnumerator DestroyMe()
    {
        yield return new WaitForSecondsRealtime(5.0f);

        Destroy(gameObject);
    }
	
}
