using System;
using Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public InputActionAsset InputActions;
    
    private InputAction attackAction;
    private GameObject currentEnemy;
    private Animator currentEnemyAnimator;
    [SerializeField] private TMP_Text TXT_Score;
    [SerializeField] private TMP_Text TXT_EnemyName;
    
    [SerializeField] private float StartDamage;
    private IKnightEnemy _currentEnemyInfo;
    private bool _isAttacked = false;

    private void Awake()
    {
        attackAction = InputActions.FindActionMap("Player").FindAction("Attack");
    }

    private void Start()
    {
        _currentEnemyInfo = new MouseEnemy();
        TXT_EnemyName.SetText(_currentEnemyInfo.GetName());
        currentEnemy = GameObject.FindGameObjectWithTag("Enemy");
        currentEnemyAnimator = currentEnemy.GetComponent<Animator>();
    }

    private void Update()
    {
        if (attackAction.WasPressedThisFrame())
        {
            _isAttacked = true;
            int score = int.Parse(TXT_Score.GetParsedText());
            TXT_Score.SetText((score+StartDamage).ToString());
        }
    }

    private void FixedUpdate()
    {
        if (_isAttacked)
        {
            currentEnemyAnimator.SetBool("isHurt", true);
            _isAttacked = false;
        }
    }
}
