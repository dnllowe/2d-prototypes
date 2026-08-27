using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueState : StateBase
{
    [SerializeField] TextBoxUI text;
    [SerializeField] List<string> dialogue = new List<string>();
    [SerializeField] int index;

    public DialogueState(World world, uint entityId, Task task) : base(world, entityId, task) {}

    public override StateResult Tick(float deltaTime)
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (index >= dialogue.Count - 1)
            {
                return StateResult.Complete;
            }
            else NextDialogue();
        }
        return StateResult.Running;
    }

    override public void Enter()
    {
        index = 0;
        text.SetText(dialogue[index]);
        text.Show();
    }

    public override void Exit()
    {
        base.Exit();
        CloseDialogue();
    }

    public void NextDialogue()
    {
        index++;
        if (index >= dialogue.Count) return;
        text.SetText(dialogue[index]);
    }

    public void CloseDialogue()
    {
        text.Hide();
    }

    public void SetDialogue(List<string> dialogue)
    {
        this.dialogue = dialogue;
    }
}
