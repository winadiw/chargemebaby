using UnityEngine;
using System.Collections;

public class StartGame : MonoBehaviour
{
    public AudioClip buttonClick;
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey(KeyCode.Space))
		{
            SoundManager.instance.PlaySingle(buttonClick);
            Application.LoadLevel(1);
		}
	}
}
