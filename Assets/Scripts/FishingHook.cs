using UnityEngine;

public class FishingHook : MonoBehaviour
{
    public bool isInWater { get; private set; }
    public Fish attachedFish { get; private set; }
    public Transform lineAttachPoint;
    
    public Color waterColor = new Color(0.5f, 0.5f, 1f, 0.5f);
    public AudioClip splashSound;
    private SpriteRenderer hookSprite;

    private void Awake()
    {
        hookSprite = GetComponentInChildren<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other.gameObject.name);
        if (other.CompareTag("Water"))
        {
            isInWater = true;
            AudioManager.Instance?.PlaySFX(splashSound);
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

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Water"))
        {
            Debug.Log("In water");
            if (hookSprite != null)
                hookSprite.color = waterColor;

            if (attachedFish != null)
            {
                var fishSr = attachedFish.GetComponentInChildren<SpriteRenderer>();
                if (fishSr)
                    fishSr.color = waterColor;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Water"))
        {
            isInWater = false;
            if (hookSprite != null)
                hookSprite.color = Color.white;

            if (attachedFish != null)
            {
                var fishSr = attachedFish.GetComponentInChildren<SpriteRenderer>();
                if (fishSr)
                    fishSr.color = Color.white;
            }
        }
    }
}
