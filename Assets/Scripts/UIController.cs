using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviourPunCallbacks
{
    #region singleton
    public static UIController instance;

    private void Awake()
    {
        instance = this;
    }
    #endregion

    [Header("Turn Panels")]
    [SerializeField] private GameObject Team1TurnPanel;
    [SerializeField] private GameObject Team2TurnPanel;
    [Header("Win Panels")]
    [SerializeField] private GameObject Team1WinPanel;
    [SerializeField] private GameObject Team2WinPanel;

    private Coroutine turnPanels;
    private Coroutine winPanels;

    [Header("Panel Turn Off Time")]
    [SerializeField] private float TurnTime;
    [SerializeField] private float WinTime;

    public void Team1Win()
    {
        Team1WinPanel.SetActive(true);
        Team2WinPanel.SetActive(false);
        StartCoroutine(WinsOff());
    }

    public void Team2Win()
    {
        Team1WinPanel.SetActive(false);
        Team2WinPanel.SetActive(true);
        StartCoroutine(WinsOff());
    }

    public void Team1Turn()
    {
        Team1TurnPanel.SetActive(true);
        Team2TurnPanel.SetActive(false);
        //turnPanels = StartCoroutine(TurnsOff());
    }

    public void Team2Turn()
    {
        Team1TurnPanel.SetActive(false);
        Team2TurnPanel.SetActive(true);
        //turnPanels = StartCoroutine(TurnsOff());
    }

    IEnumerator TurnsOff()
    {
        yield return new WaitForSeconds(TurnTime);
        Team1TurnPanel.SetActive(false);
        Team2TurnPanel.SetActive(false);
    }

    IEnumerator WinsOff()
    {
        //StopCoroutine(turnPanels);
        Team1TurnPanel.SetActive(false);
        Team2TurnPanel.SetActive(false);
        //return to main menu

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.DestroyAll();
        }

        yield return new WaitForSeconds(WinTime);

        PhotonNetwork.AutomaticallySyncScene = false;
        PhotonNetwork.LeaveRoom();

    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }
}
