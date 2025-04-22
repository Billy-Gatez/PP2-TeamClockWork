//Jeremy Cahill - Full Sail University - Portfolio 2 - Game Dev - Rod Moye
using Random = UnityEngine.Random;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PlayerController : MonoBehaviour, IDamage, IPickup
{
    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] CharacterController controller;

    [SerializeField] float origSpeed;
    [SerializeField] float speed;
    [SerializeField] int sprintMod;
    [SerializeField] int jumpSpeed;
    [SerializeField] int jumpMax;
    [SerializeField] int gravity;

    [SerializeField] List<gunStats> gunList = new List<gunStats>();
    [SerializeField] GameObject gunModel;
    [SerializeField] int shootDamage;
    [SerializeField] int shootDist;
    [SerializeField] float shootRate;

    [SerializeField] float crouchHeight = 0.5f; 
    [SerializeField] float normalHeight = 1.0f; 
    [SerializeField] float crouchSpeed = 2.0f; 
    bool isCrouching = false;


    int jumpCount;
    public int HPOrig;
    public int HP;
    public int XP;
    int gunListPos;

    float shootTimer;

    Vector3 moveDir;
    Vector3 playerVel;

    bool isSprinting;


    void Start()
    {
        origSpeed = speed;
        HPOrig = HP;
        controller.height = normalHeight;
        updatePlayerUI();
        
    }


    void Update()
    {
     Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);
            
      movement();

        sprint();

        crouch();

        selectGun();

    }

    void movement()
    {
        if (controller.isGrounded)
        {
            jumpCount = 0;
            playerVel = Vector3.zero;
        }

        // Use Input.GetAxis for both keyboard and controller
        moveDir = (Input.GetAxis("Horizontal") * transform.right) +
                   (Input.GetAxis("Vertical") * transform.forward);

        controller.Move(moveDir * speed * Time.deltaTime);

        jump();

        playerVel.y -= gravity * Time.deltaTime;
        controller.Move(playerVel * Time.deltaTime);

        shootTimer += Time.deltaTime;

        // Use Input.GetButton for controller input
        if (Input.GetButton("Fire1") && shootTimer >= shootRate)
        {
            shoot();
        }
    }

    void jump()
    {
        // Use Input.GetButtonDown for controller input
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            jumpCount++;
            playerVel.y = jumpSpeed;
        }
    }

    void sprint()
    {
        // Use Input.GetButtonDown and Input.GetButtonUp for controller input
        if (Input.GetButtonDown("Sprint"))
        {
            speed *= sprintMod;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            speed /= sprintMod;
        }
    }

    void crouch()
    {
        // Use Input.GetButtonDown and Input.GetButtonUp for controller input
        if (Input.GetButtonDown("Crouch") && !isCrouching)
        {
            isCrouching = true;
            controller.height = crouchHeight;
        }
        else if (Input.GetButtonUp("Crouch") && isCrouching)
        {
            isCrouching = false;
            controller.height = normalHeight;
        }
    }

    void shoot()
    {
        shootTimer = 0;
        if(gunList.Count != 0)
        {
            if (gunList[gunListPos].projectileAmount == 1)
            {
                RaycastHit hit;
                if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist, ~ignoreLayer))
                {
                    Debug.Log(hit.collider.name);

                    IDamage dmg = hit.collider.GetComponent<IDamage>();

                    if (dmg != null)
                    {
                        dmg.takeDamage(shootDamage);
                    }
                }
            }else for (int i = 0; i <= gunList[gunListPos].projectileAmount; ++i)
                {
                    shotgunRay();
                }
        }
    }

    void shotgunRay()
    {
        //initial setup
        Vector3 dir = Camera.main.transform.forward;
        Vector3 spread = Vector3.zero;
        //establishing shotgun spread, this will make a square shape with the spread
        spread += Camera.main.transform.right * (Random.Range(-1f, 1f) * Random.Range(-1f, 1f));
        spread += Camera.main.transform.up * (Random.Range(-1f, 1f) * Random.Range(-1f, 1f));
        //Make spread circular, also rerandomize so that pellets aren't limited to 1 unit radius
        dir += spread.normalized * Random.Range(0f, .5f);
        //actually fire the bullet & debug to show hits
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, dir, out hit, shootDist, ~ignoreLayer))
        {
            Debug.Log(hit.collider.name);
            Debug.DrawLine(Camera.main.transform.position, hit.point, Color.green, 1f);

            IDamage dmg = hit.collider.GetComponent<IDamage>();

            if (dmg != null)
            {
                dmg.takeDamage(shootDamage);
            }
        }else { Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward + dir * shootDist, Color.red, 1f); }
    }

    public void takeDamage(int amount)
    {
        HP -= amount;
        updatePlayerUI();
        StartCoroutine(flashDamageScreen());

        if (HP <= 0)
        {
            
            gamemanager.instance.youLose();
        }
    }

    public void getGunStats(gunStats gun)
    {
        gunList.Add(gun);
        gunListPos = gunList.Count - 1;

        changeGun();
    }

    void selectGun()
    {
       if (!gamemanager.instance.isPaused)
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                if (gunListPos < gunList.Count - 1) { gunListPos++; }
                else { gunListPos = 0; }

                changeGun();
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                if (gunListPos > 0) { gunListPos--; }
                else { gunListPos = gunList.Count - 1; }
                changeGun();
            }
        }
    }

    void changeGun()
    {
        shootDamage = gunList[gunListPos].shootDmg;
        shootDist = gunList[gunListPos].shootDist;
        shootRate = gunList[gunListPos].shootRate;
        speed = origSpeed * gunList[gunListPos].weaponSpeedMod;

        gunModel.GetComponent<MeshFilter>().sharedMesh = gunList[gunListPos].model.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunList[gunListPos].model.GetComponent<MeshRenderer>().sharedMaterial;
    }
    public void updatePlayerUI()
    {
        gamemanager.instance.playerHPBar.fillAmount = (float)HP / HPOrig;
    }

    IEnumerator flashDamageScreen()
    {
        gamemanager.instance.playerDamageScreen.SetActive(true);
        yield return new WaitForSeconds(0.05f);
        gamemanager.instance.playerDamageScreen.SetActive(false);
    }


}
