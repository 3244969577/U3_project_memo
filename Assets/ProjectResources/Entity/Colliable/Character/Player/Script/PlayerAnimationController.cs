using UnityEngine;
using GameStatusSystem.PlayerStatus.Events;

public class PlayerAnimationController : MonoBehaviour {
    private Animator animator;
    private EventBinding<PlayerDieEvent> playerDieEventBinding;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void Start()
    {
        playerDieEventBinding = new EventBinding<PlayerDieEvent>(OnPlayerDie);
    }
    private void OnPlayerDie(PlayerDieEvent e)
    {
        animator.SetTrigger("Die");
    }
}