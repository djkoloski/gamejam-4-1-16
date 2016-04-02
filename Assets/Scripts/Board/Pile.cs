using UnityEngine;
using System.Collections;

public class Pile : MonoBehaviour {

    public int numRocks = 0;

    public string GameType;

    bool rockGrabbed = false;

	// Use this for initialization
	void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
        Vector3 mouse2world = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector2 mouseCheck = new Vector2(mouse2world.x, mouse2world.y);

        // On click, the player cursor must be over the Pile and there must be rocks here
        if (Input.GetMouseButtonDown(0) && this.GetComponent<BoxCollider2D>().OverlapPoint(mouseCheck) && numRocks > 0)
        {
            rockGrabbed = true;
        }

        // Check for leftMouse is released
        if (Input.GetMouseButtonUp(0) && rockGrabbed)
        {
            // If this was released over a player pile
            if (pileCheck(mouseCheck))
            {
                this.removeRocks();
                this.GetComponentInParent<Board>().PlayerPile.GetComponentInParent<PlayerPile>().addRocks();
                this.GetComponentInParent<Board>().setPileLock(this.gameObject);
            }

            rockGrabbed = false;
        }


    }

    bool pileCheck(Vector2 mc) 
    {
        return this.GetComponentInParent<Board>().pileCheck(mc, this.gameObject);
    }

    public void addRocks()
    {
        numRocks += 1;
    }

    public bool removeRocks()
    {

        if (numRocks > 0)
        {
            numRocks += -1;
            Debug.Log("Rock removed, rocks remaining: " + numRocks.ToString());
            return true;
        }
        else
        {
            return false;
        }
    }
}