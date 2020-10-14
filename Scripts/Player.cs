using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public float speed;
    public GameObject[] weapons;
    public bool[] hasWeapons;

    public int coin;
    public int life;
    public int maxCoin;
    public int maxLife;

    public AudioClip coinSound;
    public AudioClip collisionSound;

    public GameObject life1;
    public GameObject life2;
    public GameObject life3;

    public GameObject gameClear;
    public GameObject gameOver;

    public GameManager manager;

    float hAxis;
    float vAxis;

    bool wDown;
    bool jDown;
    bool dDown;
    bool iDown;
    bool sDown1;
   
    bool isJump;
    bool isDodge;
    bool isCrouch;

    Vector3 moveVec;
    Vector3 dodgeVec;

    Rigidbody rigid;
    Animator anim;

    GameObject nearObject;
    GameObject equipWeapon;
    int equipWeaponIndex = -1;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        GetInput();
        Move();
        Turn();
        Jump();
        Dodge();
        Interation();
        Swap();
    }

    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        wDown = Input.GetButton("Walk");
        jDown = Input.GetButtonDown("Jump");
        dDown = Input.GetButton("Dodge");
        iDown = Input.GetButtonDown("Interation");
        sDown1 = Input.GetButtonDown("Swap1");
    }

    void Move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        if(isDodge)
        {
            moveVec = dodgeVec;
        }

        transform.position += moveVec * speed * (wDown ? 0.6f : 1f) * Time.deltaTime;

        anim.SetBool("isRun", moveVec != Vector3.zero);
        anim.SetBool("isWalk", wDown);
        anim.SetBool("isDodge", dDown);
    }

    void Turn()
    {
        transform.LookAt(transform.position + moveVec);
    }

    void Jump()
    {
        if (jDown && !isJump && !isDodge)
        {
            rigid.AddForce(Vector3.up * 20, ForceMode.Impulse);
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
            isJump = true;
        }
    }

    void Crouch()
    {
        if (dDown && moveVec != Vector3.zero && !isJump && !isDodge)
        {
            dodgeVec = moveVec;
            speed *= 2;
            anim.SetTrigger("doCrouch");
            isDodge = true;

            Invoke("CouchOut", 0.5f);
        }
    }

    void Dodge()
    {
        if (dDown && moveVec != Vector3.zero && !isJump && !isDodge)
        {
            dodgeVec = moveVec;
            speed *= 2;
            anim.SetTrigger("doDodge");
            isDodge = true;

            Invoke("DodgeOut", 0.5f);
        }
    }

    void DodgeOut()
    {
        speed *= 0.5f;
        isDodge = false;
    }

    void Swap()
    {
        if (sDown1 && (!hasWeapons[0] || equipWeaponIndex == 0))
            return;

        int weaponIndex = -1;
        if (sDown1) weaponIndex = 0;

        if (sDown1)
        {
            if(equipWeapon != null)
                equipWeapon.SetActive(false);

            equipWeaponIndex = weaponIndex;
            equipWeapon = weapons[weaponIndex];
            equipWeapon.SetActive(true);
        }
    }

    void Interation()
    {
        if(iDown && nearObject != null)
        {
            if(nearObject.tag == "Weapon")
            {
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.value;
                hasWeapons[weaponIndex] = true;

                nearObject.gameObject.SetActive(false);
                Destroy(nearObject);
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Floor")
        {
            anim.SetBool("isJump", false);
            isJump = false;
        }
        if (collision.gameObject.tag == "Cube")
        {
            anim.SetBool("isJump", false);
            isJump = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Item")
        {
            Item item = other.GetComponent<Item>();
            switch (item.type)
            {
                case Item.Type.Coin:
                    coin += item.value;
                    AudioSource.PlayClipAtPoint(coinSound, transform.position);
                    if (coin == maxCoin)
                    {
                        gameClear.SetActive(true);
                        SceneManager.LoadScene("SampleScene");
                    }
                    break;
                case Item.Type.Heart:
                    life += item.value;
                    if (life > maxLife)
                        life = maxLife;
                    break;
            }
            other.gameObject.SetActive(false);
            Destroy(other.gameObject);
        }

        if(other.tag == "Obstacle")
        {
            switch (life)
            {
                case 1:
                    life = 0;
                    life3.SetActive(false);
                    AudioSource.PlayClipAtPoint(collisionSound, transform.position);
                    gameOver.SetActive(true);
                    SceneManager.LoadScene("SampleScene");
                    break;
                case 2:
                    life = 1;
                    life2.SetActive(false);
                    AudioSource.PlayClipAtPoint(collisionSound, transform.position);
                    break;
                case 3:
                    life = 2;
                    life1.SetActive(false);
                    AudioSource.PlayClipAtPoint(collisionSound, transform.position);
                    break;
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Weapon")
            nearObject = other.gameObject;

        Debug.Log(nearObject.name);
    }


    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon") { }
            nearObject = null;
    }

}
