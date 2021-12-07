﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    // Start is called before the first frame update
    public enum Type {Melee, Range};
    public Type type;
    public int damage;
    public float rateForAttack;
    public int maxAmmo;
    public int curAmmo;

    public BoxCollider meleeArea;
    public TrailRenderer trailEffect;
    public Transform bulletPos;
    public GameObject bullet;
    public Transform bulletCasePos;
    public GameObject bulletCase;
    public void Use()
    {
        if(type == Type.Melee)
        {
            StopCoroutine("Swing");
            StartCoroutine("Swing");
        }
        else if (type == Type.Range && curAmmo>0)
        {
            
            curAmmo--;
            StartCoroutine("Shot");
        }
    }
    IEnumerator Swing()
    {
        //1
        yield return new WaitForSeconds(0.1f); //1초 대기
        meleeArea.enabled = true;
        trailEffect.enabled = true;

        //yield return null; //1프레임 대기
        //3
        yield return new WaitForSeconds(0.6f);
        meleeArea.enabled = false;
        yield return new WaitForSeconds(0.3f);
        trailEffect.enabled = false;
        // 코루틴 탈출
    }

    IEnumerator Shot()
    {
        //총알 발사
        GameObject instantBullet = Instantiate(bullet,bulletPos.position,bulletPos.rotation);
        Rigidbody bulletRigid = instantBullet.GetComponent<Rigidbody>();
        bulletRigid.velocity = bulletPos.forward * 70;
        yield return null;
        //탄피 배출

        GameObject instantCase = Instantiate(bulletCase, bulletCasePos.position, bulletCasePos.rotation);
        Rigidbody caseRigid = instantCase.GetComponent<Rigidbody>();
        Vector3 caseVec = bulletCasePos.forward * Random.Range(-3, -2) + Vector3.up* Random.Range(2,3);
        caseRigid.AddForce(caseVec, ForceMode.Impulse);
        caseRigid.AddTorque(Vector3.up * 30,ForceMode.Impulse);

    }

    //Use() 메인루틴 -> Swing() 서브루틴 -> Use() 메인루틴 
    //Use() 메인루틴 + Swing() 코루틴 (동시 실행)
}
