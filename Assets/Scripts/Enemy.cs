using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public GameObject prfHpBar;
    public GameObject canvas;

    RectTransform hpBar;

    public float height = 1.7f;
    public string enemyName;
    public int maxHp;
    public float nowHp;
    public int atkDmg;
    public float atkSpeed;
    public float moveSpeed;
    public float atkRange;
    public float fieldOfVision;
    Rigidbody2D rigid2D;
    Vector3 _hpBarPos;

    private void SetEnemyStatus(string _enemyName, int _maxHp, int _atkDmg, float _atkSpeed, float _moveSpeed, float _atkRange, float _fieldOfVision)
    {
        enemyName = _enemyName;
        maxHp = _maxHp;
        nowHp = _maxHp;
        atkDmg = _atkDmg;
        atkSpeed = _atkSpeed;
        moveSpeed = _moveSpeed;
        atkRange = _atkRange;
        fieldOfVision = _fieldOfVision;
    }
    // Start is called before the first frame update
    void Start()
    {
        rigid2D = GetComponent<Rigidbody2D>();
        Application.targetFrameRate = 60;
        hpBar = Instantiate(prfHpBar, canvas.transform).GetComponent<RectTransform>();
        
        if (enemyName == "Boss")
        {
            SetEnemyStatus("Boss", 200, 20, 3.0f, 3, 3.0f, 6f);
        }
        else if (enemyName == "Golem")
        {
            SetEnemyStatus("Golem", 150, 12, 1.5f, 1.5f, 5f, 6f);
        }
        else
        {
            SetEnemyStatus("Enemy", 100, 10, 1.5f, 2, 1.5f, 7f);
        }
        nowHpbar = hpBar.transform.GetChild(0).GetComponent<Image>();

        SetAttackSpeed(atkSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyName == "Boss")
        {
            atkSpeed = Random.Range(1.0f, 3.0f);
            _hpBarPos =
                Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y + height + 1.0f, 0));
        }
        else {
            _hpBarPos =
                Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y + height, 0));
        }
        hpBar.position = _hpBarPos;
        nowHpbar.fillAmount = (float)nowHp/(float)maxHp;
        if (nowHp < maxHp)
        {
            nowHp = nowHp += 0.02f;
        }
        
    }

    public Sword_Man sword_man;
    Image nowHpbar;
    public Animator enemyAnimator;
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Sword"))
        {
            if (sword_man.attacked)
            {
                if (sword_man.nowSt < 6)
                {                   
                    if (sword_man.GetComponent<Animator>().GetBool("jumping"))
                    {
                        nowHp -= sword_man.lowDmg * 2;
                    }
                    else nowHp -= sword_man.lowDmg;
                }


                else {                  
                    if (sword_man.GetComponent<Animator>().GetBool("jumping"))
                    {
                        nowHp -= sword_man.atkDmg * 2;
                    }
                    else nowHp -= sword_man.atkDmg;
                }

                Debug.Log(nowHp);
                sword_man.attacked = false;
                if (nowHp <= 0)
                {
                    Die();
                }
            }
        }
        if (col.CompareTag("fire"))
        {
            nowHp -= sword_man.atkDmg * 1.5f;
            if (nowHp <= 0)
            {
                Die();
            }
        }
    }
    void Die()
    {
        enemyAnimator.SetTrigger("die");            // die 애니메이션 실행
        GetComponent<EnemyAI>().enabled = false;    // 추적 비활성화
        GetComponent<Collider2D>().enabled = false; // 충돌체 비활성화
        Destroy(GetComponent<Rigidbody2D>());       // 중력 비활성화
        Destroy(gameObject, 3);                     // 3초후 제거
        Destroy(hpBar.gameObject, 3);               // 3초후 체력바 제거
        if (enemyName == "Boss")
        {
            GameObject Door = GameObject.Find("Door");
            Door.GetComponent<Rigidbody2D>().AddForce(Vector2.up*25.0f);
            enemyAnimator.SetTrigger("die2");
        }
        sword_man.nowMp = sword_man.maxMp;
    }

    void SetAttackSpeed(float speed)
    {
        enemyAnimator.SetFloat("attackSpeed", speed);
    }
}
