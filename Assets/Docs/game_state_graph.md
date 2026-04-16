# Game State Graph
## Visual Scripting State Graph Guide

---

### Game States
| Value | State |
|---|---|
| Exploring | Default — player has full control |
| Dialogue | Player frozen — dialogue panel active |

---

## State Machine Setup
- Component: `State Machine` on the Player GameObject
- Graph type: State Graph (not Script Graph)
- Start state: `Exploring`

---

## Variables (Object Scope — on Player Variables component)

| Name | Type | Default |
|---|---|---|
| InDialogue | Boolean | false |

---

## How to Read This Guide

Each section below is one **state** or **transition** in the State Graph. States contain flow nodes wired from `On Enter State`. Transitions connect states and are listed under **"Transitions."** Value wires stay within their state unless noted.

---

## State: EXPLORING
*Entry state. Re-enables the movement graph and clears the dialogue flag.*

| Node | Settings |
|---|---|
| `On Enter State` | *(event — flow starts here)* |
| `This` | *(self reference)* |
| `Get Component` | ScriptMachine |
| `Set Enabled` | *(Behaviour: Set Enabled)* target: `Get Component (ScriptMachine)` result, enabled: `Literal Boolean true` |
| `Set Variable` | *(Object scope)* name: `InDialogue`, type: Boolean, input: `Literal Boolean false` |

**Internal flow:**
`On Enter State` → `Set Enabled` assign → `Set Variable` assign

---

## State: DIALOGUE
*Disables movement graph, zeroes velocity, and sets the dialogue flag.*

| Node | Settings |
|---|---|
| `On Enter State` | *(event — flow starts here)* |
| `This` | *(self reference — used twice)* |
| `Get Component` | ScriptMachine |
| `Set Enabled` | *(Behaviour: Set Enabled)* target: `Get Component (ScriptMachine)` result, enabled: `Literal Boolean false` |
| `Get Component` | Rigidbody2D |
| `Vector2 .ctor` | x: `Literal Float 0`, y: `Literal Float 0` |
| `Rigidbody2D: Velocity` | target: `Get Component (Rigidbody2D)` result, input: `Vector2 .ctor` result |
| `Set Variable` | *(Object scope)* name: `InDialogue`, type: Boolean, input: `Literal Boolean true` |

**Internal flow:**
`On Enter State` → `Set Enabled` assign → `Rigidbody2D: Velocity` assign → `Set Variable` assign

---

## Transitions

| From | Trigger | To |
|---|---|---|
| EXPLORING | `On Custom Event` name: `StartDialogue` | DIALOGUE |
| DIALOGUE | `On Custom Event` name: `EndDialogue` | EXPLORING |

To add a transition: right-click the source state → **Make Transition** → drag to destination. Click the transition arrow → set event type to **On Custom Event** and enter the name exactly as shown.

---

## Firing Transitions from C#

Any C# script can trigger a state change by firing a custom event on the Player GameObject:

```csharp
CustomEvent.Trigger(playerGameObject, "StartDialogue");
CustomEvent.Trigger(playerGameObject, "EndDialogue");
```

`DialogueManager.cs` will use these when dialogue begins and ends.

---

## C# Scripts That Read InDialogue

| Script | How it reads the flag |
|---|---|
| `JumpBuffer.cs` | `Variables.Object(gameObject).Get<bool>("InDialogue")` |
| `StarThrower.cs` | same |

Both early-return from `Update` when `InDialogue` is `true`, suppressing jump and throw input during dialogue.

---

## Cross-State Flow Summary

| Event Fired | From State | To State | Effect |
|---|---|---|---|
| `StartDialogue` | Exploring | Dialogue | ScriptMachine disabled, velocity zeroed, InDialogue = true |
| `EndDialogue` | Dialogue | Exploring | ScriptMachine re-enabled, InDialogue = false |

---

## Verification

1. Add `Debug_StateToggle.cs` to any scene object; assign Player reference.
2. Press **Play**. Press **T** → `StartDialogue` fires → movement stops, jump stops, throw stops.
3. Press **T** again → `EndDialogue` fires → full control restored.
4. Delete `Debug_StateToggle.cs` once Phase 1 is confirmed.
