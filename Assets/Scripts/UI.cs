using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public TMP_Text problemText; // problem text in the ship
    public TMP_Text[] answersTexts;             // array of the 4 answers text
    public Image remainingTimeDial;         // remaining time image with radial fill
    private float remainingTimeDialRate;    // 1.0 / time per problem

    // instance
    public static UI instance;

    void Awake()
    {
        // set instance to be this script
        instance = this;
    }

    void Start()
    {
        // set the remaining time dial rate
        // used to convert the time per problem (8 secs for example)
        // and converts that to be used on a 0.0 - 1.0 scale
        remainingTimeDialRate = 1.0f / GameManager.instance.timePerProblem;
    }

    void Update()
    {
        // update the remaining time dial fill amount
        remainingTimeDial.fillAmount = remainingTimeDialRate * GameManager.instance.remainingTime;
    }

    // sets the ship UI to display the new problem
    public void SetProblemText(Problem problem)
    {
        string operatorText = "";

        // convert the problem operator from an enum to an actual text symbol
        switch (problem.operation)
        {
            case MathsOperation.Addition: operatorText = " + "; break;
            case MathsOperation.Subtraction: operatorText = " - "; break;
            case MathsOperation.Multiplication: operatorText = " x "; break;
            case MathsOperation.Division: operatorText = " ÷ "; break;
        }

        // set the problem text to display the problem
         problemText.text = problem.firstNumber + operatorText + problem.secondNumber;

        // set the answers texts to display the correct and incorrect answers
        for (int index = 0; index < answersTexts.Length; ++index)
        {
            answersTexts[index].text = problem.answers[index].ToString();
        }
    }  
}