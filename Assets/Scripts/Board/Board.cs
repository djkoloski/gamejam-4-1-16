using UnityEngine;
using System.Collections;

public class Board : MonoBehaviour {


    public GameObject PileObject;
    public GameObject[] piles;

    public GameObject PlayerPile;

    public int maxRocksPerTurn = 0;
    public int PileLock = -1;

    public string GameType = "none";
    public int moveNum = 0;

    public int rocksTakenThisTurn = 0;

    public int dist = 0;

    void Start()
    {
        startBasicNim();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) || rocksTakenThisTurn == maxRocksPerTurn || checkEmptyPiles())
        {
            endTurn();
        }
    }

    public void startBasicNim()
    {
        Debug.Log("GameType Started: Basic Nim");
        GameType = "basic";
        maxRocksPerTurn = 3;

        PileLock = -1;

        piles = new GameObject[1];
        GameObject p = Instantiate(PileObject);
        p.transform.parent = transform;

        p.GetComponent<Pile>().GameType = GameType;

        for (int i=0; i<11; i++)
        {
            p.GetComponent<Pile>().addRocks();
        }

        piles[0] = p;

    }

    public void startRowNim(int n)
    {
        maxRocksPerTurn = 1000000;
        piles = new GameObject[n];

        float rot = 60.0f * Mathf.Deg2Rad;

        for (int i=0; i< n; i++)
        {
            GameObject p = Instantiate(PileObject);
            p.transform.parent = transform;

            float newx = p.transform.position.x + (dist * Mathf.Sin(rot));
            float newy = p.transform.position.x + (dist * Mathf.Cos(rot));

            p.transform.position = new Vector3(newx, newy, p.transform.position.z);

            for (int j = 0; j < 5; j++)
            {
                p.GetComponent<Pile>().addRocks();
            }

            piles[i] = p;

            rot += (120 * Mathf.Deg2Rad);
        }
    }

    /* returns true if we can safely add to our pile*/
    public bool pileCheck(Vector2 mc, GameObject p)
    {
        // Check if the box is overlapping, then if the pile is locked, then if we can take rocks
        return PlayerPile.GetComponent<BoxCollider2D>().OverlapPoint(mc) && pileLockCheck(p) && maxRocksPerTurn > rocksTakenThisTurn;
    }

    /*a function that returns true if the we are allowed to take from pile p*/
    public bool pileLockCheck(GameObject p)
    {
        // Pilelock == -1, anything goes
        // Pilelock != -1 but the pile matches the locked pile, then we return true
        return (PileLock == -1 || piles[PileLock] == p);
    }

    /*checks if all piles are empty, and returns true if that is the case*/
    bool checkEmptyPiles()
    {
        for (int i=0; i<piles.Length; i++)
        {
            if (piles[i].GetComponent<Pile>().numRocks != 0)
            {
                return false;
            }
        }

        return true;
    }

    bool wincheck()
    {
        if (checkEmptyPiles() )
        {
            return true;
        }

        return false;
    }

    public void endTurn ()
    {
        if (wincheck())
        {
            Debug.Log("You win!");
            Destroy(this.gameObject);
        }
        else
        {
            Debug.Log("The game continues! Next move...");
            moveNum += 1;
            rocksTakenThisTurn = 0;
        }
    }

    // Set this pile to be the only pile one may take from for the rest of the turn
    public void setPileLock(GameObject p)
    {
        for (int i=0; i<piles.Length; i++)
        {
            if (piles[i] == p)
            {
                PileLock = i;
                return;
            }
        }
    }
}
