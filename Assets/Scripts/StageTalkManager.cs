using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;



public class StageTalkManager : MonoBehaviour
{
    public Text textName; // 캐릭터 이름
    public Text textContent; // 캐릭터 대사
    public Image faceLocation;
    public GameObject conversationPanel;
    public GameObject singleService;

    // Start is called before the first frame update 
    private Dictionary<string, ShopConversationData> shopDataDict = new Dictionary<string, ShopConversationData>();
    private int index = 0;
    private List<TalkData> shopList = new List<TalkData>();
    private bool isTyping = false;
    private int defaultScene = 0;

    void Awake()
    {
        // Scene 패널 Object
        conversationPanel = GameObject.Find("Battle");
        textName = GameObject.Find("Name").GetComponent<Text>();
        textContent = GameObject.Find("TalkText").GetComponent<Text>();
        // Conversation 객체들 담겨져 있는 Object
        singleService = GameObject.Find("SingleService");
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
                // 가져온 시나리오 json을 이용해 시나리오 진행할 List 형식으로 변환하는 코드
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
                    // S000 / S00 특정 문자 제거하기 위한 정규식
                    string pattern = "S000|S00";
                    // choice 와 choice next scene 체크하는 조건문들
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

                    // default next scene 체크하는 조건문
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

                    // event_trigger 체크하는 조건문
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
        // 첫번 째 시나리오 실행.
        StartCoroutine(CallSpriteImage(shopList[index].getFaceImage()));
        StartCoroutine(TypeName(shopList[index].getCharacter()));
        StartCoroutine(TypingSentence(shopList[index].getDialogue()));
    }

    // Update is called once per frame
    void Update()
    {

    }

    #region 대화 넘기기
    public void Next()
    {
        foreach (TalkData data in shopList) 
        {
            if(data.getDefaultScene() != 0)
            {
                defaultScene = data.getDefaultScene();
                break;
            }
        }
        if (index <= defaultScene)
        {
            if (isTyping)
            {
                isTyping = false;
                StopAllCoroutines();
                TypingCompleteSentence(shopList[index].getDialogue());
                return;
            }
            if (index == 0)
            {
                index++;
            }
            endTalk();
            if (index <= shopList.Count - 1)
            {
                Debug.Log("???");
                StartCoroutine(CallSpriteImage(shopList[index].getFaceImage()));
                StartCoroutine(TypeName(shopList[index].getCharacter()));
                StartCoroutine(TypingSentence(shopList[index].getDialogue()));
            }
            if (shopList[index].getDefaultScene() == 0)
            {
                index++;
            }
        }
    }
    #endregion

    public void endTalk()
    {
        if (shopList[index].getDefaultScene() != 0)
        {
            if (textContent.text == shopList[index].getDialogue())
            {
                singleService.SetActive(false);
            }
        }
    }



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
        // 타이핑 중인지 체크하는 플래그 변수
        isTyping = true;
        textContent.text = string.Empty;
        foreach (var letter in sentence)
        {
            textContent.text += letter;
            yield return new WaitForSeconds(0.03f);
        }
        // 타이핑 완료했을 때 타이핑 효과 끝났다는 것을 알림.
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
