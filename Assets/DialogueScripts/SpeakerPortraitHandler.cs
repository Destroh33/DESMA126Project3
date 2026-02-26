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
                case "left":
                    right.gameObject.SetActive(false);
                    left.gameObject.SetActive(true);
                    break;
                case "right":
                    right.gameObject.SetActive(true);
                    left.gameObject.SetActive(false);
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
