/*
 * You can derive from this class to make an object that:
 * 1. will keep only one instance
 * 2. you can access through ClassName.Instance
 * 3. will persist via DontDestroyOnLoad
 * 4. will destroy self on awake if there is already an instance (see 1)
 *
 * Derive like this:

public class YourClassName : SingletonMonoBehaviour<YourClassName>

 * Note: when defining Awake in a derived class, override like this:
    
    protected override void Awake()
    {
        base.Awake();
        //additional code
    }
 
 */

using System;
using UnityEngine;

public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<T>();
                if (_instance == null)
                {
                    _instance = new GameObject( typeof(T) + " (singleton)").AddComponent<T>();
                }
            }

            return _instance;
        }
    }

    /// <summary>
    /// If there is already an instance when this one is attempting
    /// to Awake, destroy self!
    /// Keep this object persistent with DontDestroyOnLoad
    /// </summary>
    /*
     * Note: when defining Awake in a derived class, override like this:
    
        protected override void Awake()
        {
            base.Awake();
            //additional code
        }
         
     */
    protected virtual void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            //gets added to don't destroy on load group and will
            //persist when new scenes are loaded
            DontDestroyOnLoad(gameObject);
        }
    }
}