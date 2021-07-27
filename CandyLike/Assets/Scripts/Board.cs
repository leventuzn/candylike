using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    wait,
    move
}

public class Board : MonoBehaviour
{
    public GameState currentState = GameState.move;
    public int width;
    public int height;
    public int offSet;
    public GameObject tilePrefab;
    public GameObject[] candies;
    public GameObject destroyEffect;
    private BackgroundTile[,] allTiles;
    public GameObject[,] allCandies;
    private FindMatches findMatches;

    // Start is called before the first frame update
    void Start()
    {
        findMatches = FindObjectOfType<FindMatches>();
        //allTiles = new BackgroundTile[width, height];
        allCandies = new GameObject[width, height];
        Setup();
    }

    private void Setup()
    {
        for(int i = 0; i<width; i++)
        {
            for(int j = 0; j<height; j++)
            {
                Vector2 tempPosition = new Vector2(i, j + offSet);
                //GameObject backgroundTile = Instantiate(tilePrefab, tempPosition, Quaternion.identity) as GameObject;
                //backgroundTile.transform.parent = this.transform;
                //backgroundTile.name = "( " + i + ", " + j + " )";
                int candyToUse = Random.Range(0, candies.Length);

                int maxIterations = 0;
                while(MatchesAt(i, j, candies[candyToUse]) && maxIterations < 100)
                {
                    candyToUse = Random.Range(0, candies.Length);
                    maxIterations++;
                }
                maxIterations = 0;

                GameObject candy = Instantiate(candies[candyToUse], tempPosition, Quaternion.identity);
                candy.GetComponent<Candy>().row = j;
                candy.GetComponent<Candy>().column = i;
                candy.transform.parent = this.transform;
                candy.name = "( " + i + ", " + j + " )";
                allCandies[i, j] = candy;
            }
        }
    }

    private bool MatchesAt(int column, int row, GameObject piece)
    {
        if(column > 1 && row > 1)
        {
            if(allCandies[column - 1, row].tag == piece.tag && allCandies[column - 2, row].tag == piece.tag)
            {
                return true;
            }
            if (allCandies[column, row - 1].tag == piece.tag && allCandies[column, row - 2].tag == piece.tag)
            {
                return true;
            }
        }else if(column <= 1 || row <= 1)
        {
            if(row > 1)
            {
                if (allCandies[column, row - 1].tag == piece.tag && allCandies[column, row - 2].tag == piece.tag)
                {
                    return true;
                }
            }
            if (column > 1)
            {
                if (allCandies[column - 1, row].tag == piece.tag && allCandies[column - 2, row].tag == piece.tag)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void DestroyMatchesAt(int column, int row)
    {
        if (allCandies[column, row].GetComponent<Candy>().isMatched)
        {
            findMatches.currentMatches.Remove(allCandies[column, row]);
            GameObject particle = Instantiate(destroyEffect, allCandies[column, row].transform.position, Quaternion.identity);
            Destroy(particle, .5f);
            Destroy(allCandies[column, row]);
            allCandies[column, row] = null;
        }
    }

    public void DestroyMatches()
    {
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                if(allCandies[i,j] != null)
                {
                    DestroyMatchesAt(i, j);
                }
            }
        }
        StartCoroutine(DecreaseRowCo());
    }

    private IEnumerator DecreaseRowCo()
    {
        int nullCount = 0;
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                if(allCandies[i,j] == null)
                {
                    nullCount++;
                }else if(nullCount > 0)
                {
                    allCandies[i, j].GetComponent<Candy>().row -= nullCount;
                    allCandies[i, j] = null;
                }
            }
            nullCount = 0;
        }
        yield return new WaitForSeconds(.4f);
        StartCoroutine(FillBoardCo());
    }

    private void RefillBoard()
    {
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                if(allCandies[i,j] == null)
                {
                    Vector2 tempPosition = new Vector2(i, j + offSet);
                    int candyToUse = Random.Range(0, candies.Length);
                    GameObject piece = Instantiate(candies[candyToUse], tempPosition, Quaternion.identity);
                    allCandies[i, j] = piece;
                    piece.GetComponent<Candy>().row = j;
                    piece.GetComponent<Candy>().column = i;
                }
            }
        }
    }

    private bool MatchesOnBoard()
    {
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                if(allCandies[i,j] != null)
                {
                    if (allCandies[i, j].GetComponent<Candy>().isMatched)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private IEnumerator FillBoardCo()
    {
        RefillBoard();
        yield return new WaitForSeconds(.5f);

        while (MatchesOnBoard())
        {
            yield return new WaitForSeconds(.5f);
            DestroyMatches();
        }
        yield return new WaitForSeconds(.5f);
        currentState = GameState.move;
    }
       
}
