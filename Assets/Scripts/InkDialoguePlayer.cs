using System;
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
    public GameObject choicesPanel;
    public Button[] choices;
    TextMeshProUGUI[] choicesText;

    public List<Tag> tags;
    public UnityEvent tagEvents;
    public UnityEvent onStoryEnd;

    /// <summary>Fired when the Ink story calls ~ TriggerFishSelection(). onStoryEnd will NOT fire for that story.</summary>
    public event Action OnFishSelectionRequested;
    private bool fishSelectionPending = false;
    private bool oneLineMode = false;
    private string oneLineMessage = "";

    private void Start()
    {
        typewriter = GetComponent<Typewriter>();
        typewriter.textBox = textBox;

        choicesText = new TextMeshProUGUI[choices.Length];
        for (int i = 0; i < choices.Length; i++)
        {
            choicesText[i] = choices[i].GetComponentInChildren<TextMeshProUGUI>(true);
        }

        if (!storyJSON)
            return;

        EnterStoryFromJSONText(storyJSON.text);
        ContinueStory();
    }

    public void LoadStory(TextAsset story)
    {
        typewriter = GetComponent<Typewriter>();
        typewriter.textBox = textBox;

        if (!story)
        {
            Debug.LogError("LoadStory was called with null story TextAsset.");
            return;
        }

        EnterStoryFromJSONText(story.text);
        ContinueStory();
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
        else if (currentChoices.Count > 0)
        {
            choicesPanel.SetActive(true);
            for (int i = 0; i < currentChoices.Count; i++)
            {
                choices[i].gameObject.SetActive(true);
                choicesText[i].text = currentChoices[i].text;
            }
        }
    }

    void HideAllChoices()
    {
        choicesPanel.SetActive(false);
        foreach (Button choice in choices)
        {
            choice.gameObject.SetActive(false);
        }
    }

    /// <summary>Display a single message line and fire onStoryEnd on the next button press.</summary>
    public void ShowOneLineAndEnd(string speaker, string message)
    {
        fishSelectionPending = false;
        oneLineMode = true;
        currentStory = null;

        typewriter = GetComponent<Typewriter>();
        typewriter.textBox = textBox;
        HideAllChoices();

        oneLineMessage = message;
        speakerBox.text = speaker;
        typewriter.StartTyping(message);
    }

    public void ContinueStory()
    {
        if (oneLineMode)
        {
            if (typewriter.isTyping)
            {
                typewriter.StopTyping();
                typewriter.DisplayWholeLine(oneLineMessage);
                return;
            }
            oneLineMode = false;
            onStoryEnd.Invoke();
            return;
        }

        if (typewriter.isTyping)
        {
            typewriter.StopTyping();
            DisplayLine(currentStory.currentText, false);
            return;
        }
        if (currentStory.canContinue)
        {
            string newLine = currentStory.Continue();

            if (string.IsNullOrWhiteSpace(newLine))
            {
                // Empty line (e.g. from a bare ~ function call) — skip display and resolve immediately
                TryDisplayChoices();
                if (currentStory.currentChoices.Count == 0)
                {
                    if (fishSelectionPending)
                    {
                        fishSelectionPending = false;
                        OnFishSelectionRequested?.Invoke();
                    }
                    else
                    {
                        onStoryEnd.Invoke();
                    }
                }
                return;
            }

            DisplayLine(newLine);

            if (currentStory.currentTags.Count > 0)
            {
                GetTags(currentStory.currentTags);
                tagEvents.Invoke();
            }
        }
        else
        {
            TryDisplayChoices();

            if (currentStory.currentChoices.Count == 0)
            {
                if (fishSelectionPending)
                {
                    fishSelectionPending = false;
                    OnFishSelectionRequested?.Invoke();
                }
                else
                {
                    onStoryEnd.Invoke();
                }
            }
        }
    }

    public void EnterStoryFromJSONText(string json)
    {
        fishSelectionPending = false;
        currentStory = new Story(json);
        currentStory.BindExternalFunction("TriggerFishSelection", () =>
        {
            fishSelectionPending = true;
        });
    }

    private void DisplayLine(string line, bool typewriteIsOn = true)
    {
        string speaker = "";
        string words = line;

        string[] splitLine = line.Split(':');
        if (splitLine.Length >= 2)
        {
            speaker = splitLine[0];
            words = string.Join(":", splitLine, 1, splitLine.Length - 1);
        }
        else
        {
            speaker = "";
            words = line;
        }

        speakerBox.text = speaker;

        if (typewriteIsOn)
            typewriter.StartTyping(words);
        else
            typewriter.DisplayWholeLine(words);

        TryDisplayChoices();
    }

    void GetTags(List<string> currentTags)
    {
        if (tags != null)
            tags.Clear();
        else
            tags = new List<Tag>();

        foreach (string tag in currentTags)
        {
            string[] splitTag = tag.Split(':');
            if (splitTag.Length != 2)
            {
                Debug.LogError("Tag is not in the correct format! Tag: " + tag);
                continue;
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