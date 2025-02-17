using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;


public class SingleTalkManager : MonoBehaviour
{
    public Text textName; // ĳ���� �̸�
    public Text textContent; // ĳ���� ���
    public Image faceLocation;
    public GameObject conversationPanel;
    public GameObject talkService;

    // Start is called before the first frame update 
    private Dictionary<string, ShopConversationData> shopDataDict = new Dictionary<string, ShopConversationData>();
    private int index = 0;
    private List<TalkData> shopList = new List<TalkData>();
    private bool isTyping = false;

    void Awake()
    {
        // Scene �г� Object
        conversationPanel = GameObject.Find("Battle");
        textName = GameObject.Find("Name").GetComponent<Text>();
        textContent = GameObject.Find("TalkText").GetComponent<Text>();
        // Conversation ��ü�� ����� �ִ� Object
        talkService = GameObject.Find("SingleService");

    }


    void Start()
    {
        string SceneName = SceneManager.GetActiveScene().name;
        TextAsset jsonFile = Resources.Load<TextAsset>(SceneName);
        if (jsonFile != null)
        {
            shopDataDict = JsonConvert.DeserializeObject<Dictionary<string, ShopConversationData>>(jsonFile.text);
            foreach (string key in shopDataDict.Keys)
            {
                // ������ �ó����� json�� �̿��� �ó����� ������ List �������� ��ȯ�ϴ� �ڵ�
                if (shopDataDict.ContainsKey(key))
                {
                    TalkData data = new TalkData();
                    string faceImage = shopDataDict[key].FaceImage;
                    string character = shopDataDict[key].Character;
                    string dialogue = shopDataDict[key].Dialogue;
                    data.setFaceImage(faceImage);
                    data.setCharacter(character);
                    data.setDialogue(dialogue);
                    //string event_trigger = shopDataDict[key].Event_Trigger;
                    int defaultScene = 0;
                    List<string> condition = new List<string>();
                    List<int> nextScene = new List<int>();
                    // S000 / S00 Ư�� ���� �����ϱ� ���� ���Խ�
                    string pattern = "S000|S00";
                    // choice �� choice next scene üũ�ϴ� ���ǹ���
                    if (shopDataDict[key].Choice_1 != null)
                    {
                        condition.Add(shopDataDict[key].Choice_1);
                        nextScene.Add(int.Parse(Regex.Replace(shopDataDict[key].Choice_1_Next_Scene, pattern, "", RegexOptions.IgnoreCase).Trim()));
                    }
                    if (shopDataDict[key].Choice_2 != null)
                    {
                        condition.Add(shopDataDict[key].Choice_2);
                        nextScene.Add(int.Parse(Regex.Replace(shopDataDict[key].Choice_2_Next_Scene, pattern, "", RegexOptions.IgnoreCase).Trim()));
                    }
                    if (shopDataDict[key].Choice_3 != null)
                    {
                        condition.Add(shopDataDict[key].Choice_3);
                        nextScene.Add(int.Parse(Regex.Replace(shopDataDict[key].Choice_3_Next_Scene, pattern, "", RegexOptions.IgnoreCase).Trim()));
                    }

                    // default next scene üũ�ϴ� ���ǹ�
                    if (shopDataDict[key].Default_Next_Scene != null)
                    {
                        defaultScene = int.Parse(Regex.Replace(shopDataDict[key].Default_Next_Scene, pattern, "", RegexOptions.IgnoreCase).Trim());
                        data.setDefaultScene(defaultScene);
                    }

                    if (condition.Count != 0)
                    {
                        data.setCondition(condition);
                        data.setNextScene(nextScene);
                    }

                    // event_trigger üũ�ϴ� ���ǹ�
                    //if (event_trigger != null)
                    //{
                    //    string[] trigger = event_trigger.Split('/');
                    //    foreach(var item in trigger)
                    //    {


                    //    }
                    //}

                    shopList.Add(data);
                }
            }
        }
        // ù�� ° �ó����� ����.
        StartCoroutine(CallSpriteImage(shopList[index].getFaceImage()));
        StartCoroutine(TypeName(shopList[index].getCharacter()));
        StartCoroutine(TypingSentence(shopList[index].getDialogue()));
        index++;
    }

    // Update is called once per frame
    void Update()
    {

    }

    #region ��ȭ �ѱ��
    public void Next()
    {
        if (isTyping)
        {
            isTyping = false;
            StopAllCoroutines();
            TypingCompleteSentence(shopList[index - 1].getDialogue());
            return;
        }
        if (index <= shopList.Count - 1)
        {
            StartCoroutine(CallSpriteImage(shopList[index].getFaceImage()));
            StartCoroutine(TypeName(shopList[index].getCharacter()));
            StartCoroutine(TypingSentence(shopList[index].getDialogue()));
        }
        if (shopList[index].getDialogue() == textContent.text)
        {
            if (shopList[index].getDefaultScene() != 0)
            {
                index = shopList[index].getDefaultScene() - 1;
                talkService.SetActive(false);
                return;
            }
        }
        index++;
    }
    #endregion




    IEnumerator CallSpriteImage(string imageName)
    {

        Sprite sprite = Resources.Load<Sprite>($"Sprites/TalkFace/{imageName}.png");


        if (sprite != null)
        {
            faceLocation.sprite = sprite;
        }

        yield return null;
    }

    IEnumerator TypeName(string name)
    {
        textName.text = name;
        yield return null;
    }

    IEnumerator TypingSentence(string sentence)
    {
        // Ÿ���� ������ üũ�ϴ� �÷��� ����
        isTyping = true;
        textContent.text = string.Empty;
        foreach (var letter in sentence)
        {
            textContent.text += letter;
            yield return new WaitForSeconds(0.03f);
        }
        // Ÿ���� �Ϸ����� �� Ÿ���� ȿ�� �����ٴ� ���� �˸�.
        if (textContent.text.Length == sentence.Length)
        {
            isTyping= false;
        }
    }

    public void TypingCompleteSentence(string sentence)
    {
        textContent.text = sentence;
    }
}
