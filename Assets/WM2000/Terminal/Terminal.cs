using UnityEngine;
using System.Reflection;

public class Terminal : MonoBehaviour
{
    public DisplayBuffer displayBuffer;
    public InputBuffer inputBuffer;
    [SerializeField] Hacker hacker;
    static Terminal primaryTerminal;

    private void Awake()
    {
        if (primaryTerminal == null) { primaryTerminal = this; } // Be the one
        inputBuffer = new InputBuffer();
        displayBuffer = new DisplayBuffer(inputBuffer);
        inputBuffer.onCommandSent += NotifyCommandHandlers;
        inputBuffer.hacker = hacker;
    }

    public string GetDisplayBuffer(int width, int height)
    {
        return displayBuffer.GetDisplayBuffer(Time.time, width, height);
    }

    public void ReceiveFrameInput(string input)
    {
        
       // if (input != "")
         //   Debug.Log(input);
        inputBuffer.ReceiveFrameInput(input);
    }

    public static void ClearScreen()
    {
        ClearCurrentInputLine();
        primaryTerminal.displayBuffer.Clear();
    }

    public static void WriteLine(string line)
    {
        primaryTerminal.displayBuffer.WriteLine(line);
    }
    public static void Write(string line)
    {
        primaryTerminal.displayBuffer.Write(line);
    }

    public void NotifyCommandHandlers(string input)
    {
        var allGameObjects = FindObjectsOfType<MonoBehaviour>();
        foreach (MonoBehaviour mb in allGameObjects)
        {
            
            var flags = BindingFlags.NonPublic | BindingFlags.Instance;
            var targetMethod = mb.GetType().GetMethod("OnUserInput", flags);
            if (targetMethod != null)
            {
                Debug.Log(mb);
                object[] parameters = new object[1];
                parameters[0] = input;
                targetMethod.Invoke(mb, parameters);
            }
        }
    }
    public static void ClearCurrentInputLine()
    {
        primaryTerminal.displayBuffer.inputBuffer.ClearCurrentInputLine();
    }
    public static void DeleteLastLine()
    {
       
        primaryTerminal.displayBuffer.logLines.RemoveAt(primaryTerminal.displayBuffer.logLines.Count - 1);
    }

    public static void SetOffset(int i)
    {
        primaryTerminal.inputBuffer.offset = i;
    }
    
}