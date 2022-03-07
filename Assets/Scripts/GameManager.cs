using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static int curProblem, curScore = 0, lifeRemaining = 3, totalScoring, bonusScoreFormula;
    public Problem[] problems;      // list of all problems
    public float timePerProblem;    // time allowed to answer each problem
    public float remainingTime;     // time remaining for the current problem
    public List<Image> lifeImageList; //life green images
    public Canvas canvasGone; //joystick canvas
    public Canvas canvasEnding; // canvas at the end of the game
    public Sprite lifeRed; //life red when a player get stunned
    public TMP_Text scoreText, scoreEnd, bonusScore, totalScore, times, up;

    public PlayerController player; // player object

    // instance
    public static GameManager instance;

    void Awake()
    {
        // set instance to this script.
        instance = this;
    }

    void Start()
    {
        // set the initial problem
        SetProblem(0);
        canvasEnding.enabled = false;
    }

    void Update()
    {
        remainingTime -= Time.deltaTime;

        // has the remaining time ran out?
        if (remainingTime <= 0.0f)
        {
            Lose();
        }
    }

    // called when the player enters a tube
    public void OnPlayerEnterTube(int tube)
    {
        // did they enter the correct tube?
        if (tube == problems[curProblem].correctTube)
            CorrectAnswer();
        else
            IncorrectAnswer();
    }

    // called when the player enters the correct tube
    void CorrectAnswer()
    {
        // is this the last problem?
        if (9 == curProblem)
            Win();
        else
        {
            curScore += 75; // add score
            scoreText.text = "SCORE: " + curScore;
            SetProblem(curProblem + 1); // next problem
        }
        
        scoreEnd.text = scoreText.text; // get the score at the end of the game
        bonusScoreFormula = lifeRemaining * 135;
        bonusScore.text = "BONUS: " + bonusScoreFormula;

        totalScoring = curScore + bonusScoreFormula; // formula for the total score
        totalScore.text = "TOTAL: " + totalScoring;

        //if the score and the life remaining is 0 at the end of the game
        if (curScore == 0 && lifeRemaining == 0)
        { 
            scoreEnd.text = "SCORE: 0";
            bonusScore.text = "BONUS: 0";
            totalScore.text = "TOTAL: 0";
        }
    }

    // called when the player enters the incorrect tube
    void IncorrectAnswer()
    {
        player.Stun();
    }

    public void lifeMinus()
    {
        lifeRemaining--; // decrement the life counter
        lifeImageList[lifeRemaining].sprite = lifeRed; // change the life green image into life red

        //if the player has no life left
        if(lifeRemaining == 0)
        {
            totalScore.text = "TOTAL: " + curScore;
            bonusScore.text = "BONUS: 0";
            Lose(); // call the ending canvas
        }
        else
        {
            //calculate the bonus score if the player have extra life after the game
            bonusScoreFormula = lifeRemaining * 135;
            bonusScore.text = "BONUS: " + bonusScoreFormula;

            totalScoring = curScore + bonusScoreFormula; // formula for the total score
            totalScore.text = "TOTAL: " + totalScoring;
        }

        
    }

    // sets the current problem
    void SetProblem(int problem)
    {
        curProblem = problem;
        UI.instance.SetProblemText(problems[curProblem]);
        remainingTime = timePerProblem;

        
    }

    // called when the player answers all the problems
    void Win()
    {
        Time.timeScale = 0.0f;
        canvasGone.enabled = false;
        canvasEnding.enabled = true;
    }

    // called if the remaining time on a problem reaches 0
    void Lose()
    {
        times.text = "TIMES";
        up.text = "UP!!!";
        Time.timeScale = 0.0f;
        canvasGone.enabled = false;
        canvasEnding.enabled = true;       
    }
}
