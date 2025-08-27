using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator> ();
    }

    void FinishHurt()
    {
        _animator.SetBool("isHurt", false);
    }
}
