using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerFishing : MonoBehaviour
{
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

    private GameObject hookInstance;
    private Rigidbody2D hookRb;
    private FishingHook hookScript;
    private bool isCast = false;
    private bool hookHasTraveled = false;
    private bool hasEnteredWater = false;
    private bool wasInWater = false;
    private float moveInputX = 0f;

    private void Update()
    {
        if (!isCast || hookInstance == null)
            return;

        if (!wasInWater && hookScript.isInWater)
            hasEnteredWater = true;
        wasInWater = hookScript.isInWater;

        lineRenderer.SetPosition(0, rodTip.position);
        lineRenderer.SetPosition(1, hookInstance.transform.position);

        Vector2 toRod = (Vector2)rodTip.position - hookRb.position;

        if (!hookHasTraveled && toRod.magnitude > reelFinishDistance)
            hookHasTraveled = true;

        bool isReeling = Keyboard.current.spaceKey.isPressed && hasEnteredWater;

        if (isReeling)
        {
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
            if (moveInputX != 0f)
                hookRb.linearVelocity = new Vector2(moveInputX * hookMoveSpeed, hookRb.linearVelocity.y);
        }
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
        hookInstance = Instantiate(hookPrefab, rodTip.position, Quaternion.identity);
        hookRb = hookInstance.GetComponent<Rigidbody2D>();
        hookScript = hookInstance.GetComponent<FishingHook>() ?? hookInstance.AddComponent<FishingHook>();
        hookRb.AddForce(new Vector2(castForceX, castForceY), ForceMode2D.Impulse);

        lineRenderer.positionCount = 2;
        lineRenderer.enabled = true;
    }

    void FinishFishing()
    {
        isCast = false;
        Destroy(hookInstance);
        hookInstance = null;
        hookRb = null;
        hookScript = null;
        lineRenderer.enabled = false;
        Debug.Log("fishing finished");
    }
}
