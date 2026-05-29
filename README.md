Question 1:

<img width="976" height="549" alt="image" src="https://github.com/user-attachments/assets/8e89e792-2493-47ea-8e86-8e00a23e1006" />

This shader graph's purpose was to map on a texture onto white portions of tiles alongside phase through three different colors to give a night sky sort of aesthetic alongside the particle effects. I wasn't able to get the texture fully working but the color phasing did work. I did this by utilizing UV, time, multiply, sine, and lerp nodes primarily. UV to map it onto tiles, time to control the periods of when the different colors phase, multiply to handle color speed phasing speed, sine to go between 0-1 values of the different colors, and lerp to get it moving smoothly between the different colors rather than flashing between them. Through multiple branches of these nodes I managed to hook up all three colors into a cycle, applying them to a material, and assigning the material within the tilemap. The previous white tiles now phase between red, purple, and blue, giving a night sky aesthetic I was striving for since my pitch. You can find it on the tile walls primarily, I kept the platforms white for noticeability.

Question 2:

The main thing I got from playtesting prior was the usage of the star not entirely feeling necessary to progress. My thought process for this was that platformer abilities were meant to make previous tasks easier and enable new tasks to be able to be completed, but with the new direction of game feel, I found that utilizing the star ability to make it easier for the player to progress through the levels and give them further reach addressed this feedback. So, if a player is struggling or thinks a level is difficult, they can utilize their star ability to reach further with the projectile and still be launched upwards.

Question 3:

I added a new level, alongside changing the existing door system to be a bit smoother for the night sky aesthetic. Door blocks now fade out, and platforms allow you to go up on a one-way so it's more coheisve. The new level is similar to other levels but with more buttons (green bubbles) that have to be interacted with to progress. Most of the work I've done recently were related to tilemap layout and overall level design to make the play experience more cohesive.
