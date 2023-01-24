using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    private bool isMenuOpened = false;

    [SerializeField]
    private GameObject pauseMenu;

    [SerializeField]
    private GameObject optionsMenu;

    [SerializeField]
    private ThirdPersonOrbitCamBasic cameraScript;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            isMenuOpened = !isMenuOpened;

            pauseMenu.SetActive(isMenuOpened);
            optionsMenu.SetActive(false);

            Time.timeScale = isMenuOpened ? 0 : 1;
            cameraScript.enabled = !isMenuOpened;
        }
    }
}
