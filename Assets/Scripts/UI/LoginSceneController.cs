using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoginSceneController : MonoBehaviour
{
    /* Instance */
    public static LoginSceneController Instance;// Static Instance

    /* Canvas */
    public Canvas LRCanvas;                     // Login and Register canvas
    public Canvas loginCanvas;                  // Login canvas
    public Canvas registerCanvas;               // Register canvas
    private int loginOrRegister;                // Canvas State

    /* Start Button */
    public GameObject startButton;              // Start Button

    /* Login Info */
    public GameObject loginAccount;             // Login account
    public GameObject loginPassword;            // Login Password
    public GameObject loginErrorText;           // Login error text

    /* Register Info */
    public GameObject registerAccount;          // Register account
    public GameObject registerPassword;         // Register Password
    public GameObject registerConfirm;          // Register Confirm Password
    public GameObject registerError1Text;       // Register error1 text
    public GameObject registerError2Text;       // Register error2 text
    public GameObject registerStateCanvas;      // Register state text

    // Start is called before the first frame update
    void Start()
    {
        /* Set Instance */
        Instance = this;
        /* Set Canvas */
        LRCanvas.enabled = false;
        loginCanvas.enabled = true;
        registerCanvas.enabled = false;
        loginOrRegister = 0;
        /* Set Error Text Invisible */
        loginErrorText.SetActive(false);
        registerError1Text.SetActive(false);
        registerError2Text.SetActive(false);
        /* Set Canvas Invisible */
        registerStateCanvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Callback function when click set login button
    /// </summary>
    public void ClickSetLoginCanvas()
    {
        if(loginOrRegister == 1)
        {
            registerCanvas.enabled = false;
            loginCanvas.enabled = true;
            loginOrRegister = 0;
        }
    }

    /// <summary>
    /// Callback function when click set register button
    /// </summary>
    public void ClickSetRegisterCanvas()
    {
        if (loginOrRegister == 0)
        {
            loginCanvas.enabled = false;
            registerCanvas.enabled = true;
            loginOrRegister = 1;
        }
    }

    /// <summary>
    /// Callback function for start button
    /// </summary>
    public void ClickStart()
    {
        Debug.Log("Press Start");
        // Hide start button
        startButton.SetActive(false);
        // show login canvas
        LRCanvas.enabled = true;
    }

    /// <summary>
    /// Callback function when click login button
    /// </summary>
    public void ClickLogin()
    {
        /* Clear Error */
        loginErrorText.SetActive(false);
        /* Get account and password */
        string account = loginAccount.GetComponent<Text>().text;
        string password = loginPassword.GetComponent<InputField>().text;
        Debug.Log("Account: " + account);
        Debug.Log("Password: " + password);
        /* Send */
        GlobalController.Instance.mainClient.SendLogin(account, password);
    }

    /// <summary>
    /// Callback function when click login button
    /// </summary>
    public void ClickRegister()
    {
        /* Clear Error */
        registerError1Text.SetActive(false);
        registerError2Text.SetActive(false);
        /* Get account and password */
        string account = registerAccount.GetComponent<Text>().text;
        string password = registerPassword.GetComponent<InputField>().text;
        string confirm = registerConfirm.GetComponent<InputField>().text;
        Debug.Log("Account: " + account);
        Debug.Log("Password: " + password);
        Debug.Log("Confirm: " + confirm);
        if(password != confirm)
        {
            registerError1Text.SetActive(true);
        }
        else
        {
            /* Send */
            GlobalController.Instance.mainClient.SendRegister(account, password);
        }
    }

    /// <summary>
    /// Enter Hall
    /// </summary>
    public void SetLoginState(int state)
    {
        if(state == MainClient.LOGIN_FAIL)
        {
            loginErrorText.SetActive(true);
        }
        else
        {
            // Record name
            GlobalController.Instance.userName = loginAccount.GetComponent<Text>().text;
            // Load Scene
            SceneManager.LoadScene("Hall");
        }
    }


    /// <summary>
    /// Set Register State
    /// </summary>
    /// <param name="state"></param>
    public void SetRegisterState(int state)
    {
        if(state == MainClient.REGISTER_FAIL)
        {
            registerError2Text.SetActive(true);
        }
        else if(state == MainClient.REGISTER_SUCCESS)
        {
            registerStateCanvas.SetActive(true);
        }
    }

    public void ClickCancelRegisterStateCanvas()
    {
        registerStateCanvas.SetActive(false);
    }

    public void ClickExitGame()
    {
        // Exit Server Connection
        GlobalController.Instance.mainClient.SendQuit();
    }


}
