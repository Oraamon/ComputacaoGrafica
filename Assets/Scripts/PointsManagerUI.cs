using UnityEngine;
using TMPro;

public class PointsManagerUI : MonoBehaviour
{
    [SerializeField] private Transform container; // Contêiner no Canvas onde as receitas serão exibidas
    [SerializeField] private TextMeshProUGUI pointsText; // Referência ao campo de texto para exibir a pontuação

    private int currentPoints = 0; // Pontuação atual
    private int previousPoints = -1; // Pontuação anterior usada para detectar mudanças

    private void Start()
    {
        UpdatePointsUI(); // Atualiza o UI no início
    }

    private void Update()
    {
        if (currentPoints != previousPoints)
        {
            UpdatePointsUI(); // Atualiza o UI se a pontuação mudar
            previousPoints = currentPoints; // Atualiza a pontuação anterior
        }
    }

    // Função chamada por outras classes para modificar a pontuação
    public void AddPoints(int pointsToAdd)
    {
        currentPoints += pointsToAdd; // Incrementa a pontuação
    }

    private void UpdatePointsUI()
    {
        // Atualiza o texto do UI com a pontuação atual
        pointsText.text = $"Points: {currentPoints}";
    }
}
