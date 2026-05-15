Milestone 2
Goal: Create a small puzzle system involving "buttons" and doors, where the player will have to interact with button(s) by walking into it or hitting it with the star projectile to open the door.

Q.1
1. Create a button script.
- Make isActivated bool to check if the button is already pressed
- Check collision if colliding object is an activator to trigger the button rather than just a random collider.
- If collider is valid, activate button.
- Also check if star collider, in which case also activate button.
- Do some fun stuff, adding animators and sfx.
2. Create a door script.
- Create an array that accepts objects that have the button script attached.
- Check if all isActivated bools in those objects are set to true.
- If true, lerp door up.
- If false, door stay.
- Add sfx for kicks
3. Create an activator script that allows the player to interact with buttons too. Also adjust star to do the same.
- Set player type to Activator through a class
- Set star type to Activator through a class

Q.2
- It helped a little bit, I'm really used to just doing things first rather than planning them out. I conceptualize then I execute, but it did help planning these on paper first to get a slightly better idea before tackling it.
- I think something to help my breakdown skills a bit more would be to plan out a bit more on the design aspect rather than just the purely scripting objectives. I did want to make the star arc so it has a bit more of a complicated usage but that might have to be for a future milestone.

Q.3
- I bridged a visual scripting graph and a normal script because my ground checks weren't sufficient. I ran into a bug, where since my visual scripting graph creates its own physics system and the player has to stop when they touch ground, the side of platforms counted as ground so the player could just stick to walls. That wasn't really my intention however, so I made the movement mode from PlayerMovementv3 an accessible variable onto the GroundCheck.cs script and check the raycast of the platform being checked. I needed to access the movement mode so the player could fall properly, otherwise they'd just be floating when they touch the side of a platform. So I'm basically accessing the variable from the visual scripting graph and changing it with a traditional script to check for ground more effeciently. The script used was GroundCheck.cs and here is a screenshot of the visual scripting graph:
<img width="1471" height="911" alt="image" src="https://github.com/user-attachments/assets/898cd8c5-6248-4a51-b40a-7f356f869007" />

Q.4
- I do want to use timeline and get more familiar with it. Historically, I made my own cutscene systems but I feel like timeline is a powerful tool that I could use instead or for other aspects such as animation timings for the background.
