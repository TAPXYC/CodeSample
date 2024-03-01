using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FirebaseController : MonoBehaviour
{
    public AuthController Auth
    {
        get;
        private set;
    }

    public DBController DataBase
    {
        get;
        private set;
    }

    public void Init()
    {
        DontDestroyOnLoad(gameObject);
        gameObject.SetActive(true);
        Auth = new AuthController(this);
        DataBase = new DBController(this, Auth);
    }

    void OnDestroy()
    {
        if (Auth != null)
            Auth.OnDestroy();
    }
}
