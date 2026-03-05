using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeakerPortraitHandler : MonoBehaviour
{
    public GameObject left;
    public GameObject right;
    public InkDialoguePlayer InkDialogueManager; //reference to the dialogue manager to access tags
    public void UpdatePortrait()
    {
        Tag portraitTag = null;

        //find the portrait tag in the current line
        portraitTag = InkDialogueManager.tags.Find(tag => tag.key == "portrait");

        if (portraitTag != null)
        {
            switch (portraitTag.value)
            {
                case "sam":
                    left.SetActive(true);
                    right.SetActive(false);
                    break;
                case "noelle":
                case "norman":
                    right.SetActive(true);
                    left.SetActive(false);
                    break;
                case "yuriko":
                    left.SetActive(true);
                    right.SetActive(false);
                    break;
                case "iron":
                    right.SetActive(true);
                    left.SetActive(false);
                    break;
                default:
                    break;
            }
        }
        else
        {
            print("no portrait tag detected in current line.");
        }
    }
}
