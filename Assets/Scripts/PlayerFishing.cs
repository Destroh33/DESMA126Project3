using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerFishing : MonoBehaviour
{
    // Static flag to indicate if returning from fishing
    public static bool ReturningFromFishing = false;
    [Header("References")]
    public GameObject hookPrefab;
    public Transform rodTip;
    public LineRenderer lineRenderer;

    [Header("Casting")]
    public float castForceX = -8f;
    public float castForceY = 12f;

    [Header("Hook Movement")]
    public float hookMoveSpeed = 3f;
    public float waterSinkSpeed = 1f;
    public float reelSpeed = 6f;
    public float reelFinishDistance = 0.5f;

    [Header("Line")]
    public float maxLineLength = 10f;
    public int linePoints = 24;
    public float sagFactor = 0.5f;
    public float sagTransitionSpeed = 3f;

    private GameObject hookInstance;
    private Rigidbody2D hookRb;
    private FishingHook hookScript;
    private bool isCast = false;
    private bool hookHasTraveled = false;
    private bool hasEnteredWater = false;
    private bool wasInWater = false;
    private bool isRetractingAfterWater = false;
    private float moveInputX = 0f;
    private float currentLineLength;
    private float sagTransitionT = 0f;
    private float sagMultiplier = 1f;

    private void Update()
    {
        if (!isCast || hookInstance == null)
        {
            return;
        }

        bool justLeftWater = wasInWater && !hookScript.isInWater;
        if (!wasInWater && hookScript.isInWater)
            hasEnteredWater = true;
        if (hasEnteredWater && justLeftWater)
            isRetractingAfterWater = true;
        wasInWater = hookScript.isInWater;

        bool isReeling = (Keyboard.current.spaceKey.isPressed && hasEnteredWater) || isRetractingAfterWater;

        sagTransitionT = hasEnteredWater
            ? Mathf.MoveTowards(sagTransitionT, 1f, Time.deltaTime * sagTransitionSpeed)
            : 0f;

        sagMultiplier = Mathf.MoveTowards(sagMultiplier, isReeling ? 0f : 1f, Time.deltaTime * sagTransitionSpeed);

        UpdateLine();

        Vector2 toRod = (Vector2)rodTip.position - hookRb.position;

        if (!hookHasTraveled && toRod.magnitude > reelFinishDistance)
            hookHasTraveled = true;

        if (isReeling)
        {
            currentLineLength -= reelSpeed * Time.deltaTime;
            currentLineLength = Mathf.Max(0f, currentLineLength);

            hookRb.gravityScale = 0f;
            hookRb.linearVelocity = toRod.normalized * reelSpeed;

            if (hookHasTraveled && toRod.magnitude <= reelFinishDistance)
                FinishFishing();
        }
        else if (hookScript.isInWater)
        {
            hookRb.gravityScale = 0f;
            hookRb.linearVelocity = new Vector2(moveInputX * hookMoveSpeed, -waterSinkSpeed);
        }
        else
        {
            hookRb.gravityScale = 1f;
            // no movement control until hook has entered water
        }
    }

    void UpdateLine()
    {
        Vector2 start = rodTip.position;
        Vector2 end = hookScript.lineAttachPoint != null
            ? (Vector2)hookScript.lineAttachPoint.position
            : (Vector2)hookInstance.transform.position;

        float straightDist = Vector2.Distance(start, end);
        float slack = Mathf.Max(0f, currentLineLength - straightDist);
        float offset = Mathf.Lerp(-straightDist * sagFactor, slack * sagFactor, sagTransitionT) * sagMultiplier;

        Vector2 mid = (start + end) * 0.5f;
        Vector2 controlPoint = mid + Vector2.down * offset;

        for (int i = 0; i < linePoints; i++)
        {
            float t = i / (float)(linePoints - 1);
            lineRenderer.SetPosition(i, QuadraticBezier(start, controlPoint, end, t));
        }
    }

    static Vector2 QuadraticBezier(Vector2 a, Vector2 b, Vector2 c, float t)
    {
        float u = 1f - t;
        return u * u * a + 2f * u * t * b + t * t * c;
    }

    void OnUseRod()
    {
        if (!isCast)
            Cast();
    }

    void OnMove(InputValue v)
    {
        moveInputX = v.Get<Vector2>().x;
    }

    void Cast()
    {
        isCast = true;
        hookHasTraveled = false;
        hasEnteredWater = false;
        wasInWater = false;
        isRetractingAfterWater = false;
        sagTransitionT = 0f;
        sagMultiplier = 1f;
        currentLineLength = maxLineLength;

        hookInstance = Instantiate(hookPrefab, rodTip.position, Quaternion.identity);
        hookRb = hookInstance.GetComponent<Rigidbody2D>();
        hookScript = hookInstance.GetComponent<FishingHook>() ?? hookInstance.AddComponent<FishingHook>();
        hookRb.AddForce(new Vector2(castForceX, castForceY), ForceMode2D.Impulse);

        lineRenderer.positionCount = linePoints;
        lineRenderer.enabled = true;
    }

    void FinishFishing()
    {
        if (hookScript.attachedFish != null && FishingInventory.Instance != null)
            FishingInventory.Instance.AddFish(hookScript.attachedFish);

        isCast = false;
        Destroy(hookInstance); // also destroys parented fish
        hookInstance = null;
        hookRb = null;
        hookScript = null;
        sagTransitionT = 0f;
        sagMultiplier = 1f;
        isRetractingAfterWater = false;
        lineRenderer.enabled = false;
        Debug.Log("fishing finished");

        // Set flag and load scene
        ReturningFromFishing = true;
        SceneManager.LoadScene("TopDownScene");
    }
}
