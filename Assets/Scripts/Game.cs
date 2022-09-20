using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.Rendering;

public class Game : MonoBehaviour
{
    public Text text;
    void TryUndo()
    {
        if (Storage.UndoList.Count <= 0) return;
        Storage.UndoList.Last.Value.Undo();
        Storage.RedoStack.Push(Storage.UndoList.Last.Value);
        Storage.UndoList.RemoveLast();
    }

    void TryRedo()
    {
        if (Storage.RedoStack.Count <= 0) return;
        var command = Storage.RedoStack.Pop();
        command.Execute();
        Storage.UndoList.AddLast(command);
    }

    void GameOver(string winner)
    {
        text.text = $"{winner} defeated all enemies and won the game!";
        Invoke(nameof(RestartGame),3);
    }


    private void RestartGame()
    {
        Storage.UndoList.Clear();
        Storage.RedoStack.Clear();
        SceneManager.LoadScene(0);
    }

    void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            TryUndo();
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            TryRedo();
        }
    }
}


public class Storage
{
    public static Stack<Command> RedoStack = new Stack<Command>();
    public static LinkedList<Command> UndoList = new LinkedList<Command>();
}
