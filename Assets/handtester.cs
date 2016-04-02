using UnityEngine;
using System.Collections;

public class handtester : MonoBehaviour {

    public GameObject hand;

	// Use this for initialization
	void Start () {
	
	}

    public void finished()
    {
        Debug.Log("Hand action finished");
    }
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetKeyDown(KeyCode.A))
        {
            hand.GetComponent<PlayerHand>().Reach(finished);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            hand.GetComponent<PlayerHand>().Retract(finished);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            hand.GetComponent<PlayerHand>().Inactive();
        }


    }
}
