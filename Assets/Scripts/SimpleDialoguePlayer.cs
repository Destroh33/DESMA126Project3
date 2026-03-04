using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//[System.Serializable]
//public struct DialogueLine
//{
//    public string speakerName;
//    public string dialogueText;
//}

public class SimpleDialoguePlayer : MonoBehaviour
{
    // show in inspector
    public TextMeshProUGUI speakerBox;
    public TextMeshProUGUI textBox;
    //[SerializeField] DialogueLine[] sentences;
    [SerializeField] string[] sentences;
    public Button playButton;
    private Typewriter typewriter;
    // private variables
    int currLine = 0;

    private void Start()
    {
        typewriter = GetComponent<Typewriter>();
        typewriter.textBox = textBox;
        PlayNextLine();
    }
    private string DisplayLine(string line)
    {
        // use ':' as a separator in your sentences,
        // marked in SINGLE QUOTES!
        // e.g. "Speaker Name: Your Sentence"
        string[] splitLine = line.Split(':');
        string speaker = splitLine[0];
        string words = splitLine[1];

        //display in UI
        speakerBox.text = speaker;
        //start typewriter coroutine for spoken words
        typewriter.StartTyping(words);
        return words;
    }


    public void PlayNextLine()
    {
        if (currLine < sentences.Length)
        {
            if (typewriter.isTyping)
            {
                typewriter.DisplayWholeLine(DisplayLine(sentences[currLine-1]));
                return;
            }
            // parse dialogue string, and then display text
            DisplayLine(sentences[currLine]);
            //prep for next line
            currLine++;
        }
        else
        {
            Debug.Log("Can't play next line because the dialogue has reached the end.");

            // maybe disable the play button?
            playButton.interactable = false;
        }
    }
}
