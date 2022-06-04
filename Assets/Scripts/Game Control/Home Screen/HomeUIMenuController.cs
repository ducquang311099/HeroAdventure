using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class HomeUIMenuController : MonoBehaviour
{
    #region ==== Fields ====
    [SerializeField] private GameObject menu;
    [SerializeField] private GameObject settingScreen;
    [SerializeField] private GameObject creditsScreen;
    [SerializeField] private GameObject txt_PressEnter;
    [SerializeField] private GameObject txt_Loading;
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject loadingGroup;
    [SerializeField] private Slider progessBar;
    [SerializeField] private Animator loadingTextAnim;
    [SerializeField] private Button[] btnMenu;

    private bool isLoadComp = false;
    private float currentProgessBar;
    #endregion

    #region ==== Unity LifeCirle ====

    // Start is called before the first frame update
    void Start()
    {
        menu.SetActive(false);
        settingScreen.SetActive(false);
        creditsScreen.SetActive(false);
        loadingGroup.SetActive(false);
        progessBar.enabled = false;
        progessBar.value = 0;
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
        SetAbleBtn();
        UpdateProgessBar();
    }
    #endregion

    #region My Methods
    public void NewGameButtonClick()
    {
        menu.GetComponent<SlideAnimation>().CloseDialog();
        LoadScene();
    }

    void SetAbleBtn()
    {
        bool isActiveBtnMenu = settingScreen.activeSelf || creditsScreen.activeSelf;
        foreach (Button btn in btnMenu)
            btn.interactable = !isActiveBtnMenu;
    }

    public void SettingButtonClick()
    {
        settingScreen.SetActive(true);
    }

    public void CreditsButtonClick()
    {
        creditsScreen.SetActive(true);
    }

    public void LoadGameButtonClick()
    {
        Debug.Log("Coming Soon !!!");

        //if (!loadGameScreen.activeSelf)
        //{
        //    loadGameText.text = "Return";
        //    loadGameScreen.SetActive(true); 
        //}
        //else
        //{
        //    loadGameScreen.GetComponent<SlideAnimation>().CloseDialog();
        //    loadGameText.text = "Load Game";
        //}
    }

    public void QuitButtonClick()
    {
        Application.Quit();
    }

    void CheckInput()
    {
        if (Input.anyKey)
        {
            if (txt_PressEnter.activeSelf)
            {
                txt_PressEnter.SetActive(false);
                menu.SetActive(true);
            }
            if (isLoadComp)
                SceneManager.LoadScene(1);
        }
        if (Input.GetKeyDown(KeyCode.Escape) && creditsScreen.activeSelf)
            creditsScreen.GetComponent<SlideAnimation>().CloseDialog();
    }

    async void LoadScene()
    {
        await Task.Delay(1000);
        loadingScreen.SetActive(true);
        await Task.Delay(1500);
        loadingGroup.SetActive(true);
        var scene = SceneManager.LoadSceneAsync(1);
        scene.allowSceneActivation = false;
        do
        {
            await Task.Delay(500);
            currentProgessBar = scene.progress;
        } while (scene.progress < .9f);
        currentProgessBar = 1;       
        loadingTextAnim.SetTrigger("IsEnd");
        await Task.Delay(1000);
        isLoadComp = true;
        txt_Loading.GetComponent<Text>().text = "Press Any Key To Continues";
    }

    void UpdateProgessBar()
    {
        progessBar.value = Mathf.MoveTowards(progessBar.value, currentProgessBar, 3 * Time.deltaTime);
    }

    #endregion
}
