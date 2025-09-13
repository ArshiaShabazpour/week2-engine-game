using UnityEngine;


public class EnemyController : MonoBehaviour
{
    public Transform player;
    public float baseSpeed = 6f;


    [Header("Slow Effect")]
    public float slowMultiplier = 0.4f; 
    public float slowDuration = 4f;


    float slowTimer = 0f;
    public float SlowRemaining => Mathf.Max(0f, slowTimer);


    void Update()
    {
        if (!player) return;


        Vector3 toPlayer = player.position - transform.position;
        toPlayer.y = 0f;
        if (toPlayer.sqrMagnitude > 0.01f)
        {
            float spd = baseSpeed * (slowTimer > 0f ? slowMultiplier : 1f);
            transform.position += toPlayer.normalized * spd * Time.deltaTime;
        }


        if (slowTimer > 0f) slowTimer -= Time.deltaTime;
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Slowdown"))
        {
            slowTimer = slowDuration; 
            Destroy(other.gameObject); 
        }
    }
}