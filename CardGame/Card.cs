using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public int Value { get; private set; }
    public bool IsRevealed { get; private set; }
    public bool IsMatched { get; private set; }
    public Text valueText;
    private ConcentrationGame game;

    public void SetValue(int value)
    {
        Value = value;
        valueText.text = "?";
        IsRevealed = false;
        IsMatched = false;
    }

    public void SetGame(ConcentrationGame game)
    {
        this.game = game;
    }

    public void OnClick()
    {
        if (!IsMatched && !IsRevealed)
        {
            game.CardSelected(this);
        }
    }

    public void Reveal()
    {
        IsRevealed = true;
        valueText.text = Value.ToString();
    }

    public void Hide()
    {
        IsRevealed = false;
        valueText.text = "?";
    }

    public void SetMatched()
    {
        IsMatched = true;
        valueText.text = "✓";
    }
}
