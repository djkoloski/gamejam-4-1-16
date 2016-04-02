using UnityEngine;
using System.Collections;

public class PlayerPile : MonoBehaviour {

    int numrocks = 0;

	// Use this for initialization
	void Start () {

    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void addRocks()
    {
        numrocks += 1;
        this.GetComponentInParent<Board>().rocksTakenThisTurn += 1;
        Debug.Log("Rock Added to pile, current rocks in pile: " + numrocks.ToString());
    }
}
