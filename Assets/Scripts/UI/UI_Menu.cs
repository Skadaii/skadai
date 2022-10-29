using UnityEngine;

public class UI_Menu : MonoBehaviour
{
    #region Internal Classes

    public enum MenuContent
    {
        GameOver,
        Pause
    }

    #endregion


    #region Variable

    [SerializeField] private GameObject m_pauseMenu;
    [SerializeField] private GameObject m_gameOverMenu;

    private Animator m_animator;


    #endregion


    #region Properties

    public bool isOpen { get; private set; }

    #endregion


    #region MonoBehaviour

    private void Awake()
    {
        m_animator = GetComponent<Animator>();
    }

    #endregion


    #region Functions

    public void Open(MenuContent content)
    {
        m_animator.SetTrigger("Open");

        switch(content)
        {
            case MenuContent.GameOver:
                m_gameOverMenu.SetActive(true);
                m_pauseMenu.SetActive(false);
                break;

            default:
                m_gameOverMenu.SetActive(false);
                m_pauseMenu.SetActive(true);
                break;
        }

        isOpen = true;
    }

    public void Close()
    {
        m_animator.SetTrigger("Close");

        isOpen = false;
    }

    public void Pause()
    {
        m_animator.SetTrigger("Close");

        GameInstance.Instance.Resume();
    }

    public void Resume()
    {
        m_animator.SetTrigger("Close");

        GameInstance.Instance.Resume();
    }

    public void Restart()
    {
        GameInstance.Instance.ReloadScene();
    }

    public void Quit()
    {
        GameInstance.Instance.Quit();
    }

    #endregion
}
