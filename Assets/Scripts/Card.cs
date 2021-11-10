using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{

    public enum CardType : byte
    {
        Cohort1,
        Cohort2,
        Neutral,
        Lose
    }

    public enum TeamType : byte
    {
        Team1,
        Team2
    }

    [SerializeField] private CardType cardType;
    [SerializeField] private Color Cohort1Color;
    [SerializeField] private Color Cohort2Color;
    [SerializeField] private Color NeutralColor;
    [SerializeField] private Color LoseColor;
    

    public CardType GetCardType
    {
        get
        {
            return cardType;
        }

    }

    public Color GetCohort1Color
    {
        get
        {
            return Cohort1Color;
        }
    }

    public Color GetCohort2Color
    {
        get
        {
            return Cohort2Color;
        }
    }

    public Color GetNeutralColor
    {
        get
        {
            return NeutralColor;
        }
    }

    public Color GetLoseColor
    {
        get
        {
            return LoseColor;
        }
    }

    private bool isClicked = false;

    public bool IsClicked
    {
        get
        {
            return isClicked;
        }
        set
        {
            isClicked = value;
        }
    }


    private Image image;
    private TMP_Text Word;

    public Card(CardType _cardType, Color _Cohort1Color, Color _Cohort2Color, Color _NeutralColor, Color _LoseColor, bool _isClicked)
    {
        cardType = _cardType;
        Cohort1Color = _Cohort1Color;
        Cohort2Color = _Cohort2Color;
        NeutralColor = _NeutralColor;
        LoseColor = _LoseColor;
        isClicked = _isClicked;
    }
    

    private void Awake()
    {
        Word = GetComponentInChildren<TMP_Text>();
        image = GetComponent<Image>();
    }

    private void Start()
    {
        //SetCardType(CardType.Neutral);
    }

    public void SetCardType(CardType cardtype)
    {
        cardType = cardtype;
    }

    public void SetCardType(CardType cardtype, bool isTeacher)
    {
        cardType = cardtype;
        if (isTeacher)
        {
            SetColor();
        }
    }

    public void SetText(string text)
    {
        Word.text = text;
    }


    public void SetColor()
    {
        switch (cardType)
        {
            case CardType.Cohort1:
                image.color = Cohort1Color;
                break;
            case CardType.Cohort2:
                image.color = Cohort2Color;
                break;
            case CardType.Neutral:
                image.color = NeutralColor;
                break;
            case CardType.Lose:
                image.color = LoseColor;
                break;
        }
    }

    public void Clicked()
    {
        if(GameManager.instance.MyTeam != GameManager.instance.TeamTurn){
            return;
        }

        SetColor();

        if (isClicked)
        {
            return;
        }

        isClicked = true;

        if (cardType == CardType.Neutral)
        {
            EndTurn();
            GameManager.instance.CardsSend();
            return;
        }

        if (cardType == CardType.Lose)
        {
            GameManager.instance.CardsSend();
            GameManager.instance.GameOverSend();
            return;
        }

        //if team is team 1 and card type is cohort 1 (correct click)
        if (GameManager.instance.TeamTurn == TeamType.Team1 && cardType == CardType.Cohort1)
        {
            Debug.Log($"team 1 and card type is cohort 1");
            GetPoint(TeamType.Team1);
        } 
        //if team is 1 and card type is cohort 2 (wrong click)
        else if (GameManager.instance.TeamTurn == TeamType.Team1 && cardType == CardType.Cohort2)
        {
            Debug.Log($"team 1 and card type is cohort 2");
            GetPoint(TeamType.Team2);
            EndTurn();
        } 
        //if team is 2 and card type is cohort 1 (wrong click)
        else if (GameManager.instance.TeamTurn == TeamType.Team2 && cardType == CardType.Cohort1)
        {
            Debug.Log($"team 2 and card type is cohort 2");
            GetPoint(TeamType.Team1);
            EndTurn();
        } 
        //if team is 2 and card type is 2 (correct click)
        else if (GameManager.instance.TeamTurn == TeamType.Team2 && cardType == CardType.Cohort2)
        {
            Debug.Log($"team 2 and card type is cohort 2");
            GetPoint(TeamType.Team2);
        }


        GameManager.instance.CardsSend();

    }

    private void GetPoint(TeamType team)
    {
        if (team == TeamType.Team1)
        {
            GameManager.instance.Team1Points++;
        } else
        {
            GameManager.instance.Team2Points++;
        }
    }

    private void EndTurn()
    {
        if(GameManager.instance.TeamTurn == TeamType.Team1)
        {
            //Debug.Log($"Swapping from team 1 to team 2");
            GameManager.instance.TeamTurn = TeamType.Team2;
        } else if(GameManager.instance.TeamTurn == TeamType.Team2)
        {
            //Debug.Log($"Swapping from team 2 to team 1");
            GameManager.instance.TeamTurn = TeamType.Team1;
        }
    }

}
