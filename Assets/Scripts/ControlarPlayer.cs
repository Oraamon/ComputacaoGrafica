using UnityEngine;

public class ControlarPlayer : MonoBehaviour
{

    public float speed = 5f; // Velocidade de movimento do jogador

    private Rigidbody rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        // Cria um vetor de movimento baseado nos inputs e multiplica pela velocidade
        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical) * speed;

        // Aplica o movimento ao Rigidbody
        rb.MovePosition(rb.position + movement * Time.fixedDeltaTime);
    }
}
