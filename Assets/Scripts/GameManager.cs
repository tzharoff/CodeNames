//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class GameManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    #region singleton
    public static GameManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else
        {
            if (instance != this)
            {
                Destroy(gameObject);
            }
        }
        DontDestroyOnLoad(gameObject);
    }
    #endregion


    public enum EventCodes : byte
    {
        GameList,
        Team1Point,
        Team2Point,
        TeamTurn,
        MyTeam,
        Cardlist,
        GameOverMessage
    }

    [Header("Cards")]
    [SerializeField] private List<Card> cards = new List<Card>();
    [SerializeField] private int Team1CardCount = 9;
    [SerializeField] private int Team2CardCount = 8;
    [SerializeField] private int neutralCards = 7;
    [SerializeField] private int LoseCardCount = 1;
    [SerializeField] private int TileCount = 25;

    [Header("Role")]
    [SerializeField] private bool isTeacher = false;
    [SerializeField] private Card.TeamType myTeam = Card.TeamType.Team1;
    [SerializeField] private Card.TeamType teamTurn = Card.TeamType.Team1;

    [Header("Points")]
    [SerializeField] private int team1Points;
    [SerializeField] private int team2Points;
    [SerializeField] private Card.TeamType winningTeam = Card.TeamType.Team1;


    private List<string> UnityWords = new List<string>();
    private List<string> GameList = new List<string>();
    public List<Card> Cards
    {
        get
        {
            return cards;
        }

        set
        {
            cards = value;
        }
    }

    private void OnEnable()
    {
        if (!PhotonNetwork.IsConnected)
        {
            SceneManager.LoadScene(0);
        }
        //Debug.Log("Testing Enable");
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        //Debug.Log("Testing Disable");
        PhotonNetwork.RemoveCallbackTarget(this);
    }


    public Card.TeamType TeamTurn
    {
        get { return teamTurn; }
        set
        {
            teamTurn = value;
            if (teamTurn == Card.TeamType.Team1)
            {
                UIController.instance.Team1Turn();
            }
            else
            {
                UIController.instance.Team2Turn();
            }
            TeamTurnSend();
        }
    }

    public int Team1Points
    {
        get { return team1Points; }
        set { team1Points = value;
            if (team1Points == Team1CardCount)
            {
                winningTeam = Card.TeamType.Team1;
                GameOver();
            }
        }
    }

    public int Team2Points
    {
        get { return team2Points; }
        set { team2Points = value;
            if (Team2Points == Team2CardCount)
            {
                winningTeam = Card.TeamType.Team2;
                GameOver();
            }
        }
    }


    public Card.TeamType MyTeam
    {
        get
        {
            return myTeam;
        }
        set
        {
            myTeam = value;
        }
    }


    public void Setup()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            SetCards();
            PopulateUnityList();
            PopulateGameList();
            ListSend();
        }
        UIController.instance.Team1Turn();
    }

    private void PopulateGameList()
    {
        int wordCount = TileCount;
        GameList.Clear();
        while (wordCount > 0)
        {
            int randomCard = Random.Range(0, UnityWords.Count);
            string unityWord = UnityWords[randomCard];
            bool inList = false;
            foreach (string str in GameList)
            {
                if (str == unityWord)
                {
                    inList = true;
                    break;
                }
            }
            if (!inList)
            {
                GameList.Add(unityWord);
                wordCount--;
            }
        }

        int counter = 0;
        foreach (string str in GameList)
        {
            cards[counter].gameObject.GetComponent<Card>().SetText(str);
            counter++;
        }
    }

    private void PopulateUnityList()
    {
        UnityWords.Add("GameObject");
        UnityWords.Add("Transform");
        UnityWords.Add("Vector3");
        UnityWords.Add("Inspector");
        UnityWords.Add("Hierarchy");
        UnityWords.Add("Scene");
        UnityWords.Add("Prefab");
        UnityWords.Add("Build");
        UnityWords.Add("iOS");
        UnityWords.Add("Android");
        UnityWords.Add("Playstation");
        UnityWords.Add("Xbox");
        UnityWords.Add("Switch");
        UnityWords.Add("WebGL");
        UnityWords.Add("bool");
        UnityWords.Add("int");
        UnityWords.Add("float");
        UnityWords.Add("NullReferenceException");
        UnityWords.Add("null");
        UnityWords.Add("string");
        UnityWords.Add("Jam");
        UnityWords.Add("Sprite");
        UnityWords.Add("rigidbody");
        UnityWords.Add("Collider");
        UnityWords.Add("Trigger");
        UnityWords.Add("ParticleSystme");
        UnityWords.Add("SpringJoint");
        UnityWords.Add("Animation");
        UnityWords.Add("Animator");
        UnityWords.Add("Game");
        UnityWords.Add("Effect");
        UnityWords.Add("Shader");
        UnityWords.Add("Assets");
        UnityWords.Add("Raycast");
        UnityWords.Add("Baked");
        UnityWords.Add("Illuminator");
        UnityWords.Add("NavMesh");
        UnityWords.Add("Bloom");
        UnityWords.Add("Vignette");
        UnityWords.Add("Color");
        UnityWords.Add("Material");
        UnityWords.Add("Texture");
        UnityWords.Add("Timeline");
        UnityWords.Add("Project");
        UnityWords.Add("DontDestroyOnLoad");
        UnityWords.Add("Destroy");
        UnityWords.Add("Instantiate");
        UnityWords.Add("Awake");
        UnityWords.Add("Update");
        UnityWords.Add("Start");
        UnityWords.Add("OnTriggerEnter");
        UnityWords.Add("OnColliderEnter");
        UnityWords.Add("FixedUpdate");
        UnityWords.Add("LateUpdate");
        UnityWords.Add("CineMachine");
        UnityWords.Add("Time");
        UnityWords.Add("Time.deltaTime");
        UnityWords.Add("Input");
        UnityWords.Add("AudioSource");
        UnityWords.Add("3D");
        UnityWords.Add("2D");
        UnityWords.Add("Coroutine");
        UnityWords.Add("While");
    }


    public void SetCards()
    {
        if (cardCountTotal != TileCount)
        {
            Debug.LogError($"TileCount != {cardCountTotal}");
        }

        FillTeam1Cards();
        FillTeam2Cards();
        FillLoseCards();
        CardsSend();
        //send this list over the network
    }

    private void FillTeam1Cards()
    {
        int cardCount = Team1CardCount;
        while (cardCount > 0) {
            int randomCard = Random.Range(0, cards.Count);
            if (cards[randomCard].GetCardType == Card.CardType.Neutral) {
                cards[randomCard].SetCardType(Card.CardType.Cohort1, isTeacher);
                cardCount--;
            }
        }
    }

    private void FillTeam2Cards()
    {
        int cardCount = Team2CardCount;
        while (cardCount > 0)
        {
            int randomCard = Random.Range(0, cards.Count);
            if (cards[randomCard].GetCardType == Card.CardType.Neutral)
            {
                cards[randomCard].SetCardType(Card.CardType.Cohort2, isTeacher);
                cardCount--;
            }
        }
    }

    private void FillLoseCards()
    {
        int cardCount = LoseCardCount;
        while (cardCount > 0)
        {
            int randomCard = Random.Range(0, cards.Count);
            if (cards[randomCard].GetCardType == Card.CardType.Neutral)
            {
                cards[randomCard].SetCardType(Card.CardType.Lose, isTeacher);
                cardCount--;
            }
        }
    }

    private int cardCountTotal
    {
        get
        {
            return Team1CardCount + Team2CardCount + neutralCards + LoseCardCount;
        }
    }


    public void GameOver()
    {
        if (team1Points == Team1CardCount)
        {
            UIController.instance.Team1Win();
        } else if (team2Points == Team1CardCount)
        {
            UIController.instance.Team2Win();
        }
        else
        {
            if (TeamTurn == Card.TeamType.Team1)
            {
                UIController.instance.Team2Win();
            }
            else
            {
                UIController.instance.Team1Win();
            }
        }
    }

    public void OnEvent(EventData photonEvent)
    {
        if(photonEvent.Code > 199)
        {
            return;
        }

        EventCodes incomingCode = (EventCodes)photonEvent.Code;
        Debug.Log($"Incoming Code {incomingCode}");

        switch (incomingCode)
        {
            case EventCodes.GameList:
                ListReceive((object[])photonEvent.CustomData);
                break;
            case EventCodes.Cardlist:
                CardsReceive((object[])photonEvent.CustomData);
                break;
            case EventCodes.TeamTurn:
                TeamTurnReceive((object[])photonEvent.CustomData);
                break;
            case EventCodes.GameOverMessage:
                GameOver();
                break;
        }

        int counter = 0;
        foreach (string str in GameList)
        {
            cards[counter].gameObject.GetComponent<Card>().SetText(str);
            counter++;
        }

    }

    public void ListSend(){
        object[] package = new object[GameList.Count];
        for(int i = 0; i < GameList.Count; i++)
        {
            package[i] = GameList[i];
        }

        PhotonNetwork.RaiseEvent(
            (byte)EventCodes.GameList,
            package,
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            new SendOptions { Reliability = true }
        );

    }

    public void ListReceive(object[] data)
    {
        GameList.Clear();
        for (int i = 0; i < data.Length; i++)
        {
            string listItem = (string)data[i];
            GameList.Add(listItem);
        }
    }

    public void CardsSend()
    {
        object[] package = new object[cards.Count];
        for (int i = 0; i < cards.Count; i++)
        {
            object[] piece = new object[2]
            {
                cards[i].GetCardType,
                cards[i].IsClicked
            };
            //Debug.Log($"Card {i} Type Sent: {cards[i].GetCardType}");
            package[i] = piece;
        }

        PhotonNetwork.RaiseEvent(
            (byte)EventCodes.Cardlist,
            package,
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            new SendOptions { Reliability = true }
        );
    }

    public void CardsReceive(object[] data)
    {
        for (int i = 0; i < data.Length; i++)
        {
            object[] listItem = (object[])data[i];
            //Debug.Log($"Card {i} type received {listItem[0]}");
            cards[i].SetCardType((Card.CardType)listItem[0], isTeacher);
            cards[i].IsClicked = (bool)listItem[1];
            if (cards[i].IsClicked)
            {
                cards[i].SetColor();
            }
        }
    }


    public void Team1PointSend()
    {

    }

    public void Team1PointReceive(object[] data)
    {

    }

    public void Team2PointSend()
    {

    }

    public void Team2PointReceive(object[] data)
    {

    }

    public void TeamTurnSend()
    {
        object[] package = new object[1] { teamTurn };
        PhotonNetwork.RaiseEvent(
            (byte)EventCodes.TeamTurn,
            package,
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            new SendOptions { Reliability = true }
);
    }

    public void TeamTurnReceive(object[] data)
    {
        Card.TeamType previousTeam = teamTurn;
        Debug.Log($"Team Data Type{data[0]}");
        teamTurn = (Card.TeamType)data[0];
        if(previousTeam != teamTurn)
        {
            if (teamTurn == Card.TeamType.Team1)
            {
                UIController.instance.Team1Turn();
            }
            else
            {
                UIController.instance.Team2Turn();
            }
        }
    }

    public void MyTeamSend()
    {

    }

    public void MyTeamReceive(object[] data)
    {

    }

    public void GameOverSend()
    {
        object[] package = new object[1] { "Game Over" };
        PhotonNetwork.RaiseEvent(
            (byte)EventCodes.GameOverMessage,
            package,
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            new SendOptions { Reliability = true });
    }


    public void SetTeam1()
    {

    }

    public void SetTeam2()
    {

    }

}
