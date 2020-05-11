using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ThreadedDataRequester : MonoBehaviour
{
    private static ThreadedDataRequester instance;
    Queue<ThreadInfo> dataQueue = new Queue<ThreadInfo>();

    void Awake()
    {
        instance = FindObjectOfType<ThreadedDataRequester>();
    }

    public static void RequestData(Func<object> generateData, Action<object> callback)
    {
        ThreadStart threadStart = delegate { instance.DataThread(generateData, callback); };
        
        new Thread(threadStart).Start();
    }

    /// <summary>
    /// If we call the method inside of the thread then it will be executed inside of that same thread
    /// </summary>
    /// <param name="callback">the callback</param>
    void DataThread(Func<object> generateData, Action<object> callback)
    {
        object data = generateData();
        /*
         * When one thread reaches this point
         * while it`s executing this bit of code
         * no other thread can execute it
         */
        lock (dataQueue)
        {
            dataQueue.Enqueue(new ThreadInfo(callback, data));
        }
    }

    void Update()
    {
        if (dataQueue.Count > 0)
        {
            for (int i = 0; i < dataQueue.Count; i++)
            {
                ThreadInfo threadInfo = dataQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
    }
    
    struct ThreadInfo
    {
        public readonly Action<object> callback;
        public readonly object parameter;

        public ThreadInfo(Action<object> callback, object parameter)
        {
            this.callback = callback;
            this.parameter = parameter;
        }
    }
}
