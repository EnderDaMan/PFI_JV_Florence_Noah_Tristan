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
    [SerializeField] public float health = 100;
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
        JumpAction.performed -= _ => Jump();
        AttackAction.performed -= _ => Attack();
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            view.RPC("TriggerAnim", RpcTarget.All, "Death");
            currentState = "Death";
            Animator.Play("Death");
        }
        else if (view.IsMine)
        {
            Move();
        }
        
    }

    void Jump()
    {
        if (view.IsMine && health > 0)
        {
            Vector3 rayOrigin = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);

            bool worked = false;
            RaycastHit hit;
            if (Physics.Raycast(rayOrigin, Vector3.down, out hit, 1f))
            {
                worked = true;
                
            }

            if (worked || transform.position.y <= -1.40f)
            {
                //Animator.StopPlayback();
                ChangeAnimationState("jump");
                GetComponent<Rigidbody>().AddForce(Vector2.up * 7f, ForceMode.Impulse);
                view.RPC("TriggerAnim", RpcTarget.All, "jump");
            }
        }
    }

    void Attack()
    {
        if (view.IsMine && health > 0)
        {
            if (elapsedTime >= attackCooldown)
            {
                Animator.StopPlayback();
                ChangeAnimationState("Attack", true);
             view.RPC("TriggerAnim", RpcTarget.All, "Attack");
                elapsedTime = 0;
                StartCoroutine(AttackCoroutine());
            }
        }
        
    }


    public void GetHit()
    {
        if (view.IsMine)
        {
            ChangeAnimationState("Hurt", true);
            view.RPC("TriggerAnim", RpcTarget.All, "Hurt");
            health -= 5;
        }
        
    }
    

    IEnumerator AttackCoroutine()
    {
        if (view.IsMine)
        {
            isAttacking = true;

            yield return new WaitForSeconds(.8f);

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
               
             view.RPC("TriggerAnim", RpcTarget.All, "Idle");
            isOnEdgeLeft = false;
            isOnEdgeRight = false;
        }
        else
        {
            if (direction.x == 1 && transform.position.x >= 8.10f)
                isOnEdgeRight = true;

            if (direction.x == -1 && transform.position.x <= -8.10f)
                isOnEdgeLeft = true;

            float moveSpeed = speed;
            
            if (direction.x == 1 && transform.position.x <= 8.10f)
            {
                if (transform.rotation != Quaternion.Euler(0, 0, 0))
                {
                    moveSpeed /= 5;
                }
                transform.Translate(Time.deltaTime * moveSpeed * Vector2.right);
                transform.rotation = Quaternion.Euler(0, 0, 0);
                
            }
            else if (direction.x == -1 && transform.position.x >= -8.10f)
            {
                if (transform.rotation != Quaternion.Euler(0, 180, 0))
                {
                    moveSpeed /= 5;
                }
                transform.Translate(Time.deltaTime * moveSpeed * Vector2.right);
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }
            ChangeAnimationState("Run");
            view.RPC("TriggerAnim", RpcTarget.All, "Run");
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
        {
            ChangeAnimationState(nextState);
             view.RPC("TriggerAnim", RpcTarget.All, "Idle");
        }
    }
    
    [PunRPC]
    private void TriggerJump()
    {
        Animator.SetTrigger("Jump");
    }
    [PunRPC]
    private void SetIsRunning(bool isRunning) 
    {
        Animator.SetBool("IsRunning", isRunning);
    }
    [PunRPC]
    private void TriggerAttack()
    {
        Animator.SetTrigger("Attack");
    }
    [PunRPC]
    private void TriggerGetHit()
    {
        Animator.SetTrigger("GetHit");
    }

    [PunRPC]
    private void TriggerAnim(string Anim) 
    {
        if (Anim == currentState || stateOverride) return;

        Animator.Play(Anim);

        if (Anim == "Attack")
        {
            StartCoroutine(WaitStateChange("Idle", .8f));
        }
    }
}
