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

    float attackCooldown = 1.5f;
    float elapsedTime;

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
                Animator.StopPlayback();
                view.RPC("TriggerJump", RpcTarget.All);
                
                GetComponent<Rigidbody>().AddForce(Vector2.up * 7f, ForceMode.Impulse);
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
                view.RPC("TriggerAttack", RpcTarget.All);
                elapsedTime = 0;
                StartCoroutine(AttackCoroutine());
            }
        }
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(view.IsMine)
        {
            if (collision.transform.tag == "Enemy" && isAttacking)
                collision.gameObject.GetComponent<enemyComponent>().GetHit();
        }
    }

    public void GetHit()
    {
        if (view.IsMine)
        {
            view.RPC("TriggerGetHit", RpcTarget.All);
            health -= 5; 
        }
        
    }

    IEnumerator AttackCoroutine()
    {
        if (view.IsMine)
        {
            isAttacking = true;

            yield return new WaitForSeconds(2);

            isAttacking = false;
        }
    }

    public void Move()
    {
        elapsedTime += Time.deltaTime;

        if (direction.x == 0)
        {
            view.RPC("SetIsRunning", RpcTarget.All, false);
            isOnEdgeLeft = false;
            isOnEdgeRight = false;
        }
        else
        {
            if (direction.x == 1 && transform.position.x >= 4.10f)
                isOnEdgeRight = true;

            if (direction.x == -1 && transform.position.x <= -4.10f)
                isOnEdgeLeft = true;

            if (direction.x == 1 && transform.position.x <= 4.10f)
            {
                transform.Translate(Time.deltaTime * speed * Vector2.right);
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            else if (direction.x == -1 && transform.position.x >= -4.10f)
            {
                transform.Translate(Time.deltaTime * speed * Vector2.right);
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }
            view.RPC("SetIsRunning", RpcTarget.All, true);
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
}
