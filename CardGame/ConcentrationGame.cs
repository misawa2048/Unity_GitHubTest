using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConcentrationGame : MonoBehaviour
{
    public GameObject cardPrefab;
    public Transform cardParent;
    public int rows = 2;
    public int cols = 4;
    public float cardSpacing = 1.5f;

    private List<Card> cards = new List<Card>();
    private Card firstCard, secondCard;
    private bool canSelect = true;

    void Start()
    {
        List<int> cardValues = new List<int>();
        for (int i = 0; i < (rows * cols) / 2; i++)
        {
            cardValues.Add(i);
            cardValues.Add(i);
        }
        Shuffle(cardValues);

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                GameObject cardObj = Instantiate(cardPrefab, cardParent);
                cardObj.transform.localPosition = new Vector3(c * cardSpacing, -r * cardSpacing, 0);
                Card card = cardObj.GetComponent<Card>();
                card.SetValue(cardValues[r * cols + c]);
                card.SetGame(this);
                cards.Add(card);
            }
        }
    }

    public void CardSelected(Card card)
    {
        if (!canSelect || card.IsRevealed) return;
        if (firstCard == null)
        {
            firstCard = card;
            card.Reveal();
        }
        else if (secondCard == null && card != firstCard)
        {
            secondCard = card;
            card.Reveal();
            StartCoroutine(CheckMatch());
        }
    }

    IEnumerator CheckMatch()
    {
        canSelect = false;
        yield return new WaitForSeconds(1f);
        if (firstCard.Value == secondCard.Value)
        {
            firstCard.SetMatched();
            secondCard.SetMatched();
        }
        else
        {
            firstCard.Hide();
            secondCard.Hide();
        }
        firstCard = null;
        secondCard = null;
        canSelect = true;
    }

    void Shuffle(List<int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int j = Random.Range(i, list.Count);
            int temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }
}
