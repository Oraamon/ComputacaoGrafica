using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 7f;
    private MeshRenderer meshRenderer;
    private Vector3 moveDir = Vector3.zero;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    private void Update()
    {
        HandleMovement();
    }

    // Função para movimentação do personagem
    private void HandleMovement()
    {
        Vector2 inputVector = new Vector2(0, 0);

        if (Input.GetKey(KeyCode.W))
        {
            inputVector.y = -1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            inputVector.y = +1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            inputVector.x = +1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            inputVector.x = -1;
        }

        inputVector = inputVector.normalized;
        moveDir = new Vector3(inputVector.x, 0f, inputVector.y);
        transform.position += moveDir * moveSpeed * Time.deltaTime;

        if (animator != null)
        {
            if (moveDir != Vector3.zero)
            {
                animator.SetBool("IsWalking", true); 
            }
            else
            {
                animator.SetBool("IsWalking", false); 
            }
        }

        // Rotate the player to face the movement direction
        if (moveDir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * moveSpeed);
        }
    }
}
