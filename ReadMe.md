# KingsAndQueensHat #

A team allocator for an Ultimate Disc Kings and Queens Tournament

This program generates sets of teams for a Kings and Queens tournament. It aims to automate the more difficult part of running such tournaments: the team allocation. In particular, we wanted to solve these problems:

- The sheer amount of time it can take to allocate many, evenly-matched teams
- Playing with different people from one round to the next
- People (especially experienced players) arriving late or leaving partway through the day, and causing teams to be very unbalanced
- Identifying players who are winning the tournament, and spreading them out across the teams (to hopefully end up with one only king and one queen at the end of the day)


## Dependencies ##

This software runs on Windows, and requires .NET Framework 4.5. Most installations of Windows 7+ should have this installed by default; but always test the program before using it at an actual tournament.

## How to use this program ##

After creating a tournament, the first thing to do is to create the list of players. This is done in the Players tab. You can do it one by one; but a much easier way is to create the list in Excel, save it as a CSV file, and import it into the program.

The CSV file should contain three columns:

- Full name
- Gender ("Male" or "Female")
- Skill (“Novice”, “Beginner”, “Intermediate”, “Experienced”, or “Guru”)

Enter the number of teams you want into the box in the top right on the Rounds tab, and then click "Generate new teams" to create teams.

The program allocates teams, but does not allocate which team must play against which team; but it is simple enough to have Team 1 play Team 2; Team 3 play Team 4, etc.

At the end of a round, indicate which teams won, lost, or drew before creating a new round.

If players have registered for the tournament, but haven’t turned up (or need to leave early), uncheck their “Include” checkbox on the players tab. They won’t be included in the next team generation.

If players turn up who haven’t registered, you can add them manually to the player list, and optionally add them to the current game.

The raw data is stored in the current user’s Application data folder: usually `C:\Users\<username>\AppData\Roaming\KingAndQueenHat`. Rounds and settings are stored are in the XML format. These files are editable; but be cautious if editing them, as I haven’t spent much time helping diagnose errors in file format.

## Other Uses ##

While this program is designed for the laborious task of allocating many teams for a Kings and Queens hat tournament, it could easily be used to generate teams for a regular hat tournament; using a single round, and managing wins and losses outside this program.

## Known issues ##

- Cannot easily add registered players who turn up late to the current round (workaround is to delete them and re-add them; though this will delete them from any rounds they have already participated in)
- No user confirmation before deleting tournaments or players.

Please report any bugs you find.

## Version 1 Algorithm Specifics ##

For those interested in the specifics of the algorithm:

The algorithm to allocate teams isn't particularly smart. The program randomly creates a large number (N) of possible sets of teams, and then picks what it considers to be the best one. The number of initial team sets generated is set on the Settings tab; though it can be terminated early at any point. 1 million generations is good, but probably excessive.

To generate an initial team set, the program shuffles the players within their skills and genders, and creates a set of teams that are at least balanced by both skill and gender.

For each team set T, the program then runs various "penalty" algorithms (P) on it, each resulting in a score from 0 to 1 for each. The functions are:

- A heuristic to penalise situations where players who have already played together would be playing together again. The more often two people have played together, the harsher the penalty: exponentially increasing for each previous game together. For example, we prefer two pairs playing together twice than one pair playing together three times.
- A heuristic to penalise situations where many undefeated players are all on the one team. This is done by optimising for having the same number of undefeated male and female players on each team, and penalising deviations from this count.

Each algorithm is run across each of the N team sets. To keep things simple, I allowed each penalty algorithm to have a different dynamic range, so to ensure that all criteria were weighted equally, for each penalty P, the score is normalised to between 0 and 1 based on the specific dynamic range of all N team sets for penalty P. The penalties are all summed together (so each team set will be allocated a score between 0 and 2). 

The team set with the smallest summed penalty score is then selected as the "best" team set.

## Version 2 Algorithm Specifics ##

The version 2 algorithm is faster and its main goal is to create even teams where the top players are competing against each other each round. There is a checkbox on the **Settings** page to enable it.

It places less emphasis on the skill level of players (although it is a factor in the early rounds) and sorts the list of players by an adjusted points score. Players with higher scores are placed on teams with players with the lowest scores, and the highest ranking women are ideally placed in different teams to the highest ranking men.

The adjusted points score is to allow for players who have missed some games not to be treated as low scoring players. If a player has a 100% win ratio and has only played three games out of a possible six, they will be ranked in the middle-top of the player list and distributed among the teams accordingly. The adjusted points score is only for sorting - the winners are still the players with the highest actual scores.

## Continuing Development ##

This project is open sourced, and the code repository can be found here: https://github.com/smashery/KingsAndQueensHat

I consider this project mostly complete; but may do small things on it and fix bugs on request. If you have feedback, feature requests or bug reports, please report them at https://github.com/smashery/KingsAndQueensHat/issues. If you have ideas for more significant development and want to help out, please let me know. 
