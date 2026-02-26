using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Ink.Runtime;
using UnityEngine.Events;
public class InkDialoguePlayer : MonoBehaviour
{
    public TextMeshProUGUI speakerBox;
    public TextMeshProUGUI textBox;
    public Button playButton;
    private Typewriter typewriter;
    int currLine = 0;

    [Header("StoryText")]
    [SerializeField] TextAsset storyJSON;
    public Story currentStory;

    [Header("DialogueOptions")]
    public Button[] choices;
    TextMeshProUGUI[] choicesText;
    public List<Tag> tags;
    public UnityEvent tagEvents;
    private void Start()
    {
        typewriter = GetComponent<Typewriter>();
        typewriter.textBox = textBox;
        EnterStoryFromJSONText(storyJSON.text);
        ContinueStory();

        choicesText = new TextMeshProUGUI[choices.Length];
        for (int i = 0; i < choices.Length; i++)
        {
            choicesText[i] = choices[i].GetComponentInChildren<TextMeshProUGUI>();
        }
    }
    public void MakeChoice(int choiceIndex)
    {
        if (!currentStory.canContinue)
        {
            Debug.Log("making choice at index" + choiceIndex);
            currentStory.ChooseChoiceIndex(choiceIndex);
            ContinueStory();
        }
    }
    void TryDisplayChoices()
    {
        HideAllChoices();
        List<Choice> currentChoices = currentStory.currentChoices;
        if (currentChoices.Count > choices.Length)
        {
            Debug.LogError("More choices were given than the UI can support. Number of choices given: " + currentChoices.Count);
        }
        else
        {
            for (int i = 0; i < currentChoices.Count; i++)
            {
                choices[i].gameObject.SetActive(true);
                choicesText[i].text = currentChoices[i].text;
            }
        }
    }
    void HideAllChoices()
    {
        foreach (Button choice in choices)
        {
            choice.gameObject.SetActive(false);
        }
    }
    public void ContinueStory()
    {
        if ((typewriter.isTyping))
        {
            typewriter.StopTyping();
            DisplayLine(currentStory.currentText, false);
        }
        else
        {
            if ((currentStory.canContinue))
            {
                string newLine = currentStory.Continue();
                DisplayLine(newLine);
                if(currentStory.currentTags.Count > 0)
                {
                    GetTags(currentStory.currentTags);
                    tagEvents.Invoke();
                }
            }
            else
            {
                Debug.LogWarning("Can't continue story");
            }
        }
    }
    public void EnterStoryFromJSONText(string json)
    {
        currentStory = new Story(json);
    }
    private void DisplayLine(string line, bool typewriteIsOn = true)
    {
        string[] splitLine = line.Split(':');
        string speaker = splitLine[0];
        string words = splitLine[1];
        speakerBox.text = speaker;
        if(typewriteIsOn)
            typewriter.StartTyping(words);
        else
            typewriter.DisplayWholeLine(words);

        TryDisplayChoices();

    }
    void GetTags(List<string> currentTags)
    {
        if (tags != null)
        {
            tags.Clear();
        }
        else
        {
            tags = new List<Tag>();
        }
        foreach(string tag in currentTags)
        {
            string[] splitTag = tag.Split(':');
            if(splitTag.Length != 2)
            {
                Debug.LogError("Tag is not in the correct format! Tag: " + tag);
            }
            string key = splitTag[0].Trim();
            string value = splitTag[1].Trim();
            tags.Add(new Tag { key = key, value = value });
        }
    }
}
[System.Serializable]
public class Tag
{
    public string key;
    public string value;
}