namespace DragonsAndSharks.Common
{
    public class GameStateManager
    {
        private GameState _state;
        private static GameStateManager _gameStateManager;

        private GameStateManager(){}

        public static GameStateManager GetInstance()
        {
            return _gameStateManager ?? (_gameStateManager = new GameStateManager());
        }

        public void SetGameState(GameState gameState)
        {
            _state = gameState;
        }

        public GameState GetGameState()
        {
            return _state;
        }
    }

    public enum GameState
    {
        Menu = 0,
        GameOver = 1,
        Level = 2,
        Options = 3,
        Scores = 4,
        Undefined = 5
    }
}
