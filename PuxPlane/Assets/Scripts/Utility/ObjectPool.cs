using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : MonoBehaviour
{
    // Pool
    private T[] _pooledObjects;
    private int _defaultSize;

    // Actions
    private Func<T> OnCreate;
    private Action<T> OnGet;
    private Action<T> OnRelease;
    private Action<T> OnDestroy;

    // Constructor
    public ObjectPool(Func<T> createObject, Action<T> onGet, Action<T> onRelease, Action<T> onDestroy, int defaultSize = 10)
    {
        // Assign actions
        OnCreate = createObject;
        OnGet = onGet;
        OnRelease = onRelease;
        OnDestroy = onDestroy;

        // Setup pool
        _defaultSize = defaultSize;
        _pooledObjects = new T[_defaultSize];

        // Create pool
        for (int i = 0; i < _pooledObjects.Length; i++)
        {
            _pooledObjects[i] = OnCreate();
            _pooledObjects[i].gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Clears the current pool. Calls OnDestroy for each pooled object and initializes a new pool.
    /// </summary>
    public void Clear()
    {
        for (int i = _pooledObjects.Length; i >= 0; i--)
        {
            OnDestroy?.Invoke(_pooledObjects[i]);
        }

        _pooledObjects = new T[_defaultSize];
    }

    /// <summary>
    /// Returns an inactive object from the pool. If no object is available a new object is created, added to the pool, and returned.
    /// OnGet is called last for the returned object.
    /// </summary>
    /// <returns>T</returns>
    public T Get()
    {
        T objectFromPool = default;

        // Find an inactive object in pool
        foreach (var element in _pooledObjects)
        {
            if (element != null && !element.gameObject.activeInHierarchy)
            {
                objectFromPool = element;
                break;
            }
        }

        // Did we find an available object from the pool?
        if (objectFromPool == null)
        {
            // No! Create a new one and add to pool
            Array.Resize(ref _pooledObjects, _pooledObjects.Length + 1);
            int lastIndex = _pooledObjects.Length - 1;
            objectFromPool = _pooledObjects[lastIndex] = OnCreate();
        }

        OnGet?.Invoke(objectFromPool);
        return objectFromPool;
    }

    /// <summary>
    /// Calls OnRelease on the object.
    /// </summary>
    /// <param name="element"></param>
    public void Release(T element)
    {
        OnRelease?.Invoke(element);
    }

    /// <summary>
    /// Returns an array of all active objects in the pool.
    /// </summary>
    /// <returns></returns>
    public T[] GetAllActive()
    {
        List<T> activeObjects = new List<T>();

        foreach (var element in _pooledObjects)
        {
            if (element != null && element.gameObject.activeInHierarchy)
            {
                activeObjects.Add(element);
            }
        }

        return activeObjects.ToArray();
    }

    /// <summary>
    /// Returns an array of all inactive objects from the pool.
    /// </summary>
    /// <returns></returns>
    public T[] GetAllInactive()
    {
        List<T> inactiveObjects = new List<T>();

        foreach (var element in _pooledObjects)
        {
            if (element != null && !element.gameObject.activeInHierarchy)
            {
                inactiveObjects.Add(element);
            }
        }

        return inactiveObjects.ToArray();
    }
}
