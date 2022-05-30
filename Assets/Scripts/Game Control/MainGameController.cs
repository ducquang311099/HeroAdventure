using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainGameController : MonoBehaviour
{
    #region ==== Fields ====
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject settingMenu;
    [SerializeField] private GameObject crossFade;
    [SerializeField] private GameObject startNoti;
    [SerializeField] private GameObject winNoti;
    [SerializeField] private GameObject loseNoti;
    [SerializeField] private GameObject gameInfo;
    [SerializeField] private Text time, enemy;
    [SerializeField] private float timeValue = 300f;
    [SerializeField] private int EnemiesAmount = 9;

    private float currentTimeValue;
    private int currentEnemies;

    #endregion
    #region ==== LifeCircle ====
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SetupScene());
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
        if (!startNoti.activeSelf)
        {
            TimeRemaining();
            EnemiesRemaining(player.GetComponent<PlayerController>().kill);
        }

    }

    #endregion
    #region ==== My Methods ====

    public void setAtiveCursor()
    {
        Cursor.visible = !Cursor.visible;
    }

    public void ReturnButtonClick()
    {
        pauseMenu.SetActive(!pauseMenu.activeSelf);
        Time.timeScale = pauseMenu.activeSelf ? 0 : 1;
        setAtiveCursor();
    }

    public void BackMenuButtonClick()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void SettingClick()
    {
        settingMenu.SetActive(true);
    }

    public void BackClick()
    {
        settingMenu.SetActive(false);
    }

    void TimeRemaining()
    {
        if (currentTimeValue > 0)
        {
            currentTimeValue -= Time.deltaTime;
            int value = (int)currentTimeValue;
            time.text = value.ToString();
        }
        else
        {
            loseNoti.SetActive(true);
            Time.timeScale = loseNoti.activeSelf ? 0 : 1;
        }
    }

    void EnemiesRemaining(int kill)
    {
        if (currentEnemies > 0)
        {
            currentEnemies = EnemiesAmount - kill;
            enemy.text = currentEnemies.ToString();
        }
        else
        {
            winNoti.SetActive(true);
            Time.timeScale = winNoti.activeSelf ? 0 : 1;
        }

    }

    IEnumerator SetupScene()
    {
        Time.timeScale = 1;
        crossFade.SetActive(true);
        pauseMenu.SetActive(false);
        Cursor.visible = true;
        gameInfo.SetActive(false);
        startNoti.SetActive(false);
        winNoti.SetActive(false);
        loseNoti.SetActive(false);
        currentTimeValue = timeValue;
        currentEnemies = EnemiesAmount;
        Destroy(crossFade, 2f);
        setAtiveCursor();
        yield return new WaitForSeconds(1f);
        startNoti.SetActive(true);
        yield return new WaitForSeconds(.5f);
        Time.timeScale = 0;
    }

    void CheckInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !startNoti.activeSelf)
            ReturnButtonClick();
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (startNoti.activeSelf)
            {
                startNoti.GetComponent<SlideAnimation>().CloseDialog();
                gameInfo.SetActive(true);
                currentTimeValue = timeValue;
                currentEnemies = EnemiesAmount;
                Time.timeScale = 1;
            }
            if (loseNoti.activeSelf || winNoti.activeSelf)
                SceneManager.LoadScene(1);                     
        }
    }

    #endregion

}
