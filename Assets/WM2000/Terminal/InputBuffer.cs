using UnityEngine;
public class InputBuffer 
{
    string currentInputLine = ""; // todo private

    public delegate void OnCommandSentHandler(string command);
    public event OnCommandSentHandler onCommandSent;
    public int offset = 0;
    string error;
    public  Hacker hacker;
    public void ReceiveFrameInput(string input)
    {
        
        foreach (char c in input)
        {
            UpdateCurrentInputLine(c);
        }
    }
    public void Write(string data)
    {
        //Debug.Log("Recieved " + data);
      //  Debug.Log("Before "+currentInputLine);
        offset = data.Length;
      //  Debug.Log(offset);
        currentInputLine += data;
        
    }
    public string GetCurrentInputLine()
    {
        return currentInputLine;
        // unless password
    }
    public void  ClearCurrentInputLine()
    {
        currentInputLine = "";
        offset = 0;
        // unless password
    }

    private void UpdateCurrentInputLine(char c)
    {
       // Debug.Log("aa"+currentInputLine);
        if (c == '\b')
        {
            DeleteCharacters();
        }
        else if (c == '\n' || c == '\r')
        {
            string s = currentInputLine.Substring(offset);
            if (hacker.CheckInput(s))
            {
                SendCommand(s);
            }
            else
            {
                error =hacker.getError();
                currentInputLine = error;
                offset = currentInputLine.Length;
            }
            
        }
        else
        {
            currentInputLine += c;
        }
        
    }

    private void DeleteCharacters()
    {
        
        if (currentInputLine.Length > offset)
        {
            //Debug.Log("here");
            currentInputLine = currentInputLine.Remove(currentInputLine.Length - 1);
        }
        else
        {
            // do nothing on delete at start of line
        }
    }

    private void SendCommand(string command)
    {
       // Debug.Log(":hehe");
        onCommandSent(command);
    } 
}
