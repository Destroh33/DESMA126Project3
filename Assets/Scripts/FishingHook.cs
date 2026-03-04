using UnityEngine;

public class FishingHook : MonoBehaviour
{
    public bool isInWater { get; private set; }
    public Fish attachedFish { get; private set; }
    public Transform lineAttachPoint;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other.gameObject.name);
        if (other.CompareTag("Water"))
        {
            isInWater = true;
            return;
        }

        if (attachedFish == null)
        {
            var fish = other.GetComponent<Fish>();
            if (fish != null)
            {
                attachedFish = fish;
                fish.GetCaught(this);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Water"))
            isInWater = false;
    }
}
