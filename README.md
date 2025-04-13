AI FSM DOC
How AI behaviors influence player strategy and decision-making. 

How player actions dynamically alter AI states and responses.

Challenges faced during implementation and their solutions.  

The AI’s behavior affects the players strategy by making the player utilize the walls around them to lose the line of sight from the AI. 
The AI can also hear the player if they are not crouched behind or around it, making the player have to be careful when to move and crouch. 
Then there's the AI’s search, the AI will look around itself looking for the player and if it spots the player then the AI will begin to chase again. This happens when the player cannot be seen or heard.

If a player is walking around behind the AI trying to avoid it then the player will trigger the AI’s chase state. 
If the player goes behind a wall when getting chased, the AI will go to where the player last was, and begin its search state, looking for the player nearby between 3 different search spots. 
If you are crouched within the AI’s hearing range you won't alert the AI to your location.

Biggest challenge was getting the AI to lose LOS when i go behind a wall, in previous versions of the AI, they would track me behind a wall and still chase and change its route to me because of it. 
Solution was to make a last known location for the AI to go to when i'm no longer in its LOS, the second i leave the vision the AI will go to where it last saw me instead of just knowing where i went. 
Also hearing was a pain, I made it so if the player is within a certain area and isn't crouching then the AI will “hear” the player and begin chasing.


