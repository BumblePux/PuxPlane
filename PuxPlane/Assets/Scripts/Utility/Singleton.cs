using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    [Header("Singleton")]
    [SerializeField, Tooltip("Does this singleton object survive scene changes?")]
    protected bool isPersistent = false;

    // Properties
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                var instance = FindObjectOfType<T>();
                if (instance)
                {
                    (instance as Singleton<T>).Awake();
                }
                else
                {
                    Debug.LogError($"Singleton<{typeof(T).Name}>: Object not found.");
                }
            }

            return _instance;
        }
    }

    public static bool Exists => _instance != null;

    private void Awake()
    {
        // Destroy this instance if one already exists.
        if (_instance != null && _instance != this as T)
        {
            Destroy(gameObject);
            return;
        }

        // No instance exists! This is the new singleton.
        _instance = this as T;

        // Run derived logic.
        OnSingletonAwake();

        // Should we survive scene changes?
        if (isPersistent)
        {
            DontDestroyOnLoad(gameObject);
        }        
    }

    protected virtual void OnSingletonAwake() { }

    private void OnDestroy()
    {
        // If destroying cached instance, clear reference and run derived logic.
        if (_instance != null)
        {
            _instance = null;
            OnSingletonDestroy();
        }
    }

    protected virtual void OnSingletonDestroy() { }
}
