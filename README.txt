The Life of a CS Major README

Controls:
    W A S D - movement
    E - advance dialogue

Scenes/Levels:
- Start Dorm Room -> Dorm Halls -> Silcon Halls

Known Issues
- Scene Transition Bug: When moving from the Dorm Room to the Dorm Halls, holding the S key (down) can cause the player to immediately trigger the exit in the next scene, sending them back to the start.
- Spawn Position Reset: Player position is not preserved between scenes. Instead of appearing at the intended exit point, the player is reset to the scene’s default start position.
- Enemy Animations: Patrolling enemy animations are incorrect when moving vertically due to limitations in the current patrol script.
- Roommate NPC: No animations have been implemented yet.

Planned Improvements
- Add more enemies and NPCs in Silicon Halls to create a more dynamic and realistic school environment.
- Implement lighting effects, especially in the target classroom (Room 237), to highlight it as the destination.
- Trigger a dialogue/level cutscene when the player enters Room 237.
- Add classroom furniture (chairs, desks, artwork) to enhance visual detail.
- Add GPU, Deodorant, and Health collectibles throughout the map
- Remove attack/projectile which came with the base character