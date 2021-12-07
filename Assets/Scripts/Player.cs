using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    public GameObject[] weapons;
    public bool[] hasWeapons;
    public GameObject[] grenades;
    public int hasGrenades;
    public GameObject grenadeObj;
    public Camera followCamera;

    public int ammo;
    public int coin;
    public int health;
    

    public int maxAmmo;
    public int maxCoin;
    public int maxHealth;
    public int maxHasGrenades;

    float haxis;
    float vaxis;


    bool rDown;
    Vector3 moveVec;
    Vector3 dodgeVec;
    Animator anim;
    Transform tr;
    bool jDown;
    Rigidbody rigid;
    public int JumpPower;
    bool isJumping;
    bool isDodge;
    bool isSwap;
    bool isReload;
    bool iDown;
    bool isFireReady =true;
    bool isBorder;

    bool sDown1;
    bool sDown2;
    bool sDown3;
    bool fDown;
    bool reDown;
    bool gDown;

    Weapon equipWeapon;
    int equipWeaponIndex;
    float fireDelay;


    GameObject nearObject;
    // Start is called before the first frame update
    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        tr = GetComponent<Transform>();
        rigid = GetComponent<Rigidbody>();
        equipWeaponIndex = -1;
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        Move();
        Turn();
        Jump();
       
        Reload();
        Attack();
        Grenade();
        Dodge();
        Interation();
        Swap();
    }
    void GetInput()
    {
        haxis = Input.GetAxisRaw("Horizontal");
        vaxis = Input.GetAxisRaw("Vertical");
        rDown = Input.GetButton("Run");
        jDown = Input.GetButtonDown("Jump");
        fDown = Input.GetButton("Fire1");
        gDown = Input.GetButtonDown("Fire2");
        reDown = Input.GetButtonDown("Reload");
        iDown = Input.GetButtonDown("Interaction");
        sDown1 = Input.GetButtonDown("Swap1"); 
        sDown2 = Input.GetButtonDown("Swap2"); 
        sDown3 = Input.GetButtonDown("Swap3");
    }

    void Move()
    {
        moveVec = new Vector3(haxis, 0, vaxis).normalized;
        if (isDodge)
            moveVec = dodgeVec;
        if (isSwap || isReload || !isFireReady)
            moveVec = Vector3.zero;
        if(!isBorder)//normalized는 Vector3의 값이 항상 1이 되게끔 해주는 함수임. 대각선 이동 시에도 일정하게 하기 위함
            transform.position += moveVec * speed * (rDown ? 1.2f : 0.7f) * Time.deltaTime;
        //Time.deltaTime 은 모든 프레임에 동일하게 하기 위한 코드!

        anim.SetBool("isWalk", moveVec != Vector3.zero);
        anim.SetBool("isRun", rDown);
    }

    void Turn()
    {
        //키보드에 의한 회전
        transform.LookAt(transform.position + moveVec);
        //마우스에 의한 회전

        Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);

        RaycastHit rayHit;
        if (fDown)
        {
            if (Physics.Raycast(ray, out rayHit, 100))
            {
                Vector3 nextVec = rayHit.point - transform.position;
                nextVec.y = 0;
                transform.LookAt(transform.position + nextVec);
            }

        }
    }
    void Jump()
    {
        if (jDown&& moveVec==Vector3.zero&&!isJumping&&!isDodge)
        {
            rigid.AddForce(Vector3.up * JumpPower, ForceMode.Impulse);
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
            isJumping = true;
        }

    }
    void Grenade()
    {
        if (hasGrenades == 0)
            return;

        if(gDown && !isReload && !isSwap)
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            if(Physics.Raycast(ray, out rayHit, 100))
            {
                Vector3 nextVec = rayHit.point - transform.position;
                nextVec.y = 0;
                GameObject instantGrenade = Instantiate(grenadeObj, transform.position, transform.rotation);

                Rigidbody rigidGrenade = instantGrenade.GetComponent<Rigidbody>();
                rigidGrenade.AddForce(nextVec, ForceMode.Impulse);
                rigidGrenade.AddTorque(Vector3.back * 10, ForceMode.Impulse);

                hasGrenades--;
                grenades[hasGrenades].SetActive(false);
            }
        }
    }
    void Attack()
    {
        if (equipWeapon == null)
            return;

        fireDelay += Time.deltaTime;
        isFireReady = equipWeapon.rateForAttack < fireDelay;

        if(fDown && isFireReady && !isDodge && !isSwap && equipWeapon.type ==Weapon.Type.Melee)
        {
            equipWeapon.Use();
            anim.SetTrigger("doSwing");
            fireDelay = 0;
        }
        else if(fDown && isFireReady && !isDodge && !isSwap && equipWeapon.type == Weapon.Type.Range)
        {
            if(equipWeapon.curAmmo == 0&&!isJumping)
            {
                
                return;
            }
            else if (equipWeapon.curAmmo >= 1)
            {
                equipWeapon.Use();
                anim.SetTrigger("doShot");
                fireDelay = 0;
            }
        }
    }
    void Reload()
    {
        if (equipWeapon == null)
            return;
        if (equipWeapon.type == Weapon.Type.Melee)
            return;
        if (ammo == 0)
            return;
        if(reDown && !isJumping && !isDodge && !isSwap && isFireReady)
        {
            anim.SetTrigger("doReload");
            isReload = true;

            Invoke("ReloadOut", 1.2f);
        }
    }
    void ReloadOut()
    {
        int reAmmo = ammo < equipWeapon.maxAmmo ? ammo : equipWeapon.maxAmmo;
        equipWeapon.curAmmo = reAmmo;
        ammo -= reAmmo;
        isReload = false;

    }
    void Dodge()
    {
        if(jDown && moveVec != Vector3.zero && !isJumping&&!isDodge)
        {
            dodgeVec = moveVec;
            speed *=2 ;
            anim.SetTrigger("doDodge");
            isDodge = true;
            Invoke("DodgeOut", 0.7f);

        }
    }
    void DodgeOut()
    {
        speed *= 0.5f;
        isDodge = false;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag =="Floor")
        {
            anim.SetBool("isJump", false);
            isJumping = false;
        }
    }
    void Interation()
    {
        if (iDown && nearObject!=null&&!isJumping&&!isDodge)
        {
            if(nearObject.tag == "Weapon")
            {
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.value;
                hasWeapons[weaponIndex] = true;
                Destroy(nearObject);
            }
        }
    }
    void Swap()
    {
        if(sDown1 && equipWeaponIndex==0)
            return;
        if (sDown2 && equipWeaponIndex == 1)
            return;
        if (sDown3 && equipWeaponIndex == 2)
            return;


        int weaponIndex = -1;
        if (sDown1) 
            weaponIndex = 0;
        if (sDown2) 
            weaponIndex = 1;
        if (sDown3)
            weaponIndex = 2;
        if ((sDown1 || sDown2 || sDown3)&&!isJumping &&!isDodge&&hasWeapons[weaponIndex])
        {
            if(equipWeapon!=null)
                equipWeapon.gameObject.SetActive(false);
            equipWeaponIndex = weaponIndex;
            equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
            equipWeapon.gameObject.SetActive(true);
            anim.SetTrigger("doSwap");
            isSwap = true;
            Invoke("SwapOut", 0.3f);
        }
    }
    void SwapOut()
    {
        isSwap = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Item")
        {
            Item item = other.GetComponent<Item>();
            switch (item.type)
            {
                case Item.Type.Ammo:
                    ammo += item.value;
                    if (ammo > maxAmmo)
                        ammo = maxAmmo;
                    break;
                case Item.Type.Coin:
                    coin += item.value;
                    if (coin> maxCoin)
                        coin = maxCoin;
                    break;
                case Item.Type.Heart:
                    health += item.value;
                    if (health > maxHealth)
                        health = maxHealth;
                    break;
                case Item.Type.Grenade:
                    grenades[hasGrenades].SetActive(true);
                    hasGrenades += item.value;
                    if (hasGrenades > maxHasGrenades)
                        hasGrenades = maxHasGrenades;
                    break;


            }
            Destroy(other.gameObject);
        }    
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Weapon")
        {
            nearObject = other.gameObject;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon")
        {
            nearObject = null;
        }
    }

    void StopToWall()
    {
        //Debug.DrawRay(transform.position, transform.forward * 3, Color.green); // 정면으로 ray발사
        isBorder = Physics.Raycast(transform.position, transform.forward, 3,LayerMask.GetMask("Wall") );

    }
    void FreezeRotation()
    {
        rigid.angularVelocity = Vector3.zero;
    }
    void FixedUpdate()
    {
        FreezeRotation();
        StopToWall();
    }

}