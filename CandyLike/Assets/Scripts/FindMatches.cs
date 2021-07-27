using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FindMatches : MonoBehaviour
{

    private Board board;
    public List<GameObject> currentMatches = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
    }

    public void FindAllMatches()
    {
        StartCoroutine(FindAllMatchesCo());
    }

    private IEnumerator FindAllMatchesCo()
    {
        yield return new WaitForSeconds(.2f);
        for(int i = 0; i < board.width; i++)
        {
            for(int j = 0; j <board.height; j++)
            {
                GameObject currentCandy = board.allCandies[i, j];
                if(currentCandy != null)
                {
                    if (i > 0 && i < board.width - 1)
                    {
                        GameObject leftCandy = board.allCandies[i - 1, j];
                        GameObject rightCandy = board.allCandies[i + 1, j];
                        if (leftCandy != null && rightCandy != null)
                        {
                            if (leftCandy.tag == currentCandy.tag && rightCandy.tag == currentCandy.tag)
                            {
                                if(currentCandy.GetComponent<Candy>().isRowBomb || leftCandy.GetComponent<Candy>().isRowBomb || rightCandy.GetComponent<Candy>().isRowBomb)
                                {
                                    currentMatches.Union(GetRowPieces(j));
                                }

                                if (!currentMatches.Contains(leftCandy))
                                {
                                    currentMatches.Add(leftCandy);
                                }
                                leftCandy.GetComponent<Candy>().isMatched = true;
                                if (!currentMatches.Contains(rightCandy))
                                {
                                    currentMatches.Add(rightCandy);
                                }
                                rightCandy.GetComponent<Candy>().isMatched = true;
                                if (!currentMatches.Contains(currentCandy))
                                {
                                    currentMatches.Add(currentCandy);
                                }
                                currentCandy.GetComponent<Candy>().isMatched = true;
                            }
                        }
                    }

                    if (j > 0 && j < board.height - 1)
                    {
                        GameObject upCandy = board.allCandies[i, j + 1];
                        GameObject downCandy = board.allCandies[i, j - 1];
                        if (upCandy != null && downCandy != null)
                        {
                            if (upCandy.tag == currentCandy.tag && downCandy.tag == currentCandy.tag)
                            {
                                if (!currentMatches.Contains(upCandy))
                                {
                                    currentMatches.Add(upCandy);
                                }
                                upCandy.GetComponent<Candy>().isMatched = true;
                                if (!currentMatches.Contains(downCandy))
                                {
                                    currentMatches.Add(downCandy);
                                }
                                downCandy.GetComponent<Candy>().isMatched = true;
                                if (!currentMatches.Contains(currentCandy))
                                {
                                    currentMatches.Add(currentCandy);
                                }
                                currentCandy.GetComponent<Candy>().isMatched = true;
                            }
                        }
                    }
                }
            }
        }
    }

    List<GameObject> GetColumnPieces(int column)
    {
        List<GameObject> candies = new List<GameObject>();
        for(int i = 0; i < board.height; i++)
        {
            if(board.allCandies[column, i] != null)
            {
                candies.Add(board.allCandies[column, i]);
                board.allCandies[column, i].GetComponent<Candy>().isMatched = true;
            }
        }


        return candies;
    }
    List<GameObject> GetRowPieces(int row)
    {
        List<GameObject> candies = new List<GameObject>();
        for (int i = 0; i < board.width; i++)
        {
            if (board.allCandies[i, row] != null)
            {
                candies.Add(board.allCandies[i, row]);
                board.allCandies[i, row].GetComponent<Candy>().isMatched = true;
            }
        }


        return candies;
    }
}
