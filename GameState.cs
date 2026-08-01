using Raylib_cs;
public enum GameState
{
    Playing,
    Paused,
    Options,
    GameOver,
    MainMenu
}

public static class GameManager
{
    static Stack<GameState> stateStack = new Stack<GameState>();
    
    static GameManager()
    {
        stateStack.Push(GameState.MainMenu);
    }

    public static GameState CurrentState => stateStack.Peek();
    public static void Push(GameState state) => stateStack.Push(state);
    public static void Pop()
    {
        if(stateStack.Count>1)
        {
            stateStack.Pop();
            Raylib.PlaySound(MainMenu.back_MENU);
        }
    }    
    public static void Input()
    {
        if(Raylib.IsKeyPressed(KeyboardKey.Escape))
        {
            if(CurrentState == GameState.Playing)
            {
            Raylib.PlaySound(MainMenu.confirm_MENU);
            Push(GameState.Paused);
            }
            else
                Pop();
        }
        
    }
    
}