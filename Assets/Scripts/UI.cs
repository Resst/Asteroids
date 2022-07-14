using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    private GameManager gameManager;

    [SerializeField] private Text scoreText;
    [SerializeField] private Text livesText;
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private Text btnControlText;

    public static bool Paused { get; private set; }

    private void Awake()
    {
        gameManager = GetComponent<GameManager>();
    }

    private void Start()
    {
        SwitchPaused();
        SetupControlsBtnText();
    }

    private void Update()
    {
        scoreText.text = "Score: " + GameManager.Score;
        livesText.text = "Lives: " + GameManager.Lives;
        if (Input.GetKeyDown(KeyCode.Escape) && GameManager.GameStarted)
        {
            SwitchPaused();
        }
    }

    public void SwitchPaused()
    {
        Paused = !Paused;
        Time.timeScale = Paused ? 0 : 1;
        AudioListener.pause = Paused;
        menuPanel.SetActive(Paused);
    }

    public void Continue()
    {
        if (GameManager.GameStarted) SwitchPaused();
    }

    public void NewGame()
    {
        gameManager.Restart();
    }

    public void SwitchControls()
    {
        GameManager.MouseControlled = !GameManager.MouseControlled;
        SetupControlsBtnText();
        gameManager.SetControls(GameManager.MouseControlled);
    }

    private void SetupControlsBtnText()
    {
        btnControlText.text = GameManager.MouseControlled ? "Управление: клавиатура + мышь" : "Управление: клавиатура";
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}