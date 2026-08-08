using System.Numerics;
using Players;
using Raylib_cs;
public class InventoryUI
{
    Texture2D inv_ItemsUI, inv_CombineUI;
    Sound inv_Open, inv_switchSection, closeInv, inv_slotCursor;
    const int GRID_COLS = 4;
    const int GRID_ROWS = 4;
    const int COMBINE_ROWS = 2;
    const int COMBINE_COLS = 4;
    int cursorCol, cursorRow;
    public bool InvIsShow { get; private set; } = false;
    public enum InventoryTab { Items, Combine }
    private InventoryTab currentTab = InventoryTab.Items;
    private float fadeAlpha = 0f;

    public float FadeAlpha
    {
        get => fadeAlpha;
        private set => fadeAlpha = Math.Clamp (value, 0f, 255f);
    }
    
    Rectangle slotsDraw;
    float paddingInv = 15f;
    
    Font UI_Menu;
    Inventory inventory;
   
    
    Vector2 posInventory;
    public InventoryUI(ref Inventory _inventory)
    {
        inventory = _inventory;
        posInventory = new Vector2(920,0);
        
        inv_ItemsUI = Raylib.LoadTexture(Path.Combine("sprites", "inventory_hud", "inventory_ui_items.png"));
        inv_CombineUI = Raylib.LoadTexture(Path.Combine("sprites", "inventory_hud", "inventory_ui_combine.png"));

        inv_Open = Raylib.LoadSound(Path.Combine("audio","sfx","UI_OpenMenu.wav"));
        inv_switchSection = Raylib.LoadSound(Path.Combine("audio","sfx","UI_SwitchModeInventory.wav"));
        inv_slotCursor = Raylib.LoadSound(Path.Combine("audio","sfx","UI_SlotsCursor.wav"));

        UI_Menu = Raylib.LoadFont(Path.Combine("fonts","Requiem_RE9.ttf"));
        
        Raylib.SetSoundVolume(inv_switchSection, 0.8f);
        Raylib.SetSoundVolume(inv_slotCursor, 0.8f);
        Raylib.SetSoundVolume(inv_Open, 0.6f);
    }
    
    public void Input()
    {
        if(Raylib.IsKeyPressed(KeyboardKey.Tab))
        {
            InvIsShow = !InvIsShow;
            Raylib.PlaySound(inv_Open);
        }
        
        if(InvIsShow)
        {
            //yeah, I know this is pretty long
            cursorCol += (Raylib.IsKeyPressed(KeyboardKey.D) ? 1:0) - (Raylib.IsKeyPressed(KeyboardKey.A) ? 1:0);
            cursorRow += (Raylib.IsKeyPressed(KeyboardKey.S) ? 1:0) - (Raylib.IsKeyPressed(KeyboardKey.W) ? 1:0);
            
            if (currentTab == InventoryTab.Items)
            {
                cursorCol = Math.Clamp(cursorCol, 0, GRID_COLS - 1);
                cursorRow = Math.Clamp(cursorRow, 0, GRID_ROWS - 1);
            }else if(currentTab == InventoryTab.Combine)
            {
                cursorCol = Math.Clamp(cursorCol, 0, COMBINE_COLS - 1);
                cursorRow = Math.Clamp(cursorRow, 0, COMBINE_ROWS - 1);
            }
            
             if(Raylib.IsKeyPressed(KeyboardKey.D) || Raylib.IsKeyPressed(KeyboardKey.A) ||
             Raylib.IsKeyPressed(KeyboardKey.W) || Raylib.IsKeyPressed(KeyboardKey.S))   
                Raylib.PlaySound(inv_slotCursor);
         
            if(Raylib.IsKeyPressed(KeyboardKey.Q))
            {
                currentTab = InventoryTab.Items;
                Raylib.PlaySound(inv_switchSection);
            }
            if(Raylib.IsKeyPressed(KeyboardKey.E))
            {
                currentTab = InventoryTab.Combine;
                Raylib.PlaySound(inv_switchSection);
            }

            if(Raylib.IsKeyPressed(KeyboardKey.X))
            {inventory.RemoveItem(cursorRow, cursorCol, currentTab==InventoryTab.Combine);}
        }
    }

    public void Update(float dt)
    {
        if(InvIsShow && FadeAlpha <255f) FadeAlpha += 2500f * dt;
        if(!InvIsShow && FadeAlpha >0f) FadeAlpha -= 2500f * dt;
    }

    public void Draw()
    {
        if(fadeAlpha<=0) return;
        
        float offsetX = 49f;  // horizontal distance from the edge of the inventory to the slots
        float offsetY = 100f;  // vertical distance (Space for header with Items/Combinations)
        float slotW = 50f;    // slot Weight
        float slotH = 47f;    // slot Height
        float gapX = 10f;   //gap X between slots
        float gapY = 8f;    //gap Y between slots
        
        Color corFade = new Color((byte)255, (byte)255, (byte)255, (byte)fadeAlpha);
        if (fadeAlpha > 0)
        {
            DrawVignette();
            if (currentTab == InventoryTab.Items)
            {
                Raylib.DrawTextureV(inv_ItemsUI, posInventory, corFade);
                DrawSlots(GRID_ROWS, GRID_COLS, false, offsetX, offsetY, slotW, slotH, gapX, gapY, corFade);
            }
            if (currentTab == InventoryTab.Combine)
            {
                Raylib.DrawTextureV(inv_CombineUI, posInventory, corFade);
                DrawSlots(COMBINE_ROWS, COMBINE_COLS, true, offsetX, offsetY, slotW, slotH, gapX, gapY, corFade);
            } 
            
        } 
    }

    void DrawSlots(int _ROWS, int _COLS, bool fromCombine, float offsetX, float offsetY, float slotW, float slotH, float gapX, float gapY, Color corfade)
    {
        float X_positionX;
        float X_positionY;
        Color dark = new Color((byte)100, (byte)100, (byte)100, (byte)100);
        Color whiteFull = new Color((byte)255, (byte)255, (byte)255, (byte)255);
        Vector2 posSlots;
        Vector2 Xs_pos;
        Vector2 sizeFillColor = new Vector2(50,47);
        
        for(int line = 0; line < _ROWS; line++)
        {
            for(int colum = 0; colum <_COLS; colum++)
            {
                posSlots.X = (posInventory.X + offsetX) + paddingInv + (colum * (slotW + gapX));
                posSlots.Y = (posInventory.Y + offsetY) + paddingInv + (line * (slotH + gapY));
                slotsDraw = new Rectangle(posSlots.X, posSlots.Y, slotW, slotH);
                X_positionX = (int)posSlots.X + (int)((slotW/2) - 6);
                X_positionY = (int)posSlots.Y + (int)((slotH/2) - 8);
                Xs_pos = new Vector2 (X_positionX, X_positionY);
                Raylib.DrawTextEx(UI_Menu, "X", Xs_pos, 20, 0, Color.Gray);
                Raylib.DrawRectangleLinesEx(slotsDraw, 2, corfade);
                Item currentItem = inventory[line, colum, fromCombine];
                if(line == cursorRow && colum == cursorCol)
                {Raylib.DrawRectangleGradientV((int)posSlots.X, (int)posSlots.Y, (int)sizeFillColor.X, (int)sizeFillColor.Y, whiteFull, dark);}
                if(currentItem.type != ItemType.None)
                {
                        Rectangle src = new Rectangle(0,0, currentItem.icon.Width, currentItem.icon.Height);
                        Rectangle dest = new Rectangle(posSlots.X, posSlots.Y, slotW, slotH);

                        Raylib.DrawTexturePro(currentItem.icon, src, dest, Vector2.Zero, 0f, Color.White);
                }
                
            }
        }
        if(Program.isShowDebug)
        {
        Raylib.DrawText($"Cursor_Row:{cursorRow}", 50,160, 30, Color.Green);
        Raylib.DrawText($"Cursor_Col:{cursorCol}", 50,190, 30, Color.Green);
        } 
    }

    public void DrawVignette()
    {
    byte vigAlpha = (byte)(180f * (fadeAlpha / 255f));

    int screenW = 1280;
    int screenH = 720;
    int vigSize = 70; // tamanho da vinheta nas bordas

    Color dark = new Color((byte)0, (byte)0, (byte)0, vigAlpha);        // preto semi-transparente
    Color transparent = new Color(0, 0, 0, 0);   // invisível no centro

    // topo → baixo
    Raylib.DrawRectangleGradientV(0, 0, screenW, vigSize, dark, transparent);
    // baixo → cima
    Raylib.DrawRectangleGradientV(0, screenH - vigSize, screenW, vigSize, transparent, dark);
    // esquerda → direita
    Raylib.DrawRectangleGradientH(0, 0, vigSize, screenH, dark, transparent);
    // direita → esquerda
    Raylib.DrawRectangleGradientH(screenW - vigSize, 0, vigSize, screenH, transparent, dark);
    }
    public void Unload()
    {
        Raylib.UnloadTexture(inv_CombineUI);
        Raylib.UnloadTexture(inv_ItemsUI);
        Raylib.UnloadSound(inv_Open);
        Raylib.UnloadSound(inv_switchSection);
        Raylib.UnloadSound(inv_slotCursor);
        Raylib.UnloadFont(UI_Menu);
    }
}