using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

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

    // Start is called before the first frame update
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
        elapsedTime += Time.deltaTime;

        if (direction.x == 0)
        {
            Animator.SetBool("IsRunning", false);
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
            Animator.SetBool("IsRunning", true);
        }
    }

    void Jump()
    {
        if (transform.position.y <= -1.40f)
        {
            Animator.StopPlayback();
            Animator.SetTrigger("Jump");
            GetComponent<Rigidbody>().AddForce(Vector2.up * 7f, ForceMode.Impulse);
        }
    }

    void Attack()
    {
        if (elapsedTime >= attackCooldown)
        {
            Animator.StopPlayback();
            Animator.SetTrigger("Attack");
            elapsedTime = 0;
            StartCoroutine(AttackCoroutine());
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"{collision.transform.tag == "Enemy"} && {isAttacking}");
        if (collision.transform.tag == "Enemy" && isAttacking)
            collision.gameObject.GetComponent<enemyComponent>().GetHit();
    }

    public void GetHit()
    {
        Animator.SetTrigger("GetHit");
        health -= 5;
    }

    IEnumerator AttackCoroutine()
    {
        isAttacking = true;

        yield return new WaitForSeconds(2);

        isAttacking = false;
    }
}
