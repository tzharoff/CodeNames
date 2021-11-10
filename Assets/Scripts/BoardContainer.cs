using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardContainer : MonoBehaviour
{
    [SerializeField] private List<Card> cards = new List<Card>();

    public List<Card> Cards
    {
        get
        {
            return cards;
        }
    }

    private void Start()
    {
        GameManager.instance.Cards = cards;
        GameManager.instance.Setup();
    }

}
