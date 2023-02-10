Welcome to the AFS Interview Project. This Unity project contains a draft of a tower-defense-ish game made for the purposes of the interview. 

Your tasks for today are as follows:
1. Create a new tower (buildable with Right Mouse Button) that shoots a new bullet type in quick bursts. The burst should shoot 3 bullets in rapid succession (say with about 0.25s interval between each shot) and then the tower should wait like 5 seconds before the next flurry of attacks. 
The new bullet should move like an arrow: it should curve and be affected by gravity. 
The tower should predict the position of the enemy before shooting (based on current speed and direction) so that the bullet has a chance of actually hitting it.
If the target changes direction mid-shot, then the shot might miss - naturally.
2. There's a bug with Simple Tower's bullets when there's a lot of towers and enemies around. Some bullets will freeze in the air and the Unity console will drop a bunch of errors. Find out what this is about and fix it to the best of your ability.
3. The word is that GameplayManager::Update method could use some optimization. Try to see if you can find something there to improve performance.

You'll be graded based on how well you've completed these tasks and on the quality of your code. 
If you're not 100% sure how something should work, try to make it work however *you* would see fit.

One more thing: get a public git repository for this task and commit your changes as you go. We'd like to see your repository work ethic in action.

Email us at work@ancientforgestudio.com with the repository link and a report of what you've managed to do within the next 24 hours.

Good luck!