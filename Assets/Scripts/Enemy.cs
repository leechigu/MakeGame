using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public int maxHealth;
    public int curHealth;
    public Transform target;
    public bool isChase;
    Rigidbody rigid;
    BoxCollider boxCollider;
    Material mat;
    Weapon equipWeapon;
    NavMeshAgent nav;
    Animator anim;
    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        mat = GetComponentInChildren<MeshRenderer>().material;
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        Invoke("ChaseStart", 2);
    }
    void FreezeVelocity()
    {
        if (isChase)
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
            
        }
    }
    void ChaseStop()
    {
        isChase = false;
        anim.SetBool("isWalk", false);
    }
    void ChaseStart()
    {
        isChase = true;
        anim.SetBool("isWalk", true);
    }
    private void Update()
    {
        
        if(isChase) 
            nav.SetDestination(target.position);
    }
    private void FixedUpdate()
    {
        FreezeVelocity();
    }
    public void HitByGrenade(Vector3 explosion)
    {

        curHealth -= 100;
        Vector3 reactVec = transform.position - explosion;
        StartCoroutine(OnDamage(reactVec,null,true));
    }
    private void OnTriggerEnter(Collider other )
    {
       if(gameObject.layer == 13)
        {
            if (other.gameObject.tag == "Melee")
            {
                Weapon weapon = other.GetComponent<Weapon>();
                curHealth -= weapon.damage;

                Vector3 reactVec = transform.position - other.transform.position;
                StartCoroutine(OnDamage(reactVec, other, false));
            }
            else if (other.gameObject.tag == "Bullet")
            {
                Bullet bullet = other.GetComponent<Bullet>();
                curHealth -= bullet.damage;

                Vector3 reactVec = transform.position - other.transform.position;
                StartCoroutine(OnDamage(reactVec, other, false));
            }
        }

    }

    IEnumerator OnDamage(Vector3 reactVec , Collider other,bool isGrenade)
    {
        ChaseStop();
        mat.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        
        
        if (curHealth > 0 )
        {
            mat.color = Color.white;
            reactVec = reactVec.normalized;

            if (isGrenade)
            {
                reactVec += Vector3.up * 20;
                rigid.freezeRotation = false;
                rigid.AddForce(reactVec * 5, ForceMode.Impulse);
                rigid.AddTorque(reactVec * 15, ForceMode.Impulse);

                yield return new WaitForSeconds(0.8f);
                
                
               

            }
            else
            {
                if (other.gameObject.tag == "Melee")
                {

                    reactVec += Vector3.up*2; 
                    rigid.AddForce(reactVec * 15, ForceMode.Impulse);
                    yield return new WaitForSeconds(0.7f);
                    
                    
                    
                }
                else if (other.gameObject.tag == "Bullet")
                {
                    rigid.AddForce(reactVec * 5, ForceMode.Impulse);
                    yield return new WaitForSeconds(0.3f);
                    
                    
                    
                }

            }
            
            
        }
        else
        {
            nav.enabled = false;
            mat.color = Color.gray;
            gameObject.layer = 14;
            rigid.AddForce(Vector3.up * 10, ForceMode.Impulse);
            anim.SetTrigger("doDie");
            Destroy(gameObject, 2);
        }
        ChaseStart();

    }
    

    

}
