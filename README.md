1. Visual Scripting Graph:
My visual scripting graph is my player movement script. It's primarily comprised of the following sections: Horizontal Movement, Gravity Gate, Rising Gravity, Falling Gravity, Jump Press, Jump Release, Set Velocity, Apex Check, Jump Processing, and Clamp Velocity.
Horizontal movement checks the input and multiplies a rigidbody by a MoveSpeed variable to get X velocity. Gravity Gate checks a movement mode integer to decide what type of gravity should be applied, rising or falling gravity, providing different gravity forces to make the jump feel snappier downwards.
Apex Check detects when vertical velocity is past zero, the peak of the jump, to change between gravity modes. Jump press starts the jump, setting vertical velocity and reseting jump timer, a timer that determines how long a player can hold down jump. Jump processing runs while jump is held, incrementing jump timer and adding jump acceleration with vertical velocity, maxing out at a float variable called JumpMaxSpeed, with a duration of another float called MaxJumpDuration.
Jump release checks if the player lets go of jump early, allowing the player to control how high they jump and have some fun short jumping. Set velocity combines X and Y into a vector, and clamp velocity makes sure the player only falls as fast as a float var called MaxFallSpeed allows them to, with the result being applied to the rigidbody.

2. Updated breakdown
<img width="1026" height="790" alt="image" src="https://github.com/user-attachments/assets/82f441af-b620-4895-9f91-f100ca1ad538" />

I updated my breakdown through cutting out tools such as SOs that I found alternative methods of implementing for the scope of this project. This includes the NPCDialogueTrigger.cs script that contains an array for dialogue that each NPC will say. I pursued this because I believed that this was more befitting the scope of this project, and had less overhead than managing an excel spreadsheet with all the present dialogue, and having to reimport said file upon every update.
I added nodes within my breakdown that represent the two states currently that a player can be in, the exploring and dialogue state. I wasn't sure exactly where to incorporate the player object within this, deciding that it should transition into the exploring state by default since that's how the current build starts. Then, build transitions between the dialogue and exploring state through arrows explaining how transitions occur.

The state machine involves two distinct states: dialogue and exploring. 

Exploring allows you to utilize the various abilities and move around freely utilizing a custom-made player movement system inspired by Hollow Knight. 
It's focused on responsiveness, setting velocity to zero the moment the player lets go of an input, and a sophisticated jump velocity system that allows you to control how much you jump, how easy it is to turn while jumping, and a bit of floatiness once reaching the apex assuming the player is holding down the jump key (Z).
I thought this would be important to implement to ensure that a 2D platformer allows the player to feel fully in control.
You are also capable of throwing a star by pressing X once you've walked into the powerup while in the exploration state.

The dialogue state occurs when the player is in front of an NPC interactable object and presses UP. This transitions the player from exploring to dialogue, removing their ability to move, repurposing their jump key to a dialogue progression key.
Z will progress through an array of dialogue that will display on a UI panel, each press prompting a new line until the array of dialogue is exhausted. Once exhausted, the panel hides itself and returns the player to an exploration state.
