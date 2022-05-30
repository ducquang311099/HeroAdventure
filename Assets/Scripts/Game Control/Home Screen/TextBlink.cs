using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextBlink : MonoBehaviour
{
    #region ===== Field ====

    [SerializeField] private float blinkFadeInTime = .5f;
    [SerializeField] private float blinkFadeOutTime = .7f;
    [SerializeField] private float blinkStayTime = .8f;

    private Text txt;
    private Color color;
    private float timeChecker = 0;

    #endregion

    #region ==== Unity LifeCircle ====

    // Start is called before the first frame update
    void Start()
    {
        txt = GetComponent<Text>();
        color = txt.color;
    }

    // Update is called once per frame
    void Update()
    {
        BlinkText();
    }

    #endregion

    #region ==== My Methods ====

    void BlinkText()
    {
        timeChecker += Time.deltaTime;
        if (timeChecker < blinkFadeInTime)
            txt.color = new Color(color.r, color.g, color.b, timeChecker / blinkFadeInTime);
        else if (timeChecker < blinkFadeInTime + blinkStayTime)
            txt.color = new Color(color.r, color.g, color.b, 1);
        else if (timeChecker < blinkFadeInTime + blinkStayTime + blinkFadeOutTime)
            txt.color = new Color(color.r, color.g, color.b, 1 - (timeChecker - (blinkFadeInTime + blinkStayTime)) / blinkFadeOutTime);
        else
            timeChecker = 0;
    }

    #endregion
}
