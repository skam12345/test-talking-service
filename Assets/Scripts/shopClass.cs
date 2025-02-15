using System.Collections.Generic;
using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

[System.Serializable]
public class ShopConversationData
{
    public string FaceImage;
    public string Character;
    public string Dialogue;
    public string Choice_1;
    public string Choice_1_Next_Scene;
    public string Choice_2;
    public string Choice_2_Next_Scene;
    public string Choice_3;
    public string Choice_3_Next_Scene;
    public string Default_Next_Scene;
    public string Event_Trigger;
    public string Sfx;
}

[System.Serializable]
public class TalkData
{
    private string faceImage;
    private string character;
    private string dialogue;
    private List<string> condition;
    private List<int> nextScene;
    private int defaultScene;

    public void setFaceImage(string faceImage) { this.faceImage = faceImage; }
    public void setCharacter(string character) { this.character = character; }
    public void setDialogue(string dialogue) {  this.dialogue = dialogue; }
    public void setCondition(List<string> condition) { this.condition = condition; }
    public void setNextScene(List<int> nextScene) { this.nextScene = nextScene; }
    public void setDefaultScene(int defaultScene) { this.defaultScene = defaultScene; }

    public string getFaceImage() { return  this.faceImage; }
    public string getCharacter() { return this.character; }
    public string getDialogue() { return this.dialogue; }
    public List<string> getCondition() { return this.condition; }
    public List<int> getNextScene() { return this.nextScene; }
    public int getDefaultScene() { return this.defaultScene; }
}
