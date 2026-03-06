using UnityEngine;

public enum FishType { Blue, Catfish, Rainbow, Bass, Tuna }

public class Fish : MonoBehaviour
{
    [Header("Type")]
    public FishType fishType;

    [Header("Movement")]
    public float swimSpeed = 2f;
    public float sinAmplitude = 0.04f;
    public float sinFrequency = 2f;
    public float direction = 1f;//right = 1 left = -1

    public Sprite invImage;


    private float baseY;
    private float phaseOffset;
    private bool isCaught = false;
    private Vector3 originalScale;

    void Start()
    {
        baseY = transform.position.y;
        phaseOffset = Random.Range(0f, Mathf.PI * 2f);
        originalScale = transform.localScale;

        var sr = GetComponent<SpriteRenderer>();
        if(sr==null)
            sr = GetComponentInChildren<SpriteRenderer>();
        
        if (sr != null)
            sr.flipX = direction < 0f;
    }

    void Update()
    {
        if (isCaught) return;

        float x = transform.position.x + direction * swimSpeed * Time.deltaTime;
        float y = baseY + Mathf.Sin(Time.time * sinFrequency + phaseOffset) * sinAmplitude;
        transform.position = new Vector3(x, y, transform.position.z);
    }

    public void GetCaught(FishingHook hook)
    {
        if (isCaught) return;
        isCaught = true;

        transform.localScale = originalScale * 0.5f;
        transform.SetParent(hook.transform);
        transform.localPosition = Vector3.zero;

        var rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.simulated = false;

        var col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isCaught && other.CompareTag("Destroyer"))
            Destroy(gameObject);
    }
}
