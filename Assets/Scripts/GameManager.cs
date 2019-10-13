using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //scores the player have got
    public static int score;

    [SerializeField] private Text _scoreText;
    [SerializeField] private GameObject _winImage;
    [SerializeField] private GameObject _loseImage;

    private bool _isWin;

    void Start()
    {
        //reset the score each time we reload the scene
        score = 0;
        //diable the win image 
        if (_winImage)
            _winImage.SetActive(false);
        //disable lose image 
        if (_loseImage)
            _loseImage.SetActive(false);
    }

    void Update()
    {
        
        //update the score
        _scoreText.text = score.ToString();
        //if no red, yellow, grey blocks in the scene
        //we win
        if ((GameObject.FindGameObjectsWithTag("red").Length
            + GameObject.FindGameObjectsWithTag("grey").Length
            + GameObject.FindGameObjectsWithTag("yellow").Length) == 0) _isWin = true;

        if (_isWin)
        {
            //Debug.Log("win");
            Time.timeScale = 0.00001f;
            //enable the win screen
            _winImage.SetActive(true);
        }
    }

    public void gameOver()
    {
       // Debug.Log("gameover!");
        _loseImage.SetActive(true);
        //wait for 3 seconds before reloading the scene
        StartCoroutine(delayLoadScene());

    }


    //wait for 3 seconds
     IEnumerator delayLoadScene()
    {

        yield return new WaitForSeconds(3);
        SceneManager.LoadScene("SampleScene");

    }
}
