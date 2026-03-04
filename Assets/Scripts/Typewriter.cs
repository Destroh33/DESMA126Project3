using System.Collections;
using TMPro;
using UnityEngine;

public class Typewriter : MonoBehaviour
{
    public TextMeshProUGUI textBox; //add dialogue textbox in Inspector
    public float typeDelayInterval = 0.1f;
    Coroutine typingLinesCoroutine;
    public bool isTyping;

    private void Start()
    {
        if(textBox)
            textBox.text = "";
    }

    public void StartTyping(string line)
    {
        if (isTyping)
        {
            StopTyping();
        }
        typingLinesCoroutine = StartCoroutine(TypingLines(line));
    }
    public void DisplayWholeLine(string line)
    {
        textBox.text = line;
    }

    IEnumerator TypingLines(string line)
    {
        isTyping = true;
        textBox.text = "";

        // convert string into character array
        char[] lineCharArray = line.ToCharArray();

        for (int i = 0; i < lineCharArray.Length; i++)
        {
            textBox.text += lineCharArray[i];

            //maybe a sound effect plays per letter typed!

            yield return new WaitForSeconds(typeDelayInterval);

        }

        isTyping = false;
    }

    public void StopTyping()
    {
        if (typingLinesCoroutine != null)
        {
            StopCoroutine(typingLinesCoroutine);
            isTyping = false;
        }
    }
}
