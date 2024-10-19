using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

public class Sword_Man : MonoBehaviour
{
    public GameObject objSwordMan;
    Animator animator;
    public float maxHp;
    public float nowHp;
    public float maxMp;
    public float nowMp;
    public float maxSt;
    public float nowSt;
    public float maxsk;
    public float Hpheal;
    public float Stheal;
    public int atkDmg;
    public int jc = 0;
    public float lowDmg;
    public float atkSpeed = 1;
    public bool attacked = false;
    public Image nowHpbar;
    public Image nowMpbar;
    public Image nowStbar;
    public Image skill;
    public Image heal;
    public bool inputRight = false;
    public bool inputLeft = false;
    public bool inputUp = false;
    public float jumpPower = 300;
    public float moveSpeed = 10;
    public float hrt;
    public float rt;
    public float resttime;
    public bool ss = false;
    public bool hs = false;
    public bool god = false;
    public GameObject fireball;
    GameObject fb;
    Rigidbody2D rigid2D;
    BoxCollider2D col2D;
    CapsuleCollider2D cap2D;
    ParticleSystem ps;
    GameObject gameover;
    GameObject retry;
    public bool IsSwordManDead = false;
    // Start is called before the first frame update
    void Start()
    {
        Hpheal = 1.0f;
        Stheal = 0.1f;

        Application.targetFrameRate = 60;
        maxHp = 60;
        nowHp = 60;
        maxMp = 50;
        nowMp = 50;
        maxSt = 70;
        nowSt = 70;
        atkDmg = 30;
        maxsk = 20.0f;
        objSwordMan.transform.position = new Vector3(-5, 0, 0);
        animator = GetComponent<Animator>();
        SetAttackSpeed(1.5f);
        rigid2D= GetComponent<Rigidbody2D>();
        col2D = GetComponent<BoxCollider2D>();
        ps = GetComponent<ParticleSystem>();
        StartCoroutine(CheckSwordManDeath());
        gameover = GameObject.Find("GameOver");
        retry = GameObject.Find("Retry");
        gameover.SetActive(false);
        retry.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(objSwordMan.transform.position.y <= -5.0f)
        {
            nowHp *= 0.8f;
            objSwordMan.transform.position = new Vector3(0, 0, 0);
        }
        lowDmg = atkDmg / 2;
        RaycastHit2D raycastHit = Physics2D.BoxCast(col2D.bounds.center, col2D.bounds.size, 0f, Vector2.down, 0.02f, LayerMask.GetMask("Ground"));
        //RaycastHit2D raycastHit = Physics2D.CapsuleCast(cap2D.bounds.center, cap2D.bounds.size, CapsuleDirection2D.Vertical,0.0f,Vector2.down, 0.02f, LayerMask.GetMask("Ground"));
        if (raycastHit.collider != null)
            animator.SetBool("jumping", false);
        else animator.SetBool("jumping", true);
        nowHpbar.fillAmount = (float)nowHp / (float)maxHp;
        nowMpbar.fillAmount = (float)nowMp / (float)maxMp;
        nowStbar.fillAmount = (float)nowSt / (float)maxSt;
        heal.fillAmount=(float)hrt/(float)maxsk;
        skill.fillAmount = (float)rt / (float)maxsk;
        if (Input.GetKey(KeyCode.RightArrow) && !animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            /*inputRight = true;
            objSwordMan.transform.localScale = new Vector3(-1, 1, 1);
            ps.transform.position = objSwordMan.transform.position;
            animator.SetBool("moving", true);*/
            MoveRight();
        }
        else if (Input.GetKey(KeyCode.LeftArrow) && !animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            /*inputLeft = true;
            objSwordMan.transform.localScale = new Vector3(1, 1, 1);
            animator.SetBool("moving", true);*/
            MoveLeft();
        }
        
        else animator.SetBool("moving", false);
        if (Input.GetKey(KeyCode.Space) && !animator.GetBool("jumping") && nowSt>=3)
        {
            jc += 1;
            nowSt -= 3;
            inputUp = true;
            resttime = 0;
        }
        if (Input.GetKeyUp(KeyCode.Space)) inputUp = false;
        if (inputRight) { animator.SetBool("moving", true); rigid2D.AddForce(Vector2.right * moveSpeed); }
        if (inputLeft) { animator.SetBool("moving", true); rigid2D.AddForce(Vector2.left * moveSpeed); }
        if (inputUp) { inputUp = false; rigid2D.velocity = new Vector2(rigid2D.velocity.x, jumpPower); }
        if (rigid2D.velocity.x >= 2.5f) rigid2D.velocity = new Vector2(2.5f, rigid2D.velocity.y);
        else if (rigid2D.velocity.x <= -2.5f) rigid2D.velocity = new Vector2(-2.5f, rigid2D.velocity.y);
        if (nowSt <= 0) nowSt = 0;
        //if (rigid2D.velocity.y >= 2.5f) rigid2D.velocity = new Vector2(rigid2D.velocity.x, 2.5f);
        if (rigid2D.velocity.y >= 7.5f) rigid2D.velocity = new Vector2(rigid2D.velocity.x, 7.5f);
        if(nowHp>=maxHp) nowHp= maxHp;
        if (Input.GetKeyDown(KeyCode.A) && !animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            animator.SetTrigger("attack");
            nowSt -= 6;
            resttime = 0;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("SampleScene");
        }

        if (Input.GetKeyDown(KeyCode.D) && nowMp>=10){
            ps.startColor = Color.yellow;
            ps.Play();
            rt = 20.0f;
            nowMp -= 10;
            ss = true;
        }
        if (Input.GetKeyDown(KeyCode.H) && nowMp>=10)
        {
            ps.startColor = Color.cyan;
            ps.Play();
            hrt = 20.0f;
            nowMp -= 10;
            hs = true;
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            fb=Instantiate(fireball);
            if (objSwordMan.transform.localScale == new Vector3(1, 1, 1))
            {
                fb.transform.localScale = new Vector3(-10, 10, 1);
                fb.transform.position = objSwordMan.transform.position + new Vector3(-1.5f, 0.5f, 0);
                fb.GetComponent<Rigidbody2D>().AddForce(Vector2.left * 300.0f);
            }
            else if(objSwordMan.transform.localScale == new Vector3(-1, 1, 1))
            {
                fb.transform.position = objSwordMan.transform.position + new Vector3(1.5f, 0.5f, 0);
                            fb.GetComponent<Rigidbody2D>().AddForce(Vector2.right*300.0f);
            }
            
            Invoke("fbdes", 1.2f);
        }
        if(Input.GetKeyDown(KeyCode.F)&& nowSt >= 10 && nowMp>=3)
        {
            god = true;
            nowSt -= 10;
            nowMp -= 3;
           if(transform.localScale == new Vector3(-1, 1, 1))
            {
                transform.position += new Vector3(3, 0, 0);
            }
            else
            {
                transform.position += new Vector3(-3, 0, 0);
            }
            Invoke("ungod", 1f);
        }
        if (hs && hrt > 0)
        {
            hrt -= 0.05f;
            nowHp += 0.05f;
        }
        else if (hs && hrt <= 0)
        {
            hs = false;
            ps.Stop();
        }

        if (ss && rt > 0)
        {
            atkDmg = 20;
            rt -= 0.05f;
            SetAttackSpeed(2.0f);
        }

        else if (ss && rt <= 0)
        {
            SetAttackSpeed(1.5f);
            atkDmg = 30;
            ss = false;
            ps.Stop();
        }
        if (nowSt < maxSt && !animator.GetBool("jumping") && !animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            resttime += 0.1f;
            if (resttime > 3.0f && resttime<7.0f)
            {
                nowSt += Stheal;
            }
            else if(resttime >= 7.0f)
            {
                nowSt += 2 * Stheal;
            }
        }
        if (Input.GetKeyDown(KeyCode.F1))
        {
            nowMp += 20;
        }
        if (GameObject.Find("key").GetComponent<Transform>().position.y < 3)
        {
            SceneManager.LoadScene("Ending");
        }
    }
    public void MoveLeft()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            inputLeft = true;
            objSwordMan.transform.localScale = new Vector3(1, 1, 1);
            animator.SetBool("moving", true);
            SFXManager.Instance.PlaySound(SFXManager.Instance.playerWalk);
        }
    }
    void fbdes()
    {
        Destroy(fb);
    }
    public void throwfb()
    {
        if(nowMp>=20 && !animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            nowMp -= 20;
            animator.SetTrigger("attack");
            fb = Instantiate(fireball);
            if (objSwordMan.transform.localScale == new Vector3(1, 1, 1))
            {
                fb.transform.localScale = new Vector3(-10, 10, 1);
                fb.transform.position = objSwordMan.transform.position + new Vector3(-1.5f, 0.5f, 0);
                fb.GetComponent<Rigidbody2D>().AddForce(Vector2.left * 300.0f);
            }
            else if (objSwordMan.transform.localScale == new Vector3(-1, 1, 1))
            {
                fb.transform.position = objSwordMan.transform.position + new Vector3(1.5f, 0.5f, 0);
                fb.GetComponent<Rigidbody2D>().AddForce(Vector2.right * 300.0f);
            }
            SFXManager.Instance.PlaySound(SFXManager.Instance.Fireball);
            Invoke("fbdes", 1.2f);
        }
    }
    public void MoveLeftf()
    {
        inputLeft = false;
    }
    public void MoveRight()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            inputRight = true;
            objSwordMan.transform.localScale = new Vector3(-1, 1, 1);
            ps.transform.position = objSwordMan.transform.position;
            animator.SetBool("moving", true);
            SFXManager.Instance.PlaySound(SFXManager.Instance.playerWalk);
        }
    }
    public void MoveRightf()
    {
        inputRight = false;
    }
    public void jump()
    {
        if (!animator.GetBool("jumping") && nowSt >= 3)
        {
            jc += 1;
            nowSt -= 3;
            inputUp = true;
            resttime = 0;
            SFXManager.Instance.PlaySound(SFXManager.Instance.playerJump);
        }
    }
    public void attack()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            animator.SetTrigger("attack");
            nowSt -= 6;
            resttime = 0;
            SFXManager.Instance.PlaySound(SFXManager.Instance.playerAttack);
        }
    }
    public void power()
    {
        if(nowMp >= 10)
        {
            ps.startColor = Color.yellow;
            ps.Play();
            rt = 20.0f;
            nowMp -= 10;
            ss = true;
            SFXManager.Instance.PlaySound(SFXManager.Instance.Power);
        }
    }
    public void healing()
    {
        if(nowMp >= 10)
        {
            ps.startColor = Color.cyan;
            ps.Play();
            hrt = 20.0f;
            nowMp -= 10;
            hs = true;
            SFXManager.Instance.PlaySound(SFXManager.Instance.Heal);
        }
    }
    public void teleport()
    {
        if(nowSt >= 10 && nowMp >= 3)
        {
            god = true;
            nowSt -= 10;
            nowMp -= 3;
            if (transform.localScale == new Vector3(-1, 1, 1))
            {
                transform.position += new Vector3(3, 0, 0);
            }
            else
            {
                transform.position += new Vector3(-3, 0, 0);
            }
            Invoke("ungod", 1f);
            SFXManager.Instance.PlaySound(SFXManager.Instance.Teleport);
        }
    }
    void ungod()
    {
        god = false;
    }
    private void FixedUpdate()
    {
        
    }
    void AttackTrue()
    {
        attacked = true;
    }
    void AttackFalse()
    {
        attacked = false;
    }
    void SetAttackSpeed(float speed)
    {
        animator.SetFloat("attackSpeed", speed);
        atkSpeed = speed;
    }
    IEnumerator CheckSwordManDeath()
    {
        while (true)
        {

            // 체력이 0이하일 때
            if (nowHp <= 0)
            {
                
                IsSwordManDead = true;
                animator.SetTrigger("die");
                yield return new WaitForSeconds(2);
                gameover.SetActive(true);
                retry.SetActive(true);
                SFXManager.Instance.PlaySound(SFXManager.Instance.playerDie);
                break;
            }
            yield return new WaitForEndOfFrame(); // 매 프레임의 마지막 마다 실행
        }
    }
    public void Die()
    {
        SceneManager.LoadScene("game");
    }
}
