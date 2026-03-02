using UnityEngine;

public class FishingHook : MonoBehaviour
{
    public bool isInWater { get; private set; }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Water"))
            isInWater = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Water"))
            isInWater = false;
    }
}
