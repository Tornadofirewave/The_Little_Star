# Hollow Knight-Style Movement System
## Visual Scripting Node Guide

---

### Movement Modes
| Value | State |
|---|---|
| 0 | Grounded |
| 1 | Jumping |
| 2 | Rising (after jump released, before apex) |
| 3 | Falling |

---

## Rigidbody2D Setup
- Gravity Scale: `0`
- Collision Detection: `Continuous`
- Interpolate: `Interpolate`
- Constraints → Freeze Rotation Z: `checked`

---

## Variables (Blackboard)

| Name | Type | Default |
|---|---|---|
| MovementMode | Integer | 0 |
| MoveSpeed | Float | 8 |
| VerticalVelocity | Float | 0 |
| JumpTimer | Float | 0 |
| MaxJumpDuration | Float | 0.23 |
| JumpAcceleration | Float | 120 |
| JumpMaxSpeed | Float | 15 |
| RisingGravity | Float | 20 |
| FallingGravity | Float | 50 |
| MaxFallSpeed | Float | 20 |

---

## How to Read This Guide

Each section below is one **group** in the VS graph. Create a named group box for each one. Within a group, nodes are listed top-to-bottom in the order you place them. Control flow wires (**exit → enter**) are described at the end of each group under **"Exits to."** Value wires stay within their group unless noted.

---

## Group: HORIZONTAL MOVEMENT
*Value nodes only — no flow wires inside this group.*

| Node | Settings | Output |
|---|---|---|
| `Input: Get Axis` | axisName: `"Horizontal"` | → feeds Scalar Multiply |
| `Get Variable` | MoveSpeed | → feeds Scalar Multiply |
| `Scalar Multiply` | a: GetAxis result, b: MoveSpeed | → **horizVel** |

**horizVel** is used later in the **SET VELOCITY** group.
No flow wires. No exits.

---

## Group: GRAVITY GATE
*Decides whether to apply gravity this frame.*

| Node | Settings |
|---|---|
| `On Update` | *(event — flow starts here)* |
| `Get Variable` | MovementMode |
| `Scalar Equal` | a: MovementMode, b: `Literal Integer(1)` |
| `If` | condition: above Scalar Equal result |

**Exits to:**
- `If` **ifTrue** → **JUMP PROCESSING** group enter
- `If` **ifFalse** → **RISING GRAVITY** group enter

---

## Group: RISING GRAVITY
*Applied when mode == 2 (rising after jump).*

| Node | Settings |
|---|---|
| `Get Variable` | MovementMode |
| `Scalar Equal` | a: MovementMode, b: `Literal Integer(2)` |
| `If` | condition: above Scalar Equal result |
| `Get Variable` | RisingGravity |
| `Get Member` | Time.deltaTime |
| `Scalar Multiply` | a: RisingGravity, b: deltaTime |
| `Scalar Multiply` | a: above result, b: `Literal Float(-1)` |
| `Get Variable` | VerticalVelocity |
| `Scalar Add` | a: VerticalVelocity, b: negated gravity step |
| `Set Variable` | VerticalVelocity = above sum |

**Exits to:**
- `If` **ifFalse** → **FALLING GRAVITY** group enter
- `Set Variable` **exit** → **JUMP PROCESSING** group enter

---

## Group: FALLING GRAVITY
*Applied when mode == 0 or 3 (grounded or falling).*

| Node | Settings |
|---|---|
| `Get Variable` | FallingGravity |
| `Get Member` | Time.deltaTime |
| `Scalar Multiply` | a: FallingGravity, b: deltaTime |
| `Scalar Multiply` | a: above result, b: `Literal Float(-1)` |
| `Get Variable` | VerticalVelocity |
| `Scalar Add` | a: VerticalVelocity, b: negated gravity step |
| `Set Variable` | VerticalVelocity = above sum |

**Exits to:**
- `Set Variable` **exit** → **JUMP PROCESSING** group enter

> The `If` in RISING GRAVITY **ifFalse** enters this group's first flow node (the Set Variable).

---

## Group: JUMP PROCESSING
*Handles jump acceleration and auto-stop. Only runs logic when mode == 1.*

| Node | Settings |
|---|---|
| `Get Variable` | MovementMode |
| `Scalar Equal` | a: MovementMode, b: `Literal Integer(1)` |
| `If` | condition: above — **this is the group's enter point** |
| `Get Variable` | VerticalVelocity |
| `Get Variable` | JumpMaxSpeed |
| `Scalar Less Than` | a: VerticalVelocity, b: JumpMaxSpeed |
| `If` | condition: canAccelerate |
| `Get Variable` | JumpAcceleration |
| `Get Member` | Time.deltaTime |
| `Scalar Multiply` | a: JumpAcceleration, b: deltaTime |
| `Get Variable` | VerticalVelocity |
| `Scalar Add` | a: VerticalVelocity, b: jump delta |
| `Set Variable` | VerticalVelocity = above sum |
| `Get Variable` | JumpTimer |
| `Get Member` | Time.deltaTime |
| `Scalar Add` | a: JumpTimer, b: deltaTime |
| `Set Variable` | JumpTimer = above sum |
| `Get Variable` | JumpTimer |
| `Get Variable` | MaxJumpDuration |
| `Scalar Greater Or Equal` | a: JumpTimer, b: MaxJumpDuration |
| `If` | condition: timerDone |
| `Literal Integer` | value: `2` |
| `Set Variable` | MovementMode = 2 |
| `Literal Float` | value: `0` |
| `Set Variable` | JumpTimer = 0 |

**Internal flow order:**
1. `If (mode==1)` ifTrue → `If (canAccelerate)` enter
2. `If (canAccelerate)` ifTrue → `Set Variable(VerticalVelocity)` enter
3. `If (canAccelerate)` ifFalse → `Set Variable(JumpTimer)` *(increment)* enter
4. `Set Variable(VerticalVelocity)` exit → `Set Variable(JumpTimer)` *(increment)* enter
5. `Set Variable(JumpTimer)` exit → `If (timerDone)` enter
6. `If (timerDone)` ifTrue → `Set Variable(MovementMode=2)` enter
7. `Set Variable(MovementMode=2)` exit → `Set Variable(JumpTimer=0)` enter

**Exits to:**
- `If (mode==1)` **ifFalse** → **APEX CHECK** group enter
- `Set Variable(JumpTimer=0)` **exit** → **APEX CHECK** group enter
- `If (timerDone)` **ifFalse** → **APEX CHECK** group enter

> All three exits above wire into the same enter port on APEX CHECK.

---

## Group: APEX CHECK
*Switches mode to Falling once vertical velocity hits zero.*

| Node | Settings |
|---|---|
| `Get Variable` | MovementMode |
| `Scalar Greater Than` | a: MovementMode, b: `Literal Integer(0)` |
| `If` | condition: above — **this is the group's enter point** |
| `Get Variable` | MovementMode |
| `Scalar Less Or Equal` | a: MovementMode, b: `Literal Integer(2)` |
| `If` | condition: above |
| `Get Variable` | VerticalVelocity |
| `Scalar Less Or Equal` | a: VerticalVelocity, b: `Literal Float(0)` |
| `If` | condition: above |
| `Literal Integer` | value: `3` |
| `Set Variable` | MovementMode = 3 |

**Internal flow:**
1. `If (mode > 0)` **ifFalse** → CLAMP VELOCITY enter
2. `If (mode > 0)` **ifTrue** → `If (mode <= 2)` enter
3. `If (mode <= 2)` **ifFalse** → CLAMP VELOCITY enter
4. `If (mode <= 2)` **ifTrue** → `If (vertVel <= 0)` enter
5. `If (vertVel <= 0)` **ifFalse** → CLAMP VELOCITY enter
6. `If (vertVel <= 0)` **ifTrue** → `Set Variable(MovementMode=3)` → CLAMP VELOCITY enter

> All four exits wire into CLAMP VELOCITY's enter port.

---

## Group: CLAMP VELOCITY
*Caps vertical velocity so falling never exceeds MaxFallSpeed.*

| Node | Settings |
|---|---|
| `Get Variable` | MaxFallSpeed |
| `Literal Float` | value: `-1` |
| `Scalar Multiply` | a: MaxFallSpeed, b: -1 → negMaxFall |
| `Get Variable` | VerticalVelocity |
| `Get Variable` | JumpMaxSpeed |
| `Scalar Clamp` | value: VerticalVelocity, min: negMaxFall, max: JumpMaxSpeed |
| `Set Variable` | VerticalVelocity = clamped result — **enter point** |

**Exits to:**
- `Set Variable` **exit** → **SET VELOCITY** group enter

---

## Group: SET VELOCITY
*Combines horizVel and VerticalVelocity and applies to Rigidbody2D.*

| Node | Settings |
|---|---|
| `This` | *(self reference)* |
| `Get Component` | Rigidbody2D |
| `Get Variable` | VerticalVelocity |
| `Vector2 Constructor` | x: **horizVel** (from HORIZONTAL MOVEMENT), y: VerticalVelocity |
| `Set Member: Rigidbody2D.velocity` | target: Rigidbody2D, input: Vector2 — **enter point** |

---

## Group: JUMP PRESS
*Separate event chain — triggers when Jump button is pressed.*

| Node | Settings |
|---|---|
| `On Button Input` | button: `"Jump"`, action: `Down` |
| `Get Variable` | MovementMode |
| `Scalar Equal` | a: MovementMode, b: `Literal Integer(0)` |
| `If` | condition: isGrounded |
| `Literal Integer` | value: `1` |
| `Set Variable` | MovementMode = 1 |
| `Literal Float` | value: `0` |
| `Set Variable` | VerticalVelocity = 0 |
| `Set Variable` | JumpTimer = 0 |

**Internal flow:**
`On Button Input` → `If` enter → ifTrue → `Set Variable(MovementMode)` → `Set Variable(VerticalVelocity)` → `Set Variable(JumpTimer)`

---

## Group: JUMP RELEASE
*Separate event chain — triggers when Jump button is released early (short hop).*

| Node | Settings |
|---|---|
| `On Button Input` | button: `"Jump"`, action: `Up` |
| `Get Variable` | MovementMode |
| `Scalar Equal` | a: MovementMode, b: `Literal Integer(1)` |
| `If` | condition: stillJumping |
| `Literal Integer` | value: `2` |
| `Set Variable` | MovementMode = 2 |
| `Set Variable` | JumpTimer = 0 |
| `Get Variable` | VerticalVelocity |
| `Literal Float` | value: `0.5` |
| `Scalar Multiply` | a: VerticalVelocity, b: 0.5 |
| `Set Variable` | VerticalVelocity = cut result |

**Internal flow:**
`On Button Input` → `If` enter → ifTrue → `Set Variable(MovementMode)` → `Set Variable(JumpTimer)` → `Set Variable(VerticalVelocity)`

---

## Group: LAND DETECTION
*Separate event chain — detects landing. The floor normal check can be added later; for now any collision while falling counts as a landing.*

| Node | Settings |
|---|---|
| `On Collision Stay 2D` | *(trigger output)* |
| `Get Variable` | MovementMode |
| `Scalar Equal` | a: MovementMode, b: `Literal Integer(3)` |
| `If` | condition: wasFalling |
| `Literal Integer` | value: `0` |
| `Set Variable` | MovementMode = 0 |
| `Literal Float` | value: `0` |
| `Set Variable` | VerticalVelocity = 0 |

**Internal flow:**
`On Collision Stay 2D` → `If (wasFalling)` enter → ifTrue → `Set Variable(MovementMode)` → `Set Variable(VerticalVelocity)`

---

## Group: LEAVE GROUND
*Separate event chain — starts falling when player walks off an edge.*

| Node | Settings |
|---|---|
| `On Collision Exit 2D` | *(trigger output)* |
| `Get Variable` | MovementMode |
| `Scalar Equal` | a: MovementMode, b: `Literal Integer(0)` |
| `If` | condition: wasGrounded |
| `Literal Integer` | value: `3` |
| `Set Variable` | MovementMode = 3 |

**Internal flow:**
`On Collision Exit 2D` → `If` enter → ifTrue → `Set Variable(MovementMode=3)`

---

## Cross-Group Flow Summary

| From | Port | To |
|---|---|---|
| GRAVITY GATE `If` | ifTrue | JUMP PROCESSING enter |
| GRAVITY GATE `If` | ifFalse | RISING GRAVITY enter |
| RISING GRAVITY `If` | ifFalse | FALLING GRAVITY `Set Variable` enter |
| RISING GRAVITY `Set Variable` | exit | JUMP PROCESSING enter |
| FALLING GRAVITY `Set Variable` | exit | JUMP PROCESSING enter |
| JUMP PROCESSING `If(mode==1)` | ifFalse | APEX CHECK enter |
| JUMP PROCESSING `Set Variable(JumpTimer=0)` | exit | APEX CHECK enter |
| JUMP PROCESSING `If(timerDone)` | ifFalse | APEX CHECK enter |
| APEX CHECK `If(shouldFall)` | ifTrue (after Set Variable) | CLAMP VELOCITY enter |
| APEX CHECK `If(shouldFall)` | ifFalse | CLAMP VELOCITY enter |
| CLAMP VELOCITY `Set Variable` | exit | SET VELOCITY enter |

---

## Tuning Reference

| Variable | Effect |
|---|---|
| `JumpAcceleration` | Explosiveness of jump (higher = snappier) |
| `MaxJumpDuration` | Maximum jump height |
| `RisingGravity` | Float time at apex (lower = floatier) |
| `FallingGravity` | Fall speed (higher = snappier landing) |
| `0.5` in JUMP RELEASE | Short hop height (lower = smaller tap jump) |
