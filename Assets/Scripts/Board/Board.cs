using UnityEngine;
using System.Collections.Generic;

public class Board : MonoBehaviour
{
	// Constants
	private const int PILE_NONE_ = -1;

	// Public variables
	public GameObject pilePrefab;
	public GameObject rockPrefab;
	public Pile opponentPile;
	public Pile playerPile;

	public List<Pile> Piles
	{
		get { return piles_; }
	}
	public int StonesLeft
	{
		get
		{
			int total = 0;
			foreach (Pile pile in piles_)
				total += pile.NumRocks;
			return total;
		}
	}
	public bool AreAllPilesEmpty
	{
		get { return StonesLeft == 0; }
	}

	// Private variables
	private List<Pile> piles_;
	private List<Rock> rocks_;

	// Initialization
	public void Awake()
	{
		piles_ = new List<Pile>();
		rocks_ = new List<Rock>();
	}

	// Public interface
	public void Reset()
	{
		opponentPile.Clear();
		playerPile.Clear();

		foreach (Pile pile in piles_)
			Destroy(pile.gameObject);

		piles_ = new List<Pile>();

		foreach (Rock rock in rocks_)
			Destroy(rock.gameObject);

		rocks_ = new List<Rock>();
	}
	public Pile AddPile(Vector2 position)
	{
		GameObject pileGameObject = Instantiate(pilePrefab);
		Pile pile = pileGameObject.GetComponent<Pile>();

		pileGameObject.transform.SetParent(transform);
		pileGameObject.transform.position = position;
		piles_.Add(pile);

		return pile;
	}
	public Rock AddRock()
	{
		GameObject rockGameObject = Instantiate(rockPrefab);
		Rock rock = rockGameObject.GetComponent<Rock>();

		rockGameObject.transform.SetParent(transform);
		rocks_.Add(rock);

		return rock;
	}
	public Rock GetRockNearPoint(Vector2 point, float distance)
	{
		foreach (Rock rock in rocks_)
			if (rock.IsWithinDistanceOfPoint(point, distance))
				return rock;
		return null;
	}
	public Pile GetPileUnderPoint(Vector2 point)
	{
		Pile nearestPile = null;
		float nearestDistance = 0.0f;
		foreach (Pile pile in piles_)
		{
			float distance = GetPileDistance(point, pile);
			if (nearestPile == null || distance < nearestDistance)
			{
				nearestPile = pile;
				nearestDistance = distance;
			}
		}

		// Test player pile
		{
			float distance = GetPileDistance(point, playerPile);
			if (nearestPile == null || distance < nearestDistance)
			{
				nearestPile = playerPile;
				nearestDistance = distance;
			}
		}

		// Test opponent pile
		{
			float distance = GetPileDistance(point, opponentPile);
			if (nearestPile == null || distance < nearestDistance)
			{
				nearestPile = opponentPile;
				nearestDistance = distance;
			}
		}

		return nearestPile;
	}

	// Private interface
	private float GetPileDistance(Vector2 point, Pile pile)
	{
		return (new Vector2(pile.transform.position.x, pile.transform.position.y) - point).magnitude;
	}
}
