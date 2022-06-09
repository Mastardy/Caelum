using Unity.Netcode;
using UnityEngine;

public class BatucaPlant : MonoBehaviour
{
    private Animator animator;
    private bool trackPlayer;
    private bool attacking;

    public Transform headAnchor;
    public float speed = 1;
    private Transform player;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!trackPlayer) {
            Quaternion defaultRotation = Quaternion.LookRotation((headAnchor.position + transform.up) - headAnchor.position, headAnchor.up);
            headAnchor.rotation = Quaternion.Slerp(headAnchor.rotation, defaultRotation, speed * Time.deltaTime);
            return;
        }

        if (attacking) return;

        Vector3 playerposition = player.position + new Vector3(0, 2, 0);
        Quaternion targetRotation = Quaternion.LookRotation(playerposition - headAnchor.position, headAnchor.up);
        headAnchor.rotation = Quaternion.Slerp(headAnchor.rotation, targetRotation, speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            trackPlayer = true;
            animator.SetBool("Attack", true);
            player = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            trackPlayer = false;
            animator.SetBool("Attack", false);
            player = null;
        }
    }

    public void StartAttack()
    {
        attacking = true;
    }
    public void FinishAttack()
    {
        attacking = false;
    }
}
