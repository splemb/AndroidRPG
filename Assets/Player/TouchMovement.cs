using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TouchMovement : MonoBehaviour
{
    public enum PlayState { Moving, Action, Hurt, Dead, Inventory }

    [SerializeField] public  PlayState playState;
    Rigidbody rb;
    Vector3 touchPos;
    [SerializeField] Transform model;
    [SerializeField] Animator animator;
    [SerializeField] LayerMask groundMask;
    [SerializeField] LayerMask actionMask;
    [SerializeField] LayerMask inputMask;
    [SerializeField] ItemInteraction itemInteraction;
    [SerializeField] Canvas inventoryDisplay;
    [SerializeField] Canvas HUD;
    [SerializeField] PhysicMaterial noFriction;
    float touchCounter = 0f;
    [SerializeField] float maxHealth = 30f;
    float health;
    Vector3 spawnPos;
    Quaternion spawnRot;

    [SerializeField] AudioClip[] audioClips;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioSource footsteps;

    [SerializeField] Slider healthbar;

    // Start is called before the first frame update
    void Start()
    {
        spawnPos = transform.position;
        spawnRot = transform.rotation;
        rb = GetComponent<Rigidbody>();
        health = maxHealth;
        if (healthbar != null) healthbar.maxValue = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {

        switch (playState)
        {
            case PlayState.Moving:
                Movement();
                break;
            case PlayState.Dead:
                CheckRespawn();
                break;
        }

        if (healthbar != null) healthbar.value = health;

        if (!CheckGrounded())
        {
            rb.AddForce(-Vector3.up, ForceMode.Impulse);
            GetComponent<CapsuleCollider>().material = noFriction;
        }
        else GetComponent<CapsuleCollider>().material = null;

        Animate();
#if UNITY_STANDALONE_WIN
        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
#endif
    }

#if UNITY_STANDALONE_WIN
    void Movement()
    {
        if (Input.GetMouseButtonDown(0))
        {
            touchCounter = 0f;
        }

        if (Input.GetMouseButton(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                touchCounter += Time.deltaTime;

                RaycastHit hit;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, inputMask))
                {

                    touchPos = hit.point;
                    Vector3 playerPos = transform.position;
                    Vector3 toPoint = (touchPos - playerPos);


                    if (toPoint.magnitude > 0.01f && CheckGrounded())
                    {
                        rb.AddForce(toPoint.normalized * Mathf.Clamp(toPoint.magnitude, 0, 2f) * Time.deltaTime * 60f * 100f);
                        Vector3 lookDir = touchPos - transform.position;
                        lookDir.y = 0; // keep only the horizontal direction
                        model.rotation = Quaternion.LookRotation(lookDir);
                        //model.rotation = Quaternion.FromToRotation(Vector3.forward, toPoint);
                    }
                }

            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (touchCounter < 0.2f)
            {
                if (CheckGrounded()) Action();
            }
        }        
    }
#elif UNITY_ANDROID
    void Movement()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (Input.GetTouch(0).phase != TouchPhase.Ended)
            {

                if (!EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                {
                    if (touch.phase == TouchPhase.Began) touchCounter = 0f;

                    touchCounter += Time.deltaTime;

                    /*

                    touchPos = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, Camera.main.transform.position.y));

                    touchPos = new Vector3(touchPos.x, transform.position.y, touchPos.z);
                    //Debug.Log(touch.position + " || " + touchPos + " || " + CheckGrounded());

                    */

                    RaycastHit hit;
                    if (Physics.Raycast(Camera.main.ScreenPointToRay(touch.position), out hit, 100, inputMask))
                    {

                        touchPos = hit.point;
                        Vector3 playerPos = transform.position;
                        Vector3 toPoint = (touchPos - playerPos);


                        if (toPoint.magnitude > 0.01f && CheckGrounded())
                        {
                            rb.AddForce(toPoint.normalized * Mathf.Clamp(toPoint.magnitude, 0, 2f) * Time.deltaTime * 60f * 100f);
                            Vector3 lookDir = touchPos - transform.position;
                            lookDir.y = 0; // keep only the horizontal direction
                            model.rotation = Quaternion.LookRotation(lookDir);
                            //model.rotation = Quaternion.FromToRotation(Vector3.forward, toPoint);
                        }
                    }
                }
            }

            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                if (touchCounter < 0.2f)
                {
                    if (CheckGrounded()) Action();
                }
            }

        }


    }
#endif

    bool CheckGrounded(float reach = 0.1f)
    {
        //return if the player is on the ground
        //reach variable (default 0.001) is how far the boxcast goes

        RaycastHit hit;
        if (Physics.BoxCast(transform.position + (transform.up * 0.5f * transform.localScale.y), new Vector3(0.1f, 0.1f, 0.1f), -transform.up, out hit, Quaternion.identity, (0.5f + reach) * transform.localScale.y, groundMask))
        {
            Quaternion q = Quaternion.FromToRotation(transform.up, Vector3.up) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime);
            return true;
        }

        else return false;
    }

    void Animate()
    {
        bool isGrounded = CheckGrounded();
        float groundSpeed = new Vector3(rb.velocity.x, 0f, rb.velocity.z).magnitude;
        animator.SetFloat("groundSpeed", groundSpeed);
        animator.SetBool("isGrounded", isGrounded);

        if (isGrounded && groundSpeed > 0.2f && !footsteps.isPlaying) footsteps.Play();
        if (!isGrounded || groundSpeed < 0.2f) footsteps.Stop();
    }

    void Action()
    {
        if (playState != PlayState.Moving) return;
        

        RaycastHit[] hits = Physics.BoxCastAll(transform.position + model.transform.forward + Vector3.up, new Vector3(1,0.5f,0.5f), model.transform.forward, model.transform.rotation, 2f, actionMask);

        if (hits.Length != 0)
        {
            bool playedSwing = false;
            foreach (RaycastHit hit in hits)
            {
                //Debug.Log(hit.transform.tag);
                if (itemInteraction.backpack.weapon.itemObject != null)
                {
                    switch (hit.transform.tag)
                    {
                        case "Enemy":
                            animator.SetTrigger("Attack");
                            if (!playedSwing) audioSource.PlayOneShot(audioClips[0]);
                            playedSwing = true;
                            playState = PlayState.Action;
                            StartCoroutine(StateTimer(.5f, PlayState.Moving));
                            hit.transform.GetComponent<EnemyBehaviour>().Damage(itemInteraction.backpack.weapon.itemObject.atkBonus);
                            Projectile(itemInteraction.backpack.weapon.itemObject);
                            break;
                        case "Interactor":
                            animator.SetTrigger("Interact");
                            playedSwing = true;
                            playState = PlayState.Action;
                            StartCoroutine(StateTimer(.5f, PlayState.Moving));
                            hit.transform.GetComponent<Interactor>().Interact();
                            break;
                        default:
                            animator.SetTrigger("Attack");
                            if (!playedSwing) audioSource.PlayOneShot(audioClips[0]);
                            playedSwing = true;
                            playState = PlayState.Action;
                            StartCoroutine(StateTimer(.5f, PlayState.Moving));
                            break;
                    }
                }
                else
                {
                    switch (hit.transform.tag)
                    {
                        case "Interactor":
                            animator.SetTrigger("Interact");
                            hit.transform.GetComponent<Interactor>().Interact();
                            playState = PlayState.Action;
                            StartCoroutine(StateTimer(.5f, PlayState.Moving));
                            Projectile(itemInteraction.backpack.weapon.itemObject);
                            break;

                    }
                }
            }
        }
        else
        {
            if (itemInteraction.backpack.weapon.itemObject != null)
            {
                animator.SetTrigger("Attack");
                audioSource.PlayOneShot(audioClips[0]);
                playState = PlayState.Action;
                StartCoroutine(StateTimer(.5f, PlayState.Moving));
                Projectile(itemInteraction.backpack.weapon.itemObject);
            }
        }
    }

    void Projectile(ItemObject item)
    {
        if (item == null) return;
        if (item.projectilePrefab == null) return;
        
        if (item.ammo != null)
        {
            bool hasAmmo = false;
            foreach (ItemSlot slot in itemInteraction.backpack.itemSlots)
            {
                if (slot.itemObject == item.ammo) { 
                    hasAmmo = true;
                    if (slot.RemoveFromStack(1) <= 0) itemInteraction.backpack.itemSlots.Remove(slot);
                    break;
                }
            }

            if (!hasAmmo) return;
        }

        audioSource.PlayOneShot(audioClips[2]);
        Instantiate(item.projectilePrefab, transform.position + Vector3.up, model.rotation);

    }

    public void Hurt(float dmg)
    {
        if (playState == PlayState.Hurt || playState == PlayState.Dead) return;
        audioSource.PlayOneShot(audioClips[1]);
        playState = PlayState.Hurt;
        if (itemInteraction.backpack.armour.itemObject != null) health -= Mathf.Clamp(dmg - itemInteraction.backpack.armour.itemObject.defBonus, 0.1f, maxHealth);
        else health -= dmg;

        if (health > 0)
        {
            animator.SetTrigger("Hurt");
            StartCoroutine(StateTimer(.5f, PlayState.Moving));
        }
        else
        {
            playState = PlayState.Dead;
            animator.SetTrigger("Death");
            //itemInteraction.backpack.DropItem(itemInteraction.backpack.itemSlots[Random.Range(0, itemInteraction.backpack.itemSlots.Count)]);
        }
    }

    public void Heal(float amt)
    {
        audioSource.PlayOneShot(audioClips[3]);
        health += amt;
        if (health > maxHealth) health = maxHealth;
    }
#if UNITY_STANDALONE_WIN
    void CheckRespawn()
    {
        if (Input.GetMouseButtonDown(0))
        {
            playState = PlayState.Moving;
            animator.Play("Grounded");
            health = maxHealth;
            transform.position = spawnPos;
            transform.rotation = spawnRot;
        }
    }
#elif UNITY_ANDROID
    void CheckRespawn()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            playState = PlayState.Moving;
            animator.Play("Grounded");
            health = maxHealth;
            transform.position = spawnPos;
            transform.rotation = spawnRot;
        }
    }
#endif
    IEnumerator StateTimer(float waitTime, PlayState newState)
    {
        yield return new WaitForSeconds(waitTime);
        playState = newState;
    }

    public void OpenInventory()
    {
        playState = PlayState.Inventory;
        inventoryDisplay.gameObject.SetActive(true);
        HUD.gameObject.SetActive(false);
        touchCounter = 1f;
    }

    public void CloseInventory()
    {
        playState = PlayState.Moving;
        inventoryDisplay.gameObject.SetActive(false);
        HUD.gameObject.SetActive(true);
        touchCounter = 1f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "DeathPlane")
        {
            Hurt(9999f);
        }
    }
}
