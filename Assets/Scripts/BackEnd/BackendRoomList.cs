using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BackEnd;

public class BackendRoomList : MonoBehaviour
{
    public Button Confirm, Back;

    public GameObject MakeRoom, Title;

    private static BackendRoomList _instance = null;

    public static BackendRoomList Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new BackendRoomList();
            }
            return _instance;
        }
    }

    private void Start()
    {
        Backend.Match.Poll();
        Confirm.onClick.AddListener(GetValue);
        Back.onClick.AddListener(GetBack);
    }

    private void GetValue()
    {
        MakeRoom.gameObject.SetActive(true);
    }

    private void GetBack()
    {
        Backend.Match.LeaveMatchMakingServer();
        this.gameObject.SetActive(false);
        MakeRoom.gameObject.SetActive(false);
        Title.gameObject.SetActive(true);
    }
}
