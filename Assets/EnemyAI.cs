using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class enemyAI : MonoBehaviour
{
    [Header("Navigation Settings")]
    public NavMeshAgent ai;
    public List<Transform> destinations;
    public float walkSpeed = 3.5f;
    public float chaseSpeed = 7f;
    private Transform currentDest;

    [Header("Detection Settings")]
    public float fieldOfView = 110f;
    public float sightDistance = 20f;
    public float catchDistance = 2f;
    public Vector3 rayCastOffset;

    [Header("Search Settings")]
    public float searchRadius = 10f;
    public float minSearchTime = 3f;
    public float maxSearchTime = 7f;
    public float rotationSpeed = 120f;
    public int searchPoints = 3;

    [Header("Idle Settings")]
    public float minIdleTime = 2f;
    public float maxIdleTime = 5f;

    [Header("Misc Settings")]
    public string deathScene;
    public float jumpscareTime = 1.5f;
    public bool visualizeVisionCone = true;

    [Header("References")]
    public Transform player;

    public float aiDistance;
    private bool walking = true, chasing, searching;
    private Vector3 lastKnownLocation;
    private Quaternion originalRotation;

    void Start()
    {
        currentDest = destinations[Random.Range(0, destinations.Count)];
        ai.speed = walkSpeed;
        originalRotation = transform.rotation;
    }

    void Update()
    {
        aiDistance = Vector3.Distance(player.position, transform.position);
        bool playerVisible = IsPlayerVisible();

        if (playerVisible)
        {
            lastKnownLocation = player.position;
            if (!chasing) StartChasing();
        }

        if (chasing)
        {
            ChasePlayer();
            if (!playerVisible) SwitchToSearching();
        }
        else if (searching)
        {
            SearchLOSCheck();
        }
        else if (walking)
        {
            Patrol();
        }

        if (visualizeVisionCone) VisualizeVisionCone();
    }

    private bool IsPlayerVisible()
    {
        if (aiDistance > sightDistance) return false;

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);

        if (angle > fieldOfView / 2) return false;

        RaycastHit hit;
        if (Physics.Raycast(transform.position + rayCastOffset, directionToPlayer, out hit, sightDistance))
        {
            return hit.collider.CompareTag("Player");
        }
        return false;
    }

    private void StartChasing()
    {
        chasing = true;
        searching = false;
        walking = false;
        ai.speed = chaseSpeed;
        ai.autoBraking = false;
        StopAllCoroutines();
    }

    private void ChasePlayer()
    {
        ai.destination = player.position;

        if (aiDistance <= catchDistance)
        {
            StartCoroutine(TriggerJumpscare());
        }
    }

    private void SwitchToSearching()
    {
        chasing = false;
        searching = true;
        ai.speed = walkSpeed;
        ai.destination = lastKnownLocation;
        StartCoroutine(SearchRoutine());
    }

    private IEnumerator SearchRoutine()
    {
        yield return new WaitUntil(() => ai.remainingDistance <= ai.stoppingDistance);

        for (int i = 0; i < searchPoints; i++)
        {
            Vector3 searchPoint = GetRandomSearchPoint();
            ai.destination = searchPoint;

            yield return new WaitUntil(() => ai.remainingDistance <= ai.stoppingDistance);

            yield return StartCoroutine(LookAround());
        }

        searching = false;
        walking = true;
        currentDest = destinations[Random.Range(0, destinations.Count)];
    }

    private IEnumerator LookAround()
    {
        float searchTimer = 0f;
        float searchDuration = Random.Range(minSearchTime, maxSearchTime);
        float targetAngle = Random.Range(-360f, 360f);

        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0) * startRotation;

        while (searchTimer < searchDuration)
        {
            if (IsPlayerVisible())
            {
                StartChasing();
                yield break;
            }

            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );

            if (Quaternion.Angle(transform.rotation, targetRotation) < 1f)
            {
                targetAngle *= -1;
                targetRotation = Quaternion.Euler(0, targetAngle, 0) * startRotation;
            }

            searchTimer += Time.deltaTime;
            yield return null;
        }

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            originalRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    private Vector3 GetRandomSearchPoint()
    {
        Vector3 randomPoint = lastKnownLocation + Random.insideUnitSphere * searchRadius;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomPoint, out hit, searchRadius, NavMesh.AllAreas);
        return hit.position;
    }

    private void Patrol()
    {
        ai.destination = currentDest.position;

        if (ai.remainingDistance <= ai.stoppingDistance)
        {
            StartCoroutine(IdleRoutine());
        }
    }

    private IEnumerator IdleRoutine()
    {
        walking = false;
        yield return new WaitForSeconds(Random.Range(minIdleTime, maxIdleTime));
        walking = true;
        currentDest = destinations[Random.Range(0, destinations.Count)];
    }

    private IEnumerator TriggerJumpscare()
    {
        player.gameObject.SetActive(false);
        yield return new WaitForSeconds(jumpscareTime);
        SceneManager.LoadScene(deathScene);
    }

    private void SearchLOSCheck()
    {
        if (IsPlayerVisible())
        {
            StartChasing();
        }
    }

    private void VisualizeVisionCone()
    {
        Vector3 origin = transform.position + Vector3.up;
        float halfFOV = fieldOfView / 2;

        for (int i = 0; i <= 10; i++)
        {
            float angle = Mathf.Lerp(-halfFOV, halfFOV, i / 10f);
            Vector3 dir = Quaternion.Euler(0, angle, 0) * transform.forward;
            Debug.DrawRay(origin, dir * sightDistance, Color.yellow);
        }
    }

    public void OnPlayerHidden()
    {
        if (chasing) SwitchToSearching();
    }
}