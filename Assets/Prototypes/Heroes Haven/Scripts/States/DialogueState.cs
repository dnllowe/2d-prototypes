using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueState : StateBase
{
    [SerializeField] TextBoxUI text;
    [SerializeField] List<string> dialogue = new List<string>();
    [SerializeField] int index;

    public DialogueState(Entity entity) : base(entity)
    {
        text = entity.gameObject.GetComponent<TextBoxUI>();
        if (text == null)
        {
            text = entity.gameObject.GetComponentInChildren<TextBoxUI>();
        }
    }

    public static DialogueState FromTask(Task task, Entity entity, List<string> toSay)
    {
        var dialogueState = new DialogueState(entity);
        dialogueState.Task = task;
        dialogueState.dialogue = toSay;

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

    [Button]
    override public void Enter()
    {
        index = 0;
        text.SetText(dialogue[index]);
        text.Show();
    }

    [Button]
    public override void Exit()
    {
        base.Exit();
        CloseDialogue();
    }

    [Button]
    public void NextDialogue()
    {
        index++;
        if (index >= dialogue.Count) return;
        text.SetText(dialogue[index]);
    }

    [Button]
    public void CloseDialogue()
    {
        text.Hide();
    }

    public void SetDialogue(List<string> dialogue)
    {
        this.dialogue = dialogue;
    }
}
