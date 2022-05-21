using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public partial class Animal : NetworkBehaviour
{
    private NavMeshAgent agent;
    [SerializeField] private Animator animator;
    private static readonly int attackCache = Animator.StringToHash("Attack");
    private static readonly int speedCache = Animator.StringToHash("Speed");


    private void Start()
    {
        Random.InitState((int)System.DateTime.Now.Ticks);
        Random.state = new Random.State {};
    
        agent = GetComponent<NavMeshAgent>();

        maxHealth += Random.Range(-20, 21);
        currentHealth.Value = maxHealth;
        
        animalStates.Add(idleState, IdleState());
        animalStates.Add(roamState, RoamState());
        animalStates.Add(fleeState, FleeState());
        animalStates.Add(attackState, AttackState());
    }

    private void Update()
    {
        if (!dead) ChangeState();
        animalStates[CurrentState].onUpdate.Invoke();
        animator.SetFloat(speedCache, agent.velocity.magnitude);
    }
}
