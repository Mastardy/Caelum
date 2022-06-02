using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public partial class Animal : NetworkBehaviour
{
    private NavMeshAgent agent;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject model;
    private static readonly int attackCache = Animator.StringToHash("Attack");
    private static readonly int speedCache = Animator.StringToHash("Speed");
    private static readonly int hitCache = Animator.StringToHash("Hit");
    private static readonly int deadCache = Animator.StringToHash("Dead");
    public UnityEvent onDestroy;
    
    private void Start()
    {
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

        RaycastHit hit;
        if(Physics.Linecast(transform.position, transform.position - new Vector3(0, 10, 0), out hit , -LayerMask.NameToLayer("Ground")))
        {
            model.transform.position = hit.point;
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        onDestroy?.Invoke();
    }
}
