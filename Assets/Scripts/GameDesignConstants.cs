using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameDesignConstants
{
    public const int ON_THROW_SCORE = 1;
    public const int ON_HIT_PENALTY = 1;

    public const int TREASURE_MIN_SCORE = 2;
    public const int TREASURE_MAX_SCORE = 5;
    public const int TREASURE_WITH_HAT_SCORE = 1;
    public const int TREASURE_WITH_HAT_SCORE_FOR_PLAYER_WITH_HAT = 5;

    public const float TREASURE_SPAWNS_HAT_PROBABILITY = 0.33f;
    public const int NUMBER_OF_HATS = 2;

    public const float COWBOY_HAT_THROW_COOLDOWN_BONUS = 2f; 
    public const float CROWN_HAT_SCORE_MULTIPLIER = 1.5f; 
    public const float POLICE_HAT_KO_MULTIPLIER = 2f; 
}
