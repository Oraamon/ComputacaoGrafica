using UnityEngine;
using TMPro; // Necessário se usar TextMeshPro
using UnityEngine.SceneManagement;


public class GameTimer : MonoBehaviour
{
    [SerializeField] private float timeRemaining = 60f; // Tempo inicial em segundos
    [SerializeField] private TextMeshProUGUI timerText; // Referência ao texto no Canvas

    private bool isGameOver = false;

    private void Update()
    {
        if (!isGameOver && timeRemaining > 1)
        {
            timeRemaining -= Time.deltaTime; // Reduz o tempo restante
            UpdateTimerUI(); // Atualiza o texto do timer
        }
        else if (!isGameOver)
        {
            EndGame(); // Finaliza o jogo quando o tempo acaba
        }
    }

    private void UpdateTimerUI()
    {
        // Formata o tempo restante como minutos e segundos
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    private void EndGame()
    {
        isGameOver = true;
        Debug.Log("Game Over!");
        SceneManager.LoadScene("MenuScene");
        // Aqui você pode adicionar lógica para encerrar o jogo ou mostrar uma tela de "Game Over".
    }
}
