using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AddButton : MonoBehaviour
{
    public GameObject battle;
    public GameObject work;
    // Start is called before the first frame update

    void Awake()
    {
        battle = GameObject.Find("Battle");
        if (battle != null)
        {
            battle.GetComponentInChildren<Text>().text = "�����Ϸ� ����";
            battle.GetComponentInChildren<Text>().fontSize = 40;
            battle.GetComponentInChildren<Text>().color = Color.white;
        }

        Button battleButton = battle.GetComponent<Button>();
        battleButton.onClick.AddListener(() => GoBattle());

        work = GameObject.Find("Work");
        if (work != null)
        {
            work.GetComponentInChildren<Text>().text= "���Ϸ� ����";
            work.GetComponentInChildren<Text>().fontSize = 40;
            work.GetComponentInChildren<Text>().color = Color.white;
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GoBattle()
    {
        SceneManager.LoadScene("LobbyBattle");
    }

}
