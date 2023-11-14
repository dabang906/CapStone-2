using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendTabUI : MonoBehaviour
{
    public int index = 0;
    private Text matchText;
    private Toggle tab;
    private Image image;
    static readonly Color offColor = new Color((float)207 / 255, (float)207 / 255, (float)207 / 255, 255 / 255);
    static readonly Color onColor = new Color((float)87 / 255, (float)87 / 255, (float)87 / 255, 255 / 255);


    void Awake()
    {
        tab = this.GetComponent<Toggle>();
        image = this.GetComponentInChildren<Image>();
        matchText = this.GetComponentInChildren<Text>();

        tab.onValueChanged.AddListener(isOn =>
        {
            if (isOn == true)
            {
                image.color = onColor;
                //LobbyUI.GetInstance().ChangeTab();
            }
            else
            {
                image.color = offColor;
            }
        });

        if (tab.isOn == true)
        {
            image.color = onColor;
        }
        else
        {
            image.color = offColor;
        }
    }

    public void SetTabText(string matchTitle)
    {
        matchText.text = matchTitle;
    }

    public bool IsOn()
    {
        return tab.isOn;
    }
}
