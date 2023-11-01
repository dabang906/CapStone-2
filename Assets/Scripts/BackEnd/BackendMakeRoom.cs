using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BackEnd;

public class BackendMakeRoom : MonoBehaviour
{
    public InputField RoomTitle;
    public Button Confirm, Back;
    public GameObject WaitingRoom;

    private static BackendMakeRoom _instance = null;

    public static BackendMakeRoom Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new BackendMakeRoom();
            }
            return _instance;
        }
    }

    private void Start()
    {
        Confirm.onClick.AddListener(GetValue);
        Back.onClick.AddListener(GetBack);
    }

    private void GetValue()
    {
        WaitingRoom.gameObject.SetActive(true);
    }

    private void GetBack()
    {
        Backend.Match.LeaveMatchMakingServer();
        this.gameObject.SetActive(false);
    }
}
