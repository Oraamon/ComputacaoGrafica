using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 7f;
    private MeshRenderer meshRenderer;
    private bool isFalling = false; // Checa se o boneco está caído
    public float standUpSpeed = 1f; // Velocidade de "levantar"
    private Vector3 moveDir = Vector3.zero; // Guarda a direção de movimento

    // Start is called before the first frame update
    void Start()
    {
        // Get the MeshRenderer component to control visibility
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.enabled = false; // Make the capsule invisible
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (!isFalling) // Permite movimento apenas quando não está caído
        {
            HandleMovement();
        }
        CheckIfFallen(); // Verifica se caiu e ativa animação para levantar
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

        // Rotate the player to face the movement direction
        if (moveDir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * moveSpeed);
        }
    }

    // Verifica se o personagem caiu
    private void CheckIfFallen()
    {
        if (!isFalling && Vector3.Angle(transform.up, Vector3.up) > 70f) // ângulo para detectar queda
        {
            isFalling = true;
            StartCoroutine(StandUp());
        }
    }

    // Corrutina para "levantar" o personagem
    private IEnumerator StandUp()
    {
        yield return new WaitForSeconds(1f); // Espera um pouco antes de levantar

        Quaternion uprightRotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        float elapsedTime = 0;
        Vector3 originalPosition = transform.position; // Posição original antes de levantar

        while (elapsedTime < standUpSpeed)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, uprightRotation, elapsedTime / standUpSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Garante que termine na posição e rotação corretas
        transform.rotation = uprightRotation;
        transform.position = originalPosition; // Retorna à posição original no chão

        moveDir = Vector3.zero; // Zera a direção de movimento ao final da animação
        isFalling = false; // Reseta o estado para permitir movimento
    }

}
