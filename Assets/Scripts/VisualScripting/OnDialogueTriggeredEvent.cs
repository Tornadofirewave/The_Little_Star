using Unity.VisualScripting;
using UnityEngine;

[UnitTitle("On Dialogue Triggered")]
[UnitCategory("Events/Dialogue")]
public class OnDialogueTriggeredEvent : EventUnit<EmptyEventArgs>
{
    protected override bool register => true;

    public override EventHook GetHook(GraphReference reference)
    {
        return new EventHook("OnDialogueTriggered");
    }

    public static void Trigger(GameObject target)
    {
        EventBus.Trigger("OnDialogueTriggered", new EmptyEventArgs());
    }
}
