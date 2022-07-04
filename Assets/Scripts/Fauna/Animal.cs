using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

[System.Serializable]
public struct Drop
{
    public InventoryItem item;
    public int amount;
}

public partial class Animal : NetworkBehaviour
{
    private NavMeshAgent agent;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject model;
    [SerializeField] private SkinnedMeshRenderer modelRenderer;
    [SerializeField] private bool agressive = true;
    [SerializeField] private LayerMask groundMask;
    public Drop[] drops;

    private static readonly int attackCache = Animator.StringToHash("Attack");
    private static readonly int speedCache = Animator.StringToHash("Speed");
    private static readonly int hitCache = Animator.StringToHash("Hit");
    private static readonly int deadCache = Animator.StringToHash("Dead");

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        currentHealth.Value = maxHealth;
        
        animalStates.Add(idleState, IdleState());
        animalStates.Add(roamState, RoamState());
        animalStates.Add(fleeState, FleeState());
        animalStates.Add(attackState, AttackState());
    }

    private void FixedUpdate()
    {
        if(Physics.Linecast(transform.position, transform.position - new Vector3(0, 10, 0), out RaycastHit hit, groundMask))
        {
            model.transform.position = Vector3.Lerp(model.transform.position, hit.point, Time.fixedDeltaTime * 2);
        }
    }

    private void Update()
    {
        if (!agent.enabled) return;
        if (!dead) ChangeState();
        animalStates[CurrentState].onUpdate.Invoke();
        animator.SetFloat(speedCache, agent.velocity.magnitude);
    }
}
