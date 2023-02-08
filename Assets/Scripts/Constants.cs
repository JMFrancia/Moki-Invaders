using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants
{
    public static class Events
    {
        public const string ENEMY_DESTROYED = "EnemyDestroyed";
        public const string PLAYER_SHOT_FIRED = "PlayerShotFired";
        public const string PLAYER_DESTROYED = "PlayerDestroyed";
        public const string GAME_OVER_LOSS = "GameOverLoss";
        public const string ENEMY_SHOT_FIRED = "EnemeyShotFired";
        public const string GAME_START = "GameStart";
        public const string GAME_OVER_WIN = "GameOverWin";
    }

    public static class Tags
    {
        public const string SHOT = "Shot";
    }
}
