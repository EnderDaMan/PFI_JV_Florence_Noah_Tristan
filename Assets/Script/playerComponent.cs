using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Photon.Pun;

public class playerMoveComponent : MonoBehaviour
{
    [SerializeField] InputAction MoveAction;
    [SerializeField] InputAction JumpAction;
    [SerializeField] InputAction AttackAction;
    [SerializeField] float speed = 5;
    [SerializeField] float health = 100;
    Animator Animator;

    public Vector2 direction = Vector2.zero;

    public bool isOnEdgeLeft = false;
    public bool isOnEdgeRight = false;
    public bool isAttacking = false;
    public bool enemyAttacking = false;

    float attackCooldown = 1f;
    float elapsedTime;

    private string currentState;
    
    PhotonView view;

    // Start is called before the first frame update
    private void Start()
    {
        view = GetComponent<PhotonView>();
    }
    void Awake()
    {
        Animator = GetComponent<Animator>();
        elapsedTime = Time.deltaTime;
    }

    private void OnEnable()
    {
        MoveAction.Enable();
        JumpAction.Enable();
        AttackAction.Enable();
        MoveAction.performed += (InputAction.CallbackContext context) => direction = context.ReadValue<Vector2>();
        MoveAction.canceled += _ => direction = Vector2.zero;
        JumpAction.performed += _ => Jump();
        AttackAction.performed += _ => Attack();
        

    }

    private void OnDisable()
    {
        MoveAction.Disable();
        JumpAction.Disable();
        AttackAction.Disable();
        MoveAction.performed -= (InputAction.CallbackContext context) => direction = context.ReadValue<Vector2>();
        MoveAction.canceled -= _ => direction = Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if (view.IsMine)
        {
            Move();
        }
        
    }

    void Jump()
    {
        if (view.IsMine)
        {
            if (transform.position.y <= -1.40f)
            {
                //Animator.StopPlayback();
                ChangeAnimationState("jump");
                GetComponent<Rigidbody>().AddForce(Vector2.up * 8f, ForceMode.Impulse);
            }
        }
    }

    void Attack()
    {
        if (view.IsMine)
        {
            if (elapsedTime >= attackCooldown)
            {
                Animator.StopPlayback();
                ChangeAnimationState("Attack", true);
                elapsedTime = 0;
                StartCoroutine(AttackCoroutine());
            }
        }
        
    }


    // private void OnCollisionEnter(Collision collision)
    // {
    //     if(view.IsMine)
    //     {
    //         if (collision.transform.tag == "Enemy" && isAttacking)
    //             collision.gameObject.GetComponent<enemyComponent>().GetHit();
    //     }
    // }

    public void GetHit()
    {
        if (view.IsMine)
        {
            ChangeAnimationState("Hurt", true);
            //Animator.SetTrigger("GetHit");
            health -= 5;
        }
        
    }

    IEnumerator AttackCoroutine()
    {
        if (view.IsMine)
        {
            isAttacking = true;

            yield return new WaitForSeconds(1f);

            isAttacking = false;
        }
    }

    public void Move()
    {
        elapsedTime += Time.deltaTime;

        if (direction.x == 0)
        {
            //Animator.SetBool("IsRunning", false);
           ChangeAnimationState("Idle");
               
            isOnEdgeLeft = false;
            isOnEdgeRight = false;
        }
        else
        {
            if (direction.x == 1 && transform.position.x >= 8.10f)
                isOnEdgeRight = true;

            if (direction.x == -1 && transform.position.x <= -8.10f)
                isOnEdgeLeft = true;

            if (direction.x == 1 && transform.position.x <= 8.10f)
            {
                transform.Translate(Time.deltaTime * speed * Vector2.right);
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            else if (direction.x == -1 && transform.position.x >= -8.10f)
            {
                transform.Translate(Time.deltaTime * speed * Vector2.right);
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }
            ChangeAnimationState("Run");
        }
    }

    private bool stateOverride = false;
    private void ChangeAnimationState(string state, bool statePriority = false)
    {
        if (state == currentState || stateOverride) return;
        
        Animator.Play(state);

        currentState = state;

        stateOverride = statePriority;
        
        if (statePriority)
            StartCoroutine(WaitStateChange("Idle", .8f));

    }

    private IEnumerator WaitStateChange(string nextState, float waitTime)
    {
        string originalState = currentState;
        
        yield return new WaitForSeconds(waitTime);

        stateOverride = false;
        
        if (currentState == originalState)
            ChangeAnimationState(nextState);
    }
}
