using UnityEngine;
using UnityEngine.InputSystem; 

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float baseMoveSpeed = 8f;

    [Header("SpeedUp Powerup")]
    public float speedBoostMultiplier = 1.8f;
    public float speedBoostDuration = 5f;

    Rigidbody rb;
    float boostTimer = 0f;

    void Awake() { rb = GetComponent<Rigidbody>(); }

    void Update()
    {
        float h = 0f, v = 0f;

        var kb = Keyboard.current;
        if (kb != null)
        {
            if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) h -= 1f;
            if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) h += 1f;
            if (kb.sKey.isPressed || kb.downArrowKey.isPressed) v -= 1f;
            if (kb.wKey.isPressed || kb.upArrowKey.isPressed) v += 1f;
        }



        float currentSpeed = baseMoveSpeed * (boostTimer > 0f ? speedBoostMultiplier : 1f);
        Vector3 input = new Vector3(h, 0f, v).normalized * currentSpeed;
        Vector3 worldVel = transform.TransformDirection(input);
        rb.linearVelocity = new Vector3(worldVel.x, rb.linearVelocity.y, worldVel.z);
        if (boostTimer > 0f) boostTimer -= Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pickup"))
            GameManager.Instance.OnPickup(other.gameObject);
        else if (other.CompareTag("Enemy"))
            GameManager.Instance.OnHitEnemy();
        else if (other.CompareTag("Goal"))
            GameManager.Instance.OnReachGoal();
        else if (other.CompareTag("KillZone"))
            GameManager.Instance.OnFallOut();
        else if (other.CompareTag("Speedup"))
        {
            boostTimer = speedBoostDuration; 
            Destroy(other.gameObject);
        }
    }
}
