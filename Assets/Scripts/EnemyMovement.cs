using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    public Animator animator;
    public GameObject Player;
    public float LookSpeed;
    public float StopDistance;
    public float MovementSpeed;
    [HideInInspector]
    public bool UpdateMovement = true;
    [HideInInspector]
    public bool IsMoving = true;

    private NavMeshAgent navigation;
    private float PlayerDistanceToleration = 0.15f;

    void Start()
    {
        if (Player == null) Player = GameObject.FindGameObjectWithTag("Player");
        navigation = GetComponent<NavMeshAgent>();
        //navigation.stoppingDistance = StopDistance;
    }

    void Update()
    {
        UpdateRoation();
        if (UpdateMovement) UpdateNavigation();
    }

    private void UpdateNavigation()
    {
        float Distance = Vector3.Distance(transform.position, Player.transform.position);
        if (Distance > StopDistance + (IsMoving ? PlayerDistanceToleration / 2 : PlayerDistanceToleration))
        {
            navigation.isStopped = false;
            navigation.speed = MovementSpeed;
            navigation.SetDestination(Player.transform.position);
            IsMoving = true;
            animator.SetBool("IsMoving", true);
        }
        else if (Distance < StopDistance - (IsMoving ? PlayerDistanceToleration / 2 : PlayerDistanceToleration))
        {
            Vector3 newDestination = Player.transform.position + ((transform.position - Player.transform.position).normalized * StopDistance * 2);
            navigation.isStopped = false;
            navigation.speed = MovementSpeed * 3;
            navigation.SetDestination(newDestination);
            IsMoving = true;
            animator.SetBool("IsMoving", true);
        }
        else
        {
            navigation.isStopped = true;
            IsMoving = false;
            animator.SetBool("IsMoving", false);
        }
    }

    private void UpdateRoation()
    {
        Quaternion lookRotation = Quaternion.LookRotation(Player.transform.position - transform.position);
        lookRotation.eulerAngles = new Vector3(transform.rotation.eulerAngles.x, lookRotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * LookSpeed);
    }
}
