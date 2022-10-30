using UnityEngine;
using UnityEngine.SceneManagement;

public class GameInstance : MonoBehaviour
{
    public static GameInstance Instance { get; private set; }

    public CameraController playerCamera { get; private set; }

    [SerializeField] private GameObject m_menuPrefab;

    private UI_Menu m_menu;
    private bool m_initialized;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
            m_menu = Instantiate(m_menuPrefab, transform).GetComponent<UI_Menu>();
        }

        if(!m_initialized) Instance.Initialize();
    }

    public void Initialize()
    {
        playerCamera = FindObjectOfType<CameraController>();

        m_initialized = true;
    }

    public void ReloadScene()
    {
        m_initialized = false;

        SceneManager.LoadScene("MainScene");

        Resume();
    }

    public void GameOver()
    {
        Time.timeScale = 0f;
        m_menu.Open(UI_Menu.MenuContent.GameOver);
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        if (!m_menu.isOpen) m_menu.Open(UI_Menu.MenuContent.Pause);
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        if(m_menu.isOpen) m_menu.Close();
    }

    public void Quit()
    {
        Application.Quit();
    }
}
