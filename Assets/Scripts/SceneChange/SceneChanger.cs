using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public string sceneName;
    public Animator fadeAnim;
    public float fadeTime = .5f;
    public Vector2 newPlayerPosition;
    private Transform player;

    public bool requireConfirmation = false;

    public CanvasGroup confirmCanvasGroup;
    public GameObject confirmationUI;
    private bool playerInTrigger = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player = collision.transform;
            if (requireConfirmation)
            {
                confirmationUI.SetActive(true);
                confirmCanvasGroup.alpha = 1;
                playerInTrigger = true;
                Time.timeScale = 0f; // Pause the game
            }
            else
            {
                fadeAnim.Play("FadeToBlack");
                StartCoroutine(DelayFade());
            }
        }
    }

    public void ChangeScene()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        fadeAnim.Play("FadeToBlack");
        StartCoroutine(DelayFade());
    }


    public void OnYesButton()
    {
        if (playerInTrigger)
        {
            confirmationUI.SetActive(false);
            Time.timeScale = 1f; // Resume the game
            fadeAnim.Play("FadeToBlack");
            StartCoroutine(DelayFade());
        }
    }

    public void OnNoButton()
    {
        confirmationUI.SetActive(false);
        playerInTrigger = false;
        Time.timeScale = 1f; // Resume the game
    }

    IEnumerator DelayFade()
    {
        yield return new WaitForSeconds(fadeTime);
        player.position = newPlayerPosition;
        SceneManager.LoadScene(sceneName);
    }


    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (player != null)
            player.position = newPlayerPosition;
    }

}
