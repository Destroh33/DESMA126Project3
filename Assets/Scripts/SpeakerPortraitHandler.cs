using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeakerPortraitHandler : MonoBehaviour
{
    public GameObject left;
    // specific right-side portraits
    public GameObject rightNoelle;
    public GameObject rightNorman;
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
                    DisableAllRight();
                    break;
                case "noelle":
                    EnableRightSpecific(rightNoelle);
                    left.SetActive(false);
                    break;
                case "norman":
                    EnableRightSpecific(rightNorman);
                    left.SetActive(false);
                    break;
                case "yuriko":
                    left.SetActive(true);
                    DisableAllRight();
                    break;
                case "iron":
                    // no right portrait for iron anymore
                    left.SetActive(false);
                    DisableAllRight();
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

    // helper methods to handle specific right sprites
    private void DisableAllRight()
    {
        if (rightNoelle != null) rightNoelle.SetActive(false);
        if (rightNorman != null) rightNorman.SetActive(false);
    }

    private void EnableRightSpecific(GameObject go)
    {
        DisableAllRight();
        if (go != null) go.SetActive(true);
    }
}
