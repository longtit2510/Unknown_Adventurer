using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatController : MonoBehaviour
{
    [SerializeField]
    private bool combatEnabled;
    private bool gotInput;
    private bool isAttacking;
    private bool isFirstAttack;
    
    [SerializeField]
    private float stunDamageAmount = 1f;
    [SerializeField]
    private float inputTimer, attack1Radius, attack1Damage;
    private float lastInputTime = Mathf.NegativeInfinity;

    [SerializeField]
    private Transform attack1HitBoxPos;
    [SerializeField]
    private LayerMask whatisDamageable;

    private AttackDetails attackDetails;

    private Animator anim;
    private PlayerController PC;
    private PlayerStat PS;

    private void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetBool(TagManager.CANATTACK_ANIMATION_PARAMETER, combatEnabled);
        PC = GetComponent<PlayerController>();
        PS = GetComponent<PlayerStat>();
    }
    private void Update()
    {
        CheckCombatInput();
        CheckAttacks();
    }

    private void CheckCombatInput()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            //Attempt combat
            gotInput = true;
            lastInputTime = Time.time;
        }
    }
    private void CheckAttacks()
    {
        if (gotInput)
        {
            //Perform Attack1
            if (!isAttacking)
            {
                FindObjectOfType<AudioManager>().Play("Swing Sword");
                gotInput = false;
                isAttacking = true;
                isFirstAttack = !isFirstAttack;
                anim.SetBool(TagManager.ATTACK1_ANIMATION_PARAMETER, true);
                anim.SetBool(TagManager.FIRSTATTACK_ANIMATION_PARAMETER, isFirstAttack);
                anim.SetBool(TagManager.ISATTACKING_ANIMATION_PARAMETER, isAttacking);
            }
        }
        if(Time.time >= lastInputTime + inputTimer)
        {
            //Wait for new input
            gotInput = false;
        }
    }

    private void CheckAttackHitBox()
    {
        Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(attack1HitBoxPos.position, attack1Radius, whatisDamageable);
        attackDetails.damageAmount = attack1Damage;
        attackDetails.position = transform.position;
        attackDetails.stunDamageAmount = stunDamageAmount;
        
        foreach (Collider2D collider in detectedObjects)
        {
            collider.transform.parent.SendMessage("Damage", attackDetails);
            //Instantiate hit particle
        }
    }
    private void FinishAttack1()
    {
        isAttacking = false;
        anim.SetBool(TagManager.ISATTACKING_ANIMATION_PARAMETER, isAttacking);
        anim.SetBool(TagManager.ATTACK1_ANIMATION_PARAMETER, false);
    }

    private void Damage(AttackDetails attackDetails)
    {
        int dir;
        //Damage Player
        FindObjectOfType<AudioManager>().Play("PlayerHurt");
        PS.DecreaseHealth(attackDetails.damageAmount);

        if(attackDetails.position.x < transform.position.x)
        {
            dir = 1;
        } else
        {
            dir = -1;
        }
        PC.Knockback(dir);
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attack1HitBoxPos.position, attack1Radius);
    }
}
