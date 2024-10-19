using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform target;
    float attackDelay;
    float distance;
    float attackTime;
    Enemy enemy;
    Animator enemyAnimator;
    void Start()
    {
        attackTime = 10f;
        Application.targetFrameRate = 60;
        enemy = GetComponent<Enemy>();
        enemyAnimator = enemy.enemyAnimator;
    }

    void Update()
    {
        attackDelay -= Time.deltaTime;
        if (attackDelay < 0) attackDelay = 0;

        distance = Vector3.Distance(transform.position, target.position);

        if (attackDelay == 0 && distance <= enemy.fieldOfVision && !target.GetComponent<Sword_Man>().IsSwordManDead)
        {
            if (enemy.enemyName == "Boss")
            {
                GameObject Trap = GameObject.Find("Trap");
                Trap.GetComponent<Rigidbody2D>().gravityScale = 1.0f;
            }
            FaceTarget();

            if (distance <= enemy.atkRange)
            {
                AttackTarget();
            }
            else
            {
                if (!enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
                {
                    MoveToTarget();
                }
            }
        }
        else
        {
            enemyAnimator.SetBool("moving", false);
        }
    }

    void MoveToTarget()
    {
        float dir = target.position.x - transform.position.x;
        dir = (dir < 0) ? -1 : 1;
        transform.Translate(new Vector2(dir, 0) * enemy.moveSpeed * Time.deltaTime);
        enemyAnimator.SetBool("moving", true);
    }

    void FaceTarget()
    {
        if (target.position.x - transform.position.x < 0) // 타겟이 왼쪽에 있을 때
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else // 타겟이 오른쪽에 있을 때
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    void AttackTarget()
    {
        enemyAnimator.SetTrigger("attack"); // 공격 애니메이션 실행
        Invoke("Hit", 0.5f);
        attackDelay = enemy.atkSpeed; // 딜레이 충전
    }
    void Hit()
    {
        if (!target.GetComponent<Sword_Man>().god)
        {
            if (distance <= enemy.atkRange)
                target.GetComponent<Sword_Man>().nowHp -= enemy.atkDmg;
        }
    }
}