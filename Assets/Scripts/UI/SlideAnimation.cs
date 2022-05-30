using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideAnimation : MonoBehaviour
{
    [SerializeField] private float timeSlide = .5f;
    [SerializeField] private float destinationPosX = -600f;
    [SerializeField] private float destinationPosY = -80f;
    [SerializeField] private bool isSlideX = true;
    private void OnEnable()
    {
        if (isSlideX)
        {
            gameObject.transform.localPosition = new Vector2(-Screen.width, destinationPosY);
            gameObject.transform.LeanMoveLocalX(destinationPosX, timeSlide).setEaseOutExpo().delay = 0f;
        }
        else
        {
            gameObject.transform.localPosition = new Vector2(destinationPosX, -Screen.height);
            gameObject.transform.LeanMoveLocalY(destinationPosY, timeSlide).setEaseOutExpo().delay = .1f;
        }
    }

    public void CloseDialog()
    {
        if (isSlideX)
            gameObject.transform.LeanMoveLocalX(-Screen.width, timeSlide).setEaseInExpo().setOnComplete(OnComplete);
        else
            gameObject.transform.LeanMoveLocalY(-Screen.height, timeSlide).setEaseInExpo().setOnComplete(OnComplete);
    }

    void OnComplete()
    {
        gameObject.SetActive(false);
    }

}
