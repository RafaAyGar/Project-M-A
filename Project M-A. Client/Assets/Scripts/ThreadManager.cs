using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreadManager : MonoBehaviour
{
    private static List<Action> actionsToExecuteOnMainThread = new List<Action>();
    private static List<Action> copiedActionsToExecuteOnMainThread = new List<Action>();
    private static bool anyActionToExecute = false;

    private void FixedUpdate()
    {
        UpdateMain();
    }

    public static void ExecuteOnMainThread(Action action)
    {
        if (action == null)
        {
            Debug.Log("Action to execute is null");
            return;
        }

        lock (actionsToExecuteOnMainThread)
        {
            actionsToExecuteOnMainThread.Add(action);
            anyActionToExecute = true;
        }
    }

    private void UpdateMain()
    {
        if (anyActionToExecute == false) return;

        copiedActionsToExecuteOnMainThread.Clear();
        lock (actionsToExecuteOnMainThread)
        {
            copiedActionsToExecuteOnMainThread.AddRange(actionsToExecuteOnMainThread);
            actionsToExecuteOnMainThread.Clear();
            anyActionToExecute = false;
        }

        for (int i = 0; i < copiedActionsToExecuteOnMainThread.Count; i++)
        {
            copiedActionsToExecuteOnMainThread[i]?.Invoke();
        }
    }
}
