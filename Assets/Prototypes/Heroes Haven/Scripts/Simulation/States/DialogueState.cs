using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueState : StateBase
{
    [SerializeField] TextBoxUI text;
    [SerializeField] List<string> dialogue = new List<string>();
    [SerializeField] int index;

    public DialogueState(Entity entity) : base(entity)
    {
        // text = entity.gameObject.GetComponent<TextBoxUI>();
        // if (text == null)
        // {
        //     text = entity.gameObject.GetComponentInChildren<TextBoxUI>();
        // }
    }

    public static DialogueState FromTask(Task task, Entity entity, List<string> toSay)
    {
        var dialogueState = new DialogueState(entity)
        {
            Task = task,
            dialogue = toSay
        };

        return dialogueState;
    }


    public override StateResult Update(float deltaTime)
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
