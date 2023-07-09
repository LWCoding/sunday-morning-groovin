using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CutsceneHandler : MonoBehaviour
{
    
    public float fadeDelay;
    public float textDelay;
    [HideInInspector] public bool dialogueComplete = false;
    private List<string> dialogueList = new List<string>();
    private string dialogueText = "";
    private string currentText = "";
    private bool isReady = false;
    private bool isEnemyTurn = true;
    private bool activeSkip = false;
    private AudioClip blip;
    // Opacity of character when they aren't talking
    private float opacityWhenDisabled = 0.5f;
    private GameObject textBox;
    private GameObject dialogueName;

    void Start()
    {
        blip = (AudioClip)Resources.Load("Audio/SFX/blip");
        PublicVariables p = GameObject.Find("PublicVariables").GetComponent<PublicVariables>();
        if (PlayerPrefs.GetInt("Cutscenes") == 1 || p.cameFromGallery || p.dialogue == null || p.dialogue.Split('|').Length == 0) {
            dialogueComplete = true;
            return;
        }
        foreach (string s in p.dialogue.Split('|')) {
            dialogueList.Add(s);
        }
        dialogueName = GameObject.Find("DName");
        dialogueName.GetComponent<Text>().text = "";
        SetSide(true, p.enemyName);
        SetSide(false, p.playerName);
        RenderNewDialogue(true);
        textBox = GameObject.Find("DText");
        textBox.GetComponent<Text>().text = "";
        StartCoroutine(DialogueAppear());
    }

    private void SetSide(bool isEnemy, string spriteName) {
        PublicVariables p = GameObject.Find("PublicVariables").GetComponent<PublicVariables>();
        string side = (isEnemy) ? "LeftSide" : "RightSide";
        GameObject.Find(side).GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Dialogue/" + spriteName);
        if (spriteName.Length > 7 && spriteName.Substring(0, 6) == "Adrian") {
            GameObject.Find(side).transform.localScale = new Vector3(((isEnemy) ? -1 : 1) * 1.1f, 1.1f, 1);
        } else {
            GameObject.Find(side).transform.localScale = new Vector3(((isEnemy) ? -1 : 1), 1, 1);
        }
        if (!Resources.Load<Sprite>("Images/Dialogue/" + spriteName)) {
            GameObject.Find(side).GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Dialogue/Oliver");
        }
    }

    IEnumerator DialogueAppear() {
        yield return new WaitForSeconds(0.6f);
        GameObject box = GameObject.Find("DBox");
        GameObject cover = GameObject.Find("DCover");
        for (int i = 0; i < 17; i++) {
            box.GetComponent<Image>().color += new Color32(0, 0, 0, 15);
            textBox.GetComponent<Text>().color += new Color32(0, 0, 0, 15);
            dialogueName.GetComponent<Text>().color += new Color32(0, 0, 0, 15);
            cover.GetComponent<Image>().color += new Color32(0, 0, 0, 80 / 17);
            yield return new WaitForSeconds(fadeDelay / 2);
        }
        GameObject left = GameObject.Find("LeftSide");
        GameObject right = GameObject.Find("RightSide");
        for (int i = 0; i < 17; i++) {
            left.GetComponent<Image>().color += new Color32(0, 0, 0, 15);
            right.GetComponent<Image>().color += new Color32(0, 0, 0, 15);
            if (isEnemyTurn && right.GetComponent<Image>().color.a > opacityWhenDisabled) {
                right.GetComponent<Image>().color = new Color(0.8f, 0.8f, 0.8f, opacityWhenDisabled);
            }
            if (!isEnemyTurn && left.GetComponent<Image>().color.a > opacityWhenDisabled) {
                left.GetComponent<Image>().color = new Color(0.8f, 0.8f, 0.8f, opacityWhenDisabled);
            }
            yield return new WaitForSeconds(fadeDelay);
        }
        StartCoroutine(SaySentence());
    }

    IEnumerator DialogueFade() {
        Color c = GameObject.Find("Cover").GetComponent<Image>().color;
        GameObject[] chars = GameObject.FindGameObjectsWithTag("Player");
        GameObject box = GameObject.Find("DBox");
        GameObject cover = GameObject.Find("DCover");
        GameObject.Find("LeftSide").GetComponent<Image>().color = new Color(0.8f, 0.8f, 0.8f, opacityWhenDisabled);
        GameObject.Find("RightSide").GetComponent<Image>().color = new Color(0.8f, 0.8f, 0.8f, opacityWhenDisabled);
        for (int i = 0; i < 17; i++) {
            textBox.GetComponent<Text>().color -= new Color32(0, 0, 0, 15);
            dialogueName.GetComponent<Text>().color -= new Color32(0, 0, 0, 15);
            box.GetComponent<Image>().color -= new Color32(0, 0, 0, 15);
            cover.GetComponent<Image>().color -= new Color32(0, 0, 0, 80 / 17);
            foreach (GameObject o in chars) {
                o.GetComponent<Image>().color -= new Color(0, 0, 0, opacityWhenDisabled / 17);
            }
            yield return new WaitForSeconds(fadeDelay);
        }
        dialogueComplete = true;
    }

    IEnumerator SaySentence() {
        while (!UpdateText()) {
            if (!activeSkip) {
                yield return new WaitForSeconds(textDelay);
            }
        }
        isReady = true;
    }

    void RenderNewDialogue(bool firstLoad = false) {
        currentText = "";
        string characterName = "";
        string nameToShowInDialogueBox = "";
        dialogueText = dialogueList[0];
        for (int i = 2; i < dialogueText.Length - 1; i++) {
            if (dialogueText[i] != '/') {
                characterName += dialogueText[i];
            } else {
                break;
            }
        }
        for (int i = characterName.Length + 1; i < dialogueText.Length - 1; i++) {
            if (dialogueText[i] != ':') {
                nameToShowInDialogueBox += dialogueText[i];
            } else {
                break;
            }
        }
        dialogueName.GetComponent<Text>().text = nameToShowInDialogueBox.Substring(2, nameToShowInDialogueBox.Length - 3);
        if (dialogueText[0] == 'E') {
            SetSide(true, characterName);
            isEnemyTurn = true;
            if (!firstLoad) {
                GameObject.Find("LeftSide").GetComponent<Image>().color = new Color(1, 1, 1, 1);
                GameObject.Find("RightSide").GetComponent<Image>().color = new Color(0.8f, 0.8f, 0.8f, opacityWhenDisabled);
            } else {
                GameObject.Find("RightSide").GetComponent<Image>().color = new Color(0.8f, 0.8f, 0.8f, 0);
            }
        } else {
            SetSide(false, characterName);
            isEnemyTurn = false;
            if (!firstLoad) {
                GameObject.Find("LeftSide").GetComponent<Image>().color = new Color(0.8f, 0.8f, 0.8f, opacityWhenDisabled);
                GameObject.Find("RightSide").GetComponent<Image>().color = new Color(1, 1, 1, 1);
            } else {
                GameObject.Find("LeftSide").GetComponent<Image>().color = new Color(0.8f, 0.8f, 0.8f, 0);
            }
        }
        dialogueText = dialogueText.Substring(2 + characterName.Length + nameToShowInDialogueBox.Length, dialogueText.Length - 2 - characterName.Length - nameToShowInDialogueBox.Length);
        dialogueList.RemoveAt(0);
    }

    void Update()
    {
        if (dialogueComplete) return;
        if ((isReady && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space)) && isReady) {
            isReady = false;
            GameObject.Find("PublicVariables").GetComponent<PublicVariables>().ScrollSound();
            if (dialogueList.Count > 0) {
                RenderNewDialogue();
                StartCoroutine(SaySentence());
            } else {
                StartCoroutine(DialogueFade());
            }
        } else if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space)) {
            StartCoroutine(ActiveSkip());
        }
    }

    IEnumerator ActiveSkip() {
        activeSkip = true;
        GameObject.Find("PublicVariables").GetComponent<PublicVariables>().ScrollSound();
        yield return new WaitForSeconds(0.1f);
        activeSkip = false;
    }

    bool UpdateText() {
        if (!activeSkip) {
            GameObject.Find("PublicVariables").GetComponent<PublicVariables>().musicPlayer.PlayOneShot(blip);
        }
        currentText = dialogueText.Substring(0, currentText.Length + 1);
        textBox.GetComponent<Text>().text = currentText;
        return currentText.Length == dialogueText.Length;
    }
}
