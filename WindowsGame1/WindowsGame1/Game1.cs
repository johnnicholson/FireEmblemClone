using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Xml.Serialization;


namespace WindowsGame1
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        //variable declarations
        PQ movable2;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Tile[,] map;
        Vector2[] playersInBattle;
        public enum State { Title, Board, Pick, Shop, BoardToShop }
        
        State gameScene;
        Texture2D Title;
        KeyboardState keyBoard, oldKeyBoard;
        private SpriteFont menuFont;
        int mapHeight;
        int mapWidth;
        MapCursor cursor;
        Player[,] players;
        SpriteFont optionsFont;
        Texture2D Title2;
        Texture2D p1, p2;
        Texture2D[] playerTexture;
        Vector2 tempPlayerFocus;
        WeaponSet weaponsetter;
        Vector2 origLocation;
        Inventory shopInv;
        enum ShopState { Swap, Browse, Menu, Buy }
        ShopState shopState;
        static RandomNumberGenerator _rand = RandomNumberGenerator.Create();
        enum Classes
        {
            Pirate = 0, Mercenary = 1, Pupil = 2, Knight = 3, Cleric = 4, Cavalier = 5,
            Soldier = 6, Myrmidon = 7, Swordlord = 8, Recruit = 9, Thief = 10, Monk = 11,
            Archer = 12, Axelord = 13, WyvernRider = 14
        }
        Classes classes;
        int playerTurn;
        MenuCursor menuCursor;
        int pickNumber;
        SpriteFont statsFont;
        HashSet<Vector2> movable;
        HashSet<Vector2> curentPlayerLocations;
        HashSet<Vector2> attackable;
        HashSet<Vector2> enemyLocations;
        HashSet<Vector2> allPlayerLocations;
        enum BoardScene { Attack, Move, Normal, Battle }
        BoardScene boardScene;
        Vector2 playerFocus;
        Vector2 emptyVector;
        Texture2D highlightW;
        Texture2D highlightR;
        enum infoFocus { stats, weapon }
        infoFocus infofocus;
        LevelLibrary.Level level1;
        Texture2D shop;
        StoreCursor storeCursor;
        ShopMenuCursor shopMenuCursor;
        Vector2 shopFocus;
        int[] money;
        InvObject randObj;
        double timeElapsed;
        int levelNumber;
        bool AI;
        Keys AKey;
        Keys BKey;
        Keys SaveKey;
        Keys TurnKey;

        public enum AIStratagy { Attack, FortHeal, ItemHeal, Defence, Track }
        AIStratagy Stratagy;
        const int playerNumber = 8;
        public struct PointDistance
        {
            public PointDistance(int x, int y, int distance)
            {
                location.X = x;
                location.Y = y;
                this.distance = distance;
            }
            public Point location;
            public int distance;
        }
        
        public struct AIInfo
        {
            public PointDistance nearestPlayer;
            public InRange playersInRange;
            public PointDistance nearestFort;
        }

        public struct InRange
        {
            public InRange(int t)
            {
                howMany = 0;
                this.players = new List<Point>();
            }
            public int howMany;
            public List<Point> players;
        }

        SaveData saveData = new SaveData();
        public struct SaveData
        {
            
            public SaveData(int[] M, Player[,] players, int turn, bool aI, Vector2 v)
            {
                money = new int[2];
                p1 = new Player[playerNumber];
                p2 = new Player[playerNumber];
                this.money = M;
                for (int x = 0; x < playerNumber; x++)
                {
                    p1[x] = players[0,x];
                    p2[x] = players[1,x];
                }
                AI = aI;
                Turn = turn;
                cursorLocation = v;
            }

            public Player[] p1;

            public Player[] p2;
            public Vector2 cursorLocation;
            public int[] money;
            public bool AI;
            public int Turn;
        }
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = 600;
            graphics.PreferredBackBufferWidth = 800;
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {

            gameScene = State.Title;
            AKey = Keys.J;
            BKey = Keys.K ;
            SaveKey = Keys.T;
            TurnKey = Keys.N;
            classes = 0;
            money = new int[2];
            money[0] = 500;
            money[1] = 500;
            pickNumber = 1;
            playerTurn = 1;
            timeElapsed = 0;
            emptyVector = new Vector2(-1, -1);
            levelNumber = 1;
            AI = false;
            IsMouseVisible = true;
            base.Initialize();


        }
        //loading textures and setting some things
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            mapHeight = 35;
            mapWidth = 50;
            attackable = new HashSet<Vector2>();
            playerFocus = new Vector2();
            infofocus = infoFocus.stats;
            boardScene = BoardScene.Normal;
            Texture2D tiles = Content.Load<Texture2D>("goodTileSheetFE");
            p1 = Content.Load<Texture2D>("bluetileset");
            p2 = Content.Load<Texture2D>("redtileset");
            highlightW = Content.Load<Texture2D>("highlightW");
            level1 = Content.Load<LevelLibrary.Level>("level1");
            Title = Content.Load<Texture2D>("FE2");
            map = new Tile[mapHeight, mapWidth];
            menuFont = Content.Load<SpriteFont>("Menu");
            statsFont = Content.Load<SpriteFont>("stats");
            optionsFont = Content.Load<SpriteFont>("options");
            Title2 = Content.Load<Texture2D>("Title2");
            allPlayerLocations = new HashSet<Vector2>();
            movable = new HashSet<Vector2>();
            highlightR = Content.Load<Texture2D>("highlightR");
            int x = 0, y = 0;
            weaponsetter = new WeaponSet();
            for (x = 0; x < mapWidth; x++)
            {
                for (y = 0; y < mapHeight; y++)
                {
                    map[y, x] = new Tile(x, y,level1.Values[y,x], tiles);
                }
            }
            players = new Player[2, playerNumber];
            playerTexture = new Texture2D[2];
            playerTexture[0] = p1;
            playerTexture[1] = p2;
            curentPlayerLocations = new HashSet<Vector2>();
            enemyLocations = new HashSet<Vector2>();
            menuCursor = new MenuCursor(0, highlightW);
            movable2 = new PQ();
            playersInBattle = new Vector2[2];
            cursor = new MapCursor(new Vector2(2, 10), highlightW, mapHeight, mapWidth);
            shop = Content.Load<Texture2D>("shop");
            shopInv = new Inventory(statsFont);
            fillShop();
            storeCursor = new StoreCursor(0, highlightW);
            shopState = ShopState.Browse;
            shopMenuCursor = new ShopMenuCursor(0, highlightW);

        }
        protected override void UnloadContent()
        {

        }
        //update whole game
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            keyBoard = Keyboard.GetState();
            switch (gameScene)
            {
                case State.Title:
                    UpdateTitle();
                    break;
                case State.Pick:
                    UpdatePick(gameTime, keyBoard);
                    break;
                case State.Board:
                    if (playerTurn == 1 || !AI)
                        UpdateBoard(gameTime, keyBoard);
                    else AIUpdateBoard(gameTime, keyBoard);
                    break;
                case State.Shop:
                    UpdateShop(gameTime, keyBoard);
                    break;
                case State.BoardToShop:
                    UpdateTransition(gameTime);
                    break;
            }
            base.Update(gameTime);
            oldKeyBoard = keyBoard;
        }
        //draw for entire game
        protected override void Draw(GameTime gameTime)
        {


            GraphicsDevice.Clear(Color.CornflowerBlue);

            switch (gameScene)
            {
                case State.Title:
                    DrawTitle();
                    break;
                case State.Pick:
                    DrawPick();
                    break;
                case State.Board:
                    DrawBoard();
                    break;
                case State.Shop:
                    DrawShop();
                    break;
                case State.BoardToShop:
                    DrawTransition();
                    break;
            }
            base.Draw(gameTime);
        }
        //done with title
        void UpdateTitle()
        {
            if (keyBoard.IsKeyDown(AKey) && oldKeyBoard.IsKeyUp(AKey))
            {
                gameScene = State.Pick;
            }
            if (keyBoard.IsKeyDown(TurnKey) && oldKeyBoard.IsKeyUp(TurnKey))
            {
                gameScene = State.Pick;
                AI = true;
            }
            if (keyBoard.IsKeyDown(SaveKey) && oldKeyBoard.IsKeyUp(SaveKey))
            {
                saveData = LoadGame("FESaveData.sav");
                AI = saveData.AI;
                money = saveData.money;
                playerTurn = saveData.Turn;
                cursor.location = saveData.cursorLocation;
                for (int x = 0; x < playerNumber; x++)
                {
                    players[0, x] = saveData.p1[x];
                    if (players[0, x] != null)
                    {
                        players[0, x].MakeSprite(p1, 1, 3);
                        players[0, x].inv.Font = statsFont;
                    }
                    players[1, x] = saveData.p2[x];
                    if (players[1, x] != null)
                    {
                        players[1, x].MakeSprite(p2, 1, 3);
                        players[1, x].inv.Font = statsFont; 
                    }
                }
                gameScene = State.Board;
                boardScene = BoardScene.Normal;
             }
        }
        void DrawTitle()
        {
            spriteBatch.Begin();
            spriteBatch.Draw(Title2, new Rectangle(0, 0, 900, 600), Color.White);
            spriteBatch.DrawString(menuFont, "Press T to Load", new Vector2(50, 240), Color.DarkBlue);
            spriteBatch.DrawString(menuFont, "Press J for two Player", new Vector2(50, 340), Color.DarkBlue);
            spriteBatch.DrawString(menuFont, "Press N for one Player", new Vector2(50, 440), Color.DarkBlue);
            spriteBatch.End();
        }
        //done with pick
        void UpdatePick(GameTime gameTime, KeyboardState keyboard)
        {
            menuCursor.Update(gameTime, keyboard);
            if (pickNumber == 5 && playerTurn == 1)
            {
                pickNumber = 1;
                int x;
                for (x = 0; x <4; x++)
                {
                    if (players[0, x] != null)
                    {
                        players[0, x].location = new Vector2(2, 5 * (x + 2));
                        players[0, x].curhp = players[0, x].hp;
                        for (int q = 0; q < 4; q++)
                        {
                            players[0, x].LevelUp();
                        }
                    }
                }
                if (AI)
                {
                    gameScene = State.Shop;
                    for (x = 0; x < level1.Opponents; x++)
                    {
                        players[1, x] = defineClass((int)Enum.Parse(typeof(Classes), level1.GetClass(x)));
                        players[1, x].location = new Vector2(level1.GetPosition(x, 0), level1.GetPosition(x, 1));
                        players[1, x].curhp = players[1, x].hp;
                        for (int q = 0; q < 4; q++)
                        {
                            players[1, x].LevelUp();
                        }
                    }
                    return;
                }
                nextTurn();
            }
            else if (pickNumber == 5 && playerTurn == 2)
            {
                int x;
                for (x = 0; x < 4; x++)
                {
                    if (players[1, x] != null)
                    {
                        players[1, x].location = new Vector2(47, 5 * (x + 2));
                        players[1, x].curhp = players[1, x].hp;
                    }
                }
                gameScene = State.Shop;
                playerTurn = 1;
            }
            if (keyBoard.IsKeyDown(AKey) && oldKeyBoard.IsKeyUp(AKey))
            {
                
                players[playerTurn-1, pickNumber-1] = defineClass(menuCursor.position);
                players[playerTurn-1, pickNumber-1].sprite.Texture = playerTexture[playerTurn-1];
                pickNumber++;
            }
        }
        void DrawPick()
        {

            spriteBatch.Begin();
            spriteBatch.Draw(Title, new Rectangle(0, 0, 900, 600), new Color(235, 235, 235));
            int i;
            if (playerTurn == 1)
            {
                spriteBatch.DrawString(menuFont, "Player 1 Pick", new Vector2(200, 100), Color.DarkBlue);
            }
            if (playerTurn == 2)
            {
                spriteBatch.DrawString(menuFont, "Player 2 Pick", new Vector2(200, 100), Color.DarkBlue);
            }
            for (i = 0; i < 15; i++)
            {
                classes = (Classes)i;
                spriteBatch.DrawString(optionsFont, classes.ToString(), new Vector2(50, i * 20 + 150), Color.Black);
            }
            spriteBatch.End();
            menuCursor.Draw(spriteBatch);
        }
        //working on shop
        void UpdateShop(GameTime gameTime, KeyboardState keyboard)
        {
            if (shopState == ShopState.Browse)
            {
                if (keyBoard.IsKeyDown(TurnKey) && oldKeyBoard.IsKeyUp(TurnKey) && !AI)
                {
                    nextTurn();
                }
                if(keyBoard.IsKeyDown(Keys.U) && oldKeyBoard.IsKeyUp(Keys.U))
                {
                    playerTurn = 1;
                    gameScene = State.Board;
                    return;
                }
                if (keyBoard.IsKeyDown(Keys.S) && oldKeyBoard.IsKeyUp(Keys.S) && storeCursor.focus == 4 && storeCursor.position == 4 && shopInv.view + 5 < shopInv.HowMany())
                {
                    shopInv.view++;
                }
                if (keyBoard.IsKeyDown(Keys.W) && oldKeyBoard.IsKeyUp(Keys.W) && storeCursor.focus == 4 && storeCursor.position == 0 && shopInv.view > 0)
                {
                    shopInv.view--;
                }
                if (storeCursor.focus < 4)
                    storeCursor.Update(gameTime, keyboard, players[playerTurn - 1, storeCursor.focus].inv.HowMany());
                else storeCursor.Update(gameTime, keyboard, shopInv.HowMany());
                if (keyBoard.IsKeyDown(AKey) && oldKeyBoard.IsKeyUp(AKey))
                {
                    if (storeCursor.focus < 4)
                    {
                        if (players[playerTurn - 1, storeCursor.focus].inv.Check(storeCursor.position) != null)
                            shopState = ShopState.Menu;
                    }
                    else shopState = ShopState.Menu;   
                    return;
                }
            }
            if (shopState == ShopState.Menu)
            {
                
                shopMenuCursor.Update(gameTime, keyBoard);
                if (keyboard.IsKeyDown(BKey) && oldKeyBoard.IsKeyUp(BKey))
                {
                    shopState = ShopState.Browse;
                }
                if (keyBoard.IsKeyDown(AKey) && oldKeyBoard.IsKeyUp(AKey))
                {
                    if (shopMenuCursor.position == 0 && storeCursor.focus != 4)
                    {
                        shopState = ShopState.Swap;
                        shopFocus = new Vector2(storeCursor.position, storeCursor.focus);
                        return;
                    }
                    if (shopMenuCursor.position == 1)
                    {
                        shopState = ShopState.Buy;
                        shopFocus = new Vector2(storeCursor.position, storeCursor.focus);
                        return;
                    }

                }
            }
            if (shopState == ShopState.Buy)
            {
                if (shopFocus.Y < 4)
                {
                    randObj = players[playerTurn - 1, (int)shopFocus.Y].inv.Remove((int) shopFocus.X);
                    money[playerTurn - 1] += randObj.worth/2;
                    shopState = ShopState.Browse;
                }
                else if (shopFocus.Y == 4)
                {
                    if (storeCursor.focus < 4)
                        storeCursor.Update(gameTime, keyboard, players[playerTurn - 1, storeCursor.focus].inv.HowMany());
                    else storeCursor.Update(gameTime, keyboard, shopInv.HowMany());
                    if (keyboard.IsKeyDown(BKey) && oldKeyBoard.IsKeyUp(BKey))
                    {
                        shopState = ShopState.Menu;
                    }
                    if (storeCursor.focus < 4)
                    {

                        if (keyBoard.IsKeyDown(AKey) && oldKeyBoard.IsKeyUp(AKey))
                        {

                            randObj = shopInv.Check((int)shopFocus.X + shopInv.view);
                            if (money[playerTurn - 1] - randObj.worth > 0)
                            {
                                money[playerTurn - 1] -= randObj.worth;
                                players[playerTurn - 1, storeCursor.focus].inv.Add(randObj);
                            }
                            shopState = ShopState.Browse;
                        }
                    }
                }
            }
            if (shopState == ShopState.Swap)
            {
                if (storeCursor.focus < 4)
                    storeCursor.Update(gameTime, keyboard, players[playerTurn - 1, storeCursor.focus].inv.HowMany());
                else storeCursor.Update(gameTime, keyboard, shopInv.HowMany());
                if (keyboard.IsKeyDown(BKey) && oldKeyBoard.IsKeyUp(BKey))
                {
                    shopState = ShopState.Browse;
                }
                if (shopFocus.Y < 4 && storeCursor.focus < 4)
                {
                    if (keyBoard.IsKeyDown(AKey) && oldKeyBoard.IsKeyUp(AKey))
                    {

                            players[playerTurn - 1, storeCursor.focus].inv.Add(players[playerTurn - 1, (int)shopFocus.Y].inv.Remove((int)shopFocus.X));
                            shopState = ShopState.Browse;

                    }
                }
            }
        }
        void DrawShop()
        {
            spriteBatch.Begin();
            spriteBatch.Draw(shop, new Rectangle(0, 0, 800, 600), Color.White);
            spriteBatch.DrawString(statsFont, "Player " + playerTurn.ToString(), new Vector2(350, 5), Color.White);
            spriteBatch.DrawString(statsFont, "Shop", new Vector2(5, 5), Color.White);
            spriteBatch.DrawString(statsFont, "Store", new Vector2(638, 50), Color.White);
            spriteBatch.DrawString(statsFont, money[playerTurn - 1].ToString(), new Vector2(600, 20), Color.White);
            if (shopState == ShopState.Browse)
            {
                storeCursor.Draw(spriteBatch);
            }
            if (shopState == ShopState.Menu)
            {
                spriteBatch.DrawString(statsFont, "Give", new Vector2(50, 520), Color.White);
                spriteBatch.DrawString(statsFont, "Sell / Buy", new Vector2(50, 540), Color.White);
                shopMenuCursor.Draw(spriteBatch);
            }
            if (shopState == ShopState.Swap || shopState == ShopState.Buy)
            {
                spriteBatch.Draw(highlightR, new Rectangle(35 + 150 * (int)shopFocus.Y, 80 + (int)shopFocus.X * 90, 145, 70), Color.White);
                storeCursor.Draw(spriteBatch);
            }
            for(int x = 0; x < 4; x++){
                spriteBatch.DrawString(statsFont, players[playerTurn - 1, x].CLASS.ToString(), new Vector2(38 + 150 * x, 50), Color.White);
                players[playerTurn - 1, x].inv.PrintAll(new Vector2(38 + 150 * x, 80), spriteBatch);
            } 


            shopInv.PrintAll(new Vector2(638, 80), spriteBatch);

            spriteBatch.End();
        }
        //Working on Board
        void AIUpdateBoard(GameTime gameTime, KeyboardState keyboard)
        {
            for (int y = 0; y < playerNumber; y++)
            {
                movable.Clear();
                if (players[1, y] != null && players[1,y].location != emptyVector)
                {
                    AIInfo info = AIPercieve(players[1, y].location, players[1, y].moves + 1);
                    Point destination = new Point();
                    if (players[1, y].curhp < 9 && info.nearestFort.distance < 8 && info.playersInRange.howMany == 0)
                    {
                        Stratagy = AIStratagy.FortHeal;
                    }
                    /*else if (players[1, y].curhp < 5)
                    {
                        Stratagy = AISratagy.ItemHeal;
                    }*/
                    else if (players[1, y].curhp > 9 && players[1, y].curhp < 14 && info.playersInRange.howMany == 0)
                    {
                        Stratagy = AIStratagy.Defence;
                    }

                    else if (info.playersInRange.howMany > 0)
                    {
                        Point lowestHealth;
                        int x = 1;
                        lowestHealth = new Point(info.playersInRange.players[0].X, info.playersInRange.players[0].Y);
                        while (x < info.playersInRange.players.Count)
                        {
                            if (players[info.playersInRange.players[x].X, info.playersInRange.players[x].Y].curhp < players[lowestHealth.X, lowestHealth.Y].curhp)
                                lowestHealth = new Point(info.playersInRange.players[x].X, info.playersInRange.players[x].Y);
                            x++;

                        }
                        Stratagy = AIStratagy.Attack;
                        destination = lowestHealth;
                    }
                    else if (info.nearestPlayer.distance < 20)
                    {
                        Stratagy = AIStratagy.Track;
                    }
                    else Stratagy = AIStratagy.Defence;
                    if (Stratagy == AIStratagy.Attack)
                    {
                        Point q = AIFindPositionToAttack(y, destination);
                        
                        AIMove(y, q);
                        MapReset();
                        AIAttack(y, destination);
                    }
                    if (Stratagy == AIStratagy.Track)
                    {

                        AIMove(y, AIFindClosest(y, info.nearestPlayer.location));
                    }
                }
            }
                nextTurn();
                for (int y = 0; y < playerNumber; y++)
                {
                    if (players[0, y] != null)
                    {
                        if (players[playerTurn - 1, y].location != emptyVector)
                        {
                            cursor.location = players[playerTurn - 1, y].location;
                            return;
                        }
                    }
                }
            }
        void UpdateBoard(GameTime gameTime, KeyboardState keyboard)
        {
            
            int x = 0, y = 0;
            curentPlayerLocations.Clear();
            enemyLocations.Clear();
            allPlayerLocations.Clear();

                for (y = 0; y <playerNumber; y++)
                {
                    if (players[1, y] != null)
                    {
                        if (playerTurn == 1)
                        {
                            enemyLocations.Add(players[1, y].location);
                            if (players[0, y] != null)
                            curentPlayerLocations.Add(players[0, y].location);
                        }
                        allPlayerLocations.Add(players[1, y].location);
                    }
                    if (players[0, y] != null)
                    {
                        if (playerTurn == 2)
                        {
                            enemyLocations.Add(players[0, y].location);
                            if (players[1, y] != null)
                            curentPlayerLocations.Add(players[1, y].location);
                        }
                        allPlayerLocations.Add(players[0, y].location);
                    }


                }
            
            for (y = 0; y <playerNumber; y++)
            {
                if (players[playerTurn - 1, y] != null)
                if (players[playerTurn - 1, y].location != emptyVector) break;
                if (y == playerNumber - 1)
                {
                    gameScene = State.BoardToShop;
                    money[playerTurn - 1] += 200;
                }
            }
            for (y = 0; y <playerNumber; y++)
            {
                if(players[otherTurn() - 1, y] != null)
                if (players[otherTurn() - 1, y].location != emptyVector) break;
                if (y == playerNumber - 1)
                {
                    gameScene = State.BoardToShop;
                    money[otherTurn() - 1] += 200;
                }
            }
            for (x = 0; x < mapWidth; x++)
            {
                for (y = 0; y < mapHeight; y++)
                {
                    map[y, x].Update();
                }
            }
            for (x = 0; x < 2; x++)
            {
                for (y = 0; y <playerNumber; y++)
                {
                    if(players[x,y] != null)
                    players[x, y].Update();
                }
            }
            foreach (Player value in players)
            {
                if(value != null)
                if (value.exp >= 50) value.LevelUp();
            }
            if (allPlayerLocations.Contains(cursor.location))
            {
                for (x = 0; x < 2; x++)
                {
                    for (y = 0; y <playerNumber; y++)
                    {
                        if(players[x,y] != null)
                        if (players[x, y].location == cursor.location)
                        {
                            tempPlayerFocus = new Vector2(x, y);
                        }
                    }
                }
            }
            else if (!curentPlayerLocations.Contains(cursor.location))
            {
                tempPlayerFocus = emptyVector;
            }
            if (boardScene == BoardScene.Battle)
            {
                Battle();
            }
            if(boardScene == BoardScene.Normal)
            {
                if (keyBoard.IsKeyDown(SaveKey) && oldKeyBoard.IsKeyUp(SaveKey))
                {
                    SaveGame(new SaveData(money, players, playerTurn, AI, cursor.location), "FESaveData.sav");
                }
                if (keyBoard.IsKeyDown(AKey) && oldKeyBoard.IsKeyUp(AKey))
                {
                    
                    if (curentPlayerLocations.Contains(cursor.location))
                    {
                       
                        for (x = 0; x < 2; x++)
                        {
                            for (y = 0; y <playerNumber; y++)
                            {
                                if (players[x, y] != null)
                                if (players[x, y].location == cursor.location)
                                {
                                    playerFocus = new Vector2(x, y);
                                }
                            }
                        }
                        if (players[(int)playerFocus.X, (int)playerFocus.Y].moved == false)
                        {
                            movable.Clear();
                            origLocation = players[(int)playerFocus.X,(int) playerFocus.Y].location;
                            highlightMove(cursor.location, players[(int)playerFocus.X, (int)playerFocus.Y].moves);
                            boardScene = BoardScene.Move;
                            return;
                        }
                        else if (players[(int)playerFocus.X, (int)playerFocus.Y].moved == true && players[(int)playerFocus.X, (int)playerFocus.Y].attacked == false)
                        {
                            attackable.Clear();
                            boardScene = BoardScene.Attack;
                            highlightAttack(cursor.location, 1);
                            return;
                        }
                         
                    }
                }
                if (keyBoard.IsKeyDown(TurnKey) && oldKeyBoard.IsKeyUp(TurnKey))
                {

                    nextTurn();
                    for (x = 0; x < 2; x++)
                    {
                        for (y = 0; y <playerNumber; y++)
                        {
                            if(players[x, y] != null)
                            players[x, y].reset();

                            
                        }
                    }

                        for (y = 0; y <playerNumber; y++)
                        {
                            if (players[playerTurn - 1, y] != null && players[playerTurn - 1, y].location != emptyVector)
                            {
                                //check for fort to heal
                                if (map[(int)players[playerTurn - 1, y].location.Y, (int)players[playerTurn - 1, y].location.X].type == Tile.Type.Fort)
                                {
                                    if (players[playerTurn - 1, y].hp - players[playerTurn - 1, y].curhp < (int)(.2f * players[playerTurn - 1, y].hp))
                                        players[playerTurn - 1, y].curhp += (int)(.2f * players[playerTurn - 1, y].hp);
                                    else players[playerTurn - 1, y].curhp = players[playerTurn - 1, y].hp;
                                }
                                if (players[playerTurn - 1, y].location != emptyVector)
                                {
                                    cursor.location = players[playerTurn - 1, y].location;
                                    return;
                                }
                            }
                        }
                    
                    
                }
                if (keyBoard.IsKeyDown(Keys.U) && oldKeyBoard.IsKeyUp(Keys.U))
                {
                    if (infofocus == infoFocus.stats)
                    {
                        infofocus = infoFocus.weapon;
                        return;
                    }
                    if (infofocus == infoFocus.weapon)
                    {
                        infofocus = infoFocus.stats;
                        return;
                    }
                }
                cursor.Update(gameTime, keyboard);
            }
            if (boardScene == BoardScene.Attack)
            {
                if (players[(int)playerFocus.X,(int)playerFocus.Y].inv.FirstWeapon() == null)
                {
                    boardScene = BoardScene.Normal;
                    MapReset();
                    return;
                }
                cursor.UpdateAnim(gameTime);
                if (keyboard.IsKeyDown(AKey) && oldKeyBoard.IsKeyUp(AKey))
                {
                    if (enemyLocations.Contains(cursor.location))
                    {
                        boardScene = BoardScene.Battle;
                        playersInBattle[0] = playerFocus;
                        for (x = 0; x < 2; x++)
                        {
                            for (y = 0; y <playerNumber; y++)
                            {
                                if(players[x, y] != null)
                                if (players[x, y].location == cursor.location)
                                {
                                    playersInBattle[1] = new Vector2(x, y);
                                }
                            }
                        }
                    }
                    else
                    {
                        boardScene = BoardScene.Normal;
                    }

                    players[(int)playerFocus.X, (int)playerFocus.Y].attacked = true;
                    for (x = 0; x < mapWidth; x++)
                    {
                        for (y = 0; y < mapHeight; y++)
                        {
                            map[y, x].highlighted = false;
                            map[y, x].highlightedRed = false;
                        }
                    }
                    
                }
                if (keyBoard.IsKeyDown(Keys.W) && oldKeyBoard.IsKeyUp(Keys.W))
                {
                    if (attackable.Contains(new Vector2(players[(int)playerFocus.X, (int)playerFocus.Y].location.X,
                        players[(int)playerFocus.X, (int)playerFocus.Y].location.Y - players[(int)playerFocus.X, (int)playerFocus.Y].inv.FirstWeapon().range)))
                    {
                        cursor.location = new Vector2(players[(int)playerFocus.X, (int)playerFocus.Y].location.X,
                        players[(int)playerFocus.X, (int)playerFocus.Y].location.Y - players[(int)playerFocus.X, (int)playerFocus.Y].inv.FirstWeapon().range);
                    }  
                }
                if (keyBoard.IsKeyDown(Keys.S) && oldKeyBoard.IsKeyUp(Keys.S))
                {
                    if (attackable.Contains(new Vector2(players[(int)playerFocus.X, (int)playerFocus.Y].location.X,
                        players[(int)playerFocus.X, (int)playerFocus.Y].location.Y + players[(int)playerFocus.X, (int)playerFocus.Y].inv.FirstWeapon().range)))
                    {
                        cursor.location = new Vector2(players[(int)playerFocus.X, (int)playerFocus.Y].location.X,
                        players[(int)playerFocus.X, (int)playerFocus.Y].location.Y + players[(int)playerFocus.X, (int)playerFocus.Y].inv.FirstWeapon().range);
                    }
                }
                if (keyBoard.IsKeyDown(Keys.A) && oldKeyBoard.IsKeyUp(Keys.A))
                {
                    if (attackable.Contains(new Vector2(players[(int)playerFocus.X, (int)playerFocus.Y].location.X - players[(int)playerFocus.X, (int)playerFocus.Y].inv.FirstWeapon().range,
                        players[(int)playerFocus.X, (int)playerFocus.Y].location.Y)))
                    {
                        cursor.location = new Vector2(players[(int)playerFocus.X, (int)playerFocus.Y].location.X - players[(int)playerFocus.X, (int)playerFocus.Y].inv.FirstWeapon().range,
                        players[(int)playerFocus.X, (int)playerFocus.Y].location.Y);
                    }
                }
                if (keyBoard.IsKeyDown(Keys.D) && oldKeyBoard.IsKeyUp(Keys.D))
                {
                    if (attackable.Contains(new Vector2(players[(int)playerFocus.X, (int)playerFocus.Y].location.X + players[(int)playerFocus.X, (int)playerFocus.Y].inv.FirstWeapon().range,
                        players[(int)playerFocus.X, (int)playerFocus.Y].location.Y)))
                    {
                        cursor.location = new Vector2(players[(int)playerFocus.X, (int)playerFocus.Y].location.X + players[(int)playerFocus.X, (int)playerFocus.Y].inv.FirstWeapon().range,
                        players[(int)playerFocus.X, (int)playerFocus.Y].location.Y);
                    }
                }
            }
            if (boardScene == BoardScene.Move)
            {

                if (keyBoard.IsKeyDown(Keys.W) && oldKeyBoard.IsKeyUp(Keys.W))
                {
                    if (movable.Contains(new Vector2(
                        players[(int)playerFocus.X, (int)playerFocus.Y].location.X, players[(int)playerFocus.X, (int)playerFocus.Y].location.Y - 1)))
                    {
                        players[(int)playerFocus.X, (int)playerFocus.Y].location = new Vector2(
                            players[(int)playerFocus.X, (int)playerFocus.Y].location.X, players[(int)playerFocus.X, (int)playerFocus.Y].location.Y - 1);
                    }
                }
                if (keyBoard.IsKeyDown(Keys.S) && oldKeyBoard.IsKeyUp(Keys.S))
                {
                    if (movable.Contains(new Vector2(
                        players[(int)playerFocus.X, (int)playerFocus.Y].location.X, players[(int)playerFocus.X, (int)playerFocus.Y].location.Y + 1)))
                    {
                        players[(int)playerFocus.X, (int)playerFocus.Y].location = new Vector2(
                            players[(int)playerFocus.X, (int)playerFocus.Y].location.X, players[(int)playerFocus.X, (int)playerFocus.Y].location.Y + 1);
                    }
                }
                if (keyBoard.IsKeyDown(Keys.A) && oldKeyBoard.IsKeyUp(Keys.A))
                {
                    if (movable.Contains(new Vector2(
                         players[(int)playerFocus.X, (int)playerFocus.Y].location.X - 1, players[(int)playerFocus.X, (int)playerFocus.Y].location.Y)))
                    {
                        players[(int)playerFocus.X, (int)playerFocus.Y].location = new Vector2(
                             players[(int)playerFocus.X, (int)playerFocus.Y].location.X - 1, players[(int)playerFocus.X, (int)playerFocus.Y].location.Y);
                    }
                }
                if (keyBoard.IsKeyDown(Keys.D) && oldKeyBoard.IsKeyUp(Keys.D))
                {
                    if (movable.Contains(new Vector2(
                        players[(int)playerFocus.X, (int)playerFocus.Y].location.X + 1, players[(int)playerFocus.X, (int)playerFocus.Y].location.Y)))
                    {
                        players[(int)playerFocus.X, (int)playerFocus.Y].location = new Vector2(
                            players[(int)playerFocus.X, (int)playerFocus.Y].location.X + 1, players[(int)playerFocus.X, (int)playerFocus.Y].location.Y);
                    }
                }
                if (keyboard.IsKeyDown(BKey) && oldKeyBoard.IsKeyUp(BKey))
                {
                    boardScene = BoardScene.Normal;
                    players[(int)playerFocus.X, (int)playerFocus.Y].location = origLocation;
                    for (x = 0; x < mapWidth; x++)
                    {
                        for (y = 0; y < mapHeight; y++)
                        {
                            map[y, x].highlighted = false;
                            map[y, x].highlightedRed = false;
                        }
                    }
                }
                if (keyboard.IsKeyDown(AKey) && oldKeyBoard.IsKeyUp(AKey))
                {
                    boardScene = BoardScene.Normal;
                    players[(int)playerFocus.X, (int)playerFocus.Y].moved = true;
                    for (x = 0; x < mapWidth; x++)
                    {
                        for (y = 0; y < mapHeight; y++)
                        {
                            map[y, x].highlighted = false;
                            map[y, x].highlightedRed = false;
                        }
                    }
                }

            }


        }
        void DrawBoard()
        {
            spriteBatch.Begin();
            spriteBatch.Draw(highlightW,  new Rectangle(0, 560, 800, 40), new Rectangle(5, 5, 1, 1), Color.White);
            spriteBatch.End();
            int x = 0, y = 0;
            for (x = 0; x < mapWidth; x++)
            {
                for (y = 0; y < mapHeight; y++)
                {
                    map[y, x].Draw(spriteBatch);
                }
            }
            if (boardScene == BoardScene.Normal)
            {
                cursor.Draw(spriteBatch);
            }
            if (boardScene == BoardScene.Attack)
            {
                cursor.Draw(spriteBatch);
            }
            for (x = 0; x < 2; x++)
            {
                for (y = 0; y <playerNumber; y++)
                {
                    if(players[x, y] != null)
                    players[x, y].Draw(spriteBatch);
                }
            }
            if (boardScene == BoardScene.Battle)
            {

            }
            if (tempPlayerFocus != emptyVector)
            {
                if (infofocus == infoFocus.stats)
                {
                    spriteBatch.Begin();
                    spriteBatch.DrawString(statsFont,players[(int)tempPlayerFocus.X, (int)tempPlayerFocus.Y].ToString(), new Vector2(5, 570), Color.Black);
                    spriteBatch.DrawString(statsFont,"exp: "  + players[(int)tempPlayerFocus.X, (int)tempPlayerFocus.Y].exp.ToString(), new Vector2(700, 570), Color.Black);
                    spriteBatch.End();
                }
                else if (infofocus == infoFocus.weapon)
                {
                    spriteBatch.Begin();
                    if(players[(int)tempPlayerFocus.X, (int)tempPlayerFocus.Y].inv.FirstWeapon() != null)
                    spriteBatch.DrawString(statsFont, players[(int)tempPlayerFocus.X, (int)tempPlayerFocus.Y].inv.FirstWeapon().ToString(), new Vector2(5,570), Color.Black);
                    else spriteBatch.DrawString(statsFont,"No Weapon", new Vector2(5, 570), Color.Black);
                    spriteBatch.End();
                }
            }
        }
        void UpdateTransition(GameTime gameTime)
        {
            timeElapsed += gameTime.ElapsedGameTime.Milliseconds;
        }
        void DrawTransition()
        {
            if (timeElapsed < 3000)
            {
                spriteBatch.Begin();
                spriteBatch.DrawString(menuFont, "Level" + levelNumber + " Over", new Vector2(375, 250), Color.White);
                spriteBatch.End();
            }
            if (timeElapsed > 3000)
            {
                gameScene = State.Shop;
                levelNumber++;
            }
        }
        Player defineClass(int pos)
        {
            Player playeri = new Player(0,0,p1, 1,3,statsFont);
            if (pos == 0) playeri = definePirate();
            if (pos == 1) playeri = defineMercenary();
            if (pos == 2) playeri = definePupil();
            if (pos == 3) playeri = defineKnight();
            if (pos == 4) playeri = defineCleric();
            if (pos == 5) playeri = defineCavalier();
            if (pos == 6) playeri = defineSoldier();
            if (pos == 7) playeri = defineMyrmidon();
            if (pos == 8) playeri = defineSwordlord();
            if (pos == 9) playeri = defineRecruit();
            if (pos == 10) playeri = defineThief();
            if (pos == 11) playeri = defineMonk();
            if (pos == 12) playeri = defineArcher();
            if (pos == 13) playeri = defineAxelord();
            if (pos == 14) playeri = defineWyvernRider();
            return playeri;
        }
        Player definePirate()
        {
            Player p = new Player(0, 0, p1, 1, 3, statsFont);
            p.CLASS = Player.CLIST.Pirate;
            p.hp = 20;
            p.con = 10;
            p.str = 6;
            p.skl = 1;
            p.spd = 3;
            p.luk = 0;
            p.def = 2;
            p.res = 0;
            p.hpgr = 75;
            p.strgr = 50;
            p.sklgr = 35;
            p.spdgr = 25;
            p.lukgr = 15;
            p.defgr = 10;
            p.resgr = 13;
            p.moves = 5;
            p.inv.Add(weaponsetter.setWeapon(Weapon.weapon.IronAxe));
            return p;
        }
        Player defineMercenary()
        {
            Player p = new Player(0,0,p1, 1,3, statsFont);
            p.CLASS = Player.CLIST.Mercenary;
            p.moves = 5;
            p.hp = 18;
            p.con = 9;
            p.str = 4;
            p.skl = 4;
            p.spd = 5;
            p.luk = 3;
            p.def = 2;
            p.res = 1;
            p.hpgr = 80;
            p.strgr = 40;
            p.sklgr = 40;
            p.spdgr = 32;
            p.lukgr = 30;
            p.defgr = 18;
            p.resgr = 20;
            p.inv.Add(weaponsetter.setWeapon(Weapon.weapon.IronSword));
            return p;
        }
        Player defineArcher()
        {
            Player p = new Player(0,0,p1, 1,3,statsFont);
            p.CLASS = Player.CLIST.Archer;
            p.moves = 5;
            p.hp = 14;
            p.con = 7;
            p.str = 3;
            p.skl = 4;
            p.spd = 4;
            p.luk = 4;
            p.def = 2;
            p.res = 2;
            p.hpgr = 70;
            p.strgr = 35;
            p.sklgr = 40;
            p.spdgr = 32;
            p.lukgr = 35;
            p.defgr = 15;
            p.resgr = 15;
            return p;
        }
        Player definePupil()
        {
            Player p = new Player(0,0,p1, 1,3,statsFont);
            p.CLASS = Player.CLIST.Pupil;
            p.moves = 5;
            p.hp = 14;
            p.con = 5;
            p.str = 5;
            p.skl = 3;
            p.spd = 3;
            p.luk = 6;
            p.def = 0;
            p.res = 3;
            p.hpgr = 55;
            p.strgr = 55;
            p.sklgr = 40;
            p.spdgr = 35;
            p.lukgr = 40;
            p.defgr = 10;
            p.resgr = 30;
            return p;
        }
        Player defineKnight()
        {
            Player p = new Player(0,0,p1, 1,3,statsFont);
            p.CLASS = Player.CLIST.Knight;
            p.hp = 22;
            p.con = 13;
            p.str = 5;
            p.skl = 1;
            p.spd = 0;
            p.luk = 3;
            p.def = 6;
            p.res = 2;
            p.hpgr = 80;
            p.strgr = 40;
            p.sklgr = 30;
            p.spdgr = 15;
            p.lukgr = 25;
            p.defgr = 28;
            p.resgr = 20;
            p.moves = 4;
            p.inv.Add(weaponsetter.setWeapon(Weapon.weapon.IronLance));
            return p;
        }
        Player defineCavalier()
        {
            Player p = new Player(0,0,p1, 1,3,statsFont);
            p.CLASS = Player.CLIST.Cavalier;
            p.hp = 19;
            p.con = 9;
            p.str = 4;
            p.skl = 3;
            p.spd = 4;
            p.luk = 4;
            p.def = 3;
            p.res = 3;
            p.hpgr = 75;
            p.strgr = 35;
            p.sklgr = 40;
            p.spdgr = 28;
            p.lukgr = 30;
            p.defgr = 15;
            p.resgr = 15;
            p.moves = 7;
            p.inv.Add(weaponsetter.setWeapon(Weapon.weapon.IronLance));
            return p;
        }
        Player defineSoldier()
        {
            Player p = new Player(0,0,p1, 1,3,statsFont);
            p.CLASS = Player.CLIST.Soldier;
            p.hp = 17;
            p.con = 6;
            p.str = 4;
            p.skl = 4;
            p.spd = 4;
            p.luk = 4;
            p.def = 3;
            p.res = 2;
            p.hpgr = 80;
            p.strgr = 50;
            p.sklgr = 30;
            p.spdgr = 20;
            p.lukgr = 25;
            p.defgr = 12;
            p.resgr = 15;
            p.moves = 5;
            p.inv.Add(weaponsetter.setWeapon(Weapon.weapon.IronLance));
            return p;
        }
        Player defineMyrmidon()
        {
            Player p = new Player(0,0,p1, 1,3,statsFont);
            p.CLASS = Player.CLIST.Myrmidon;
            p.hp = 14;
            p.con = 8;
            p.str = 3;
            p.skl = 6;
            p.spd = 6;
            p.luk = 1;
            p.def = 0;
            p.res = 0;
            p.hpgr = 70;
            p.strgr = 35;
            p.sklgr = 40;
            p.spdgr = 40;
            p.lukgr = 30;
            p.defgr = 15;
            p.resgr = 20;
            p.moves = 5;
            p.inv.Add(weaponsetter.setWeapon(Weapon.weapon.IronSword));
            return p;
        }
        Player defineSwordlord()
        {
            Player p = new Player(0,0,p1, 1,3,statsFont);
            p.CLASS = Player.CLIST.Swordlord;
            p.hp = 18;
            p.con = 9;
            p.str = 4;
            p.skl = 9;
            p.spd = 9;
            p.luk = 6;
            p.def = 3;
            p.res = 4;
            p.hpgr = 65;
            p.strgr = 25;
            p.sklgr = 30;
            p.spdgr = 30;
            p.lukgr = 25;
            p.defgr = 15;
            p.resgr = 22;
            p.moves = 6;
            p.inv.Add(weaponsetter.setWeapon(Weapon.weapon.IronSword));
            return p;
        }
        Player defineRecruit()
        {
            Player p = new Player(0,0,p1, 1,3,statsFont);
            p.CLASS = Player.CLIST.Recruit;
            p.hp = 14;
            p.con = 6;
            p.str = 3;
            p.skl = 4;
            p.spd = 4;
            p.luk = 6;
            p.def = 2;
            p.res = 1;
            p.hpgr = 75;
            p.strgr = 45;
            p.sklgr = 40;
            p.spdgr = 40;
            p.lukgr = 40;
            p.defgr = 25;
            p.resgr = 35;
            p.moves = 6;
            p.inv.Add(weaponsetter.setWeapon(Weapon.weapon.IronLance));
            return p;
        }
        Player defineThief()
        {
            Player p = new Player(0,0,p1, 1,3,statsFont);
            p.CLASS = Player.CLIST.Thief;
            p.hp = 14;
            p.con = 6;
            p.str = 2;
            p.skl = 3;
            p.spd = 9;
            p.luk = 3;
            p.def = 1;
            p.res = 0;
            p.hpgr = 50;
            p.strgr = 5;
            p.sklgr = 45;
            p.spdgr = 40;
            p.lukgr = 40;
            p.defgr = 5;
            p.resgr = 20;
            p.moves = 6;
            p.inv.Add(weaponsetter.setWeapon(Weapon.weapon.IronSword));
            return p;
        }
        Player defineMonk()
        {
            Player p = new Player(0,0,p1, 1,3,statsFont);
            p.CLASS = Player.CLIST.Monk;
            p.hp = 14;
            p.con = 6;
            p.str = 4;
            p.skl = 3;
            p.spd = 2;
            p.luk = 3;
            p.def = 1;
            p.res = 5;
            p.hpgr = 50;
            p.strgr = 30;
            p.sklgr = 35;
            p.spdgr = 32;
            p.lukgr = 45;
            p.defgr = 8;
            p.resgr = 40;
            p.moves = 5;
            return p;
        }
        Player defineCleric()
        {
            Player p = new Player(0,0,p1, 1,3,statsFont);
            p.CLASS = Player.CLIST.Cleric;
            p.hp = 13;
            p.con = 4;
            p.str = 1;
            p.skl = 2;
            p.spd = 4;
            p.luk = 2;
            p.def = 1;
            p.res = 6;
            p.hpgr = 50;
            p.strgr = 30;
            p.sklgr = 35;
            p.spdgr = 32;
            p.lukgr = 45;
            p.defgr = 8;
            p.resgr = 50;
            p.moves = 5;
            return p;
        }
        Player defineAxelord()
        {
            Player p = new Player(0,0,p1, 1,3,statsFont);
            p.CLASS = Player.CLIST.Axelord;
            p.hp = 23;
            p.con = 11;
            p.str = 9;
            p.skl = 4;
            p.spd = 2;
            p.luk = 5;
            p.def = 7;
            p.res = 3;
            p.hpgr = 70;
            p.strgr = 50;
            p.sklgr = 35;
            p.spdgr = 35;
            p.lukgr = 35;
            p.defgr = 50;
            p.resgr = 35;
            p.moves = 5;
            p.inv.Add(weaponsetter.setWeapon(Weapon.weapon.IronAxe));
            return p;
        }
        Player defineWyvernRider()
        {
            Player p = new Player(0,0,p1, 1,3, statsFont);
            p.CLASS = Player.CLIST.WyvernRider;
            p.hp = 20;
            p.con = 10;
            p.str = 5;
            p.skl = 5;
            p.spd = 6;
            p.luk = 1;
            p.def = 5;
            p.res = 0;
            p.hpgr = 80;
            p.strgr = 45;
            p.sklgr = 35;
            p.spdgr = 30;
            p.lukgr = 25;
            p.defgr = 25;
            p.resgr = 15;
            p.moves = 10;
            p.inv.Add(weaponsetter.setWeapon(Weapon.weapon.IronLance));
            return p;
        }
        void nextTurn()
        {
            playerTurn++;
            if (playerTurn == 3)
            {
                playerTurn = 1;
            }

        }
        public void checkMove(Vector2 location, int moves)
        {
            movable.Add(location);
            if (moves > 0)
            {
                if (location.X > 0)
                {
                    checkMove(new Vector2(location.X - 1, location.Y), moves - 1);
                }
                if (location.X < mapWidth - 1)
                {
                    checkMove(new Vector2(location.X + 1, location.Y), moves - 1);
                }
                if (location.Y > 0)
                {
                    checkMove(new Vector2(location.X, location.Y - 1), moves - 1);
                }
                if (location.Y < mapHeight - 1)
                {
                    checkMove(new Vector2(location.X, location.Y+1), moves - 1);
                }
                
            }
        }
        public void checkAttack(Vector2 location, int range)
        {
            bool cursorSet = false;
                if (location.X > 0)
                {
                    if (!cursorSet)
                    {
                        cursor.location = new Vector2(location.X - range, location.Y);
                        cursorSet = true;
                    }
                    attackable.Add(new Vector2(location.X - range, location.Y));
                }
                if (location.X < mapWidth - 1)
                {
                    if (!cursorSet)
                    {
                        cursor.location = new Vector2(location.X + range, location.Y);
                        cursorSet = true;
                    }
                    attackable.Add(new Vector2(location.X + range, location.Y));
                }
                if (location.Y > 0)
                {
                    if (!cursorSet)
                    {
                        cursor.location = new Vector2(location.X, location.Y - range);
                        cursorSet = true;
                    }
                    attackable.Add(new Vector2(location.X, location.Y - range) );
                }
                if (location.Y < mapHeight - 1)
                {
                    if (!cursorSet)
                    {
                        cursor.location = new Vector2(location.X, location.Y + range);
                        cursorSet = true;
                    }
                    attackable.Add(new Vector2(location.X, location.Y + range));
                }
        }
        public void highlightMove(Vector2 location, int moves)
        {
            checkMove2(location, moves);
            foreach (Vector2 value in movable)
            {
                map[(int)value.Y, (int)value.X].highlighted = true;
            }
        }
        public void highlightAttack(Vector2 location, int moves)
        {
            checkAttack(location, moves);
            foreach (Vector2 value in attackable)
            {
                map[(int)value.Y, (int)value.X].highlightedRed = true;
            }
        }
        static int RandomNext(int min, int max)
        {

            if (min > max) throw new ArgumentOutOfRangeException("min");

            byte[] bytes = new byte[4];

            _rand.GetBytes(bytes);

            uint next = BitConverter.ToUInt32(bytes, 0);

            int range = max - min;

            return (int)((next % range) + min);
        }
        public int otherTurn()
        {
            if (playerTurn == 1) return 2;
            if (playerTurn == 2) return 1;
            else return -5000;
        }
        public void checkMove2(Vector2 location, int moves)
        {
            movable2 = new PQ();
            movable.Add(location);
            movable2.Add(0, location);
            while(!movable2.empty() )
            {

                if (movable2.Peek().Value.X > 0 && !allPlayerLocations.Contains(new Vector2(movable2.Peek().Value.X - 1, movable2.Peek().Value.Y))) 
                    if(( movable2.Peek().Key + map[(int)movable2.Peek().Value.Y, (int)movable2.Peek().Value.X - 1].moveCost) <= moves &&
                    !movable.Contains(new Vector2(movable2.Peek().Value.X - 1, movable2.Peek().Value.Y)))

                {
                    movable2.Add(
                        movable2.Peek().Key + 
                        map[(int)movable2.Peek().Value.Y, (int)movable2.Peek().Value.X - 1].moveCost, 
                        new Vector2(movable2.Peek().Value.X - 1, movable2.Peek().Value.Y)
                        );
                    movable.Add(new Vector2(movable2.Peek().Value.X - 1, movable2.Peek().Value.Y));
                }
                if (movable2.Peek().Value.X < mapWidth - 1 && !allPlayerLocations.Contains(new Vector2(movable2.Peek().Value.X + 1, movable2.Peek().Value.Y))) 
                    if((movable2.Peek().Key +
                        map[(int)movable2.Peek().Value.Y, (int)movable2.Peek().Value.X + 1].moveCost) <= moves &&
                        !movable.Contains(new Vector2(movable2.Peek().Value.X + 1, movable2.Peek().Value.Y))) 
                {
                    movable2.Add(
                        movable2.Peek().Key +
                        map[(int)movable2.Peek().Value.Y, (int)movable2.Peek().Value.X + 1].moveCost, 
                        new Vector2(movable2.Peek().Value.X + 1, movable2.Peek().Value.Y)
                        );
                    movable.Add(new Vector2(movable2.Peek().Value.X + 1, movable2.Peek().Value.Y));
                }
                if (movable2.Peek().Value.Y > 0 && !allPlayerLocations.Contains(new Vector2(movable2.Peek().Value.X, movable2.Peek().Value.Y - 1)))
                    
                    if((movable2.Peek().Key +
                        map[(int)movable2.Peek().Value.Y - 1, (int)movable2.Peek().Value.X].moveCost) <= moves &&
                    !movable.Contains(new Vector2(movable2.Peek().Value.X, movable2.Peek().Value.Y - 1)))
                {
                    movable2.Add(
                        movable2.Peek().Key +
                        map[(int)movable2.Peek().Value.Y - 1, (int)movable2.Peek().Value.X].moveCost, 
                        new Vector2(movable2.Peek().Value.X, movable2.Peek().Value.Y - 1)
                        );
                    movable.Add(new Vector2(movable2.Peek().Value.X,movable2.Peek().Value.Y - 1));
                }
                if (movable2.Peek().Value.Y < mapHeight - 1 && !allPlayerLocations.Contains(new Vector2(movable2.Peek().Value.X, movable2.Peek().Value.Y + 1)))
                    if((movable2.Peek().Key +
                        map[(int)movable2.Peek().Value.Y + 1, (int)movable2.Peek().Value.X].moveCost) <= moves &&
                    !movable.Contains(new Vector2(movable2.Peek().Value.X, movable2.Peek().Value.Y + 1)))
                {
                    movable2.Add(
                        movable2.Peek().Key +
                        map[(int)movable2.Peek().Value.Y + 1, (int)movable2.Peek().Value.X].moveCost, 
                        new Vector2(movable2.Peek().Value.X, movable2.Peek().Value.Y + 1)
                        );
                    movable.Add(new Vector2(movable2.Peek().Value.X, movable2.Peek().Value.Y + 1));
                }
                movable2.Remove();
            }
        }
        public PointDistance checkNearestOpponentAI(Vector2 location, int within)
        {
            int moves = within;
            movable.Clear();
            movable2 = new PQ();
            movable.Add(location);
            
            movable2.Add(0, location);
            while (!movable2.empty())
            {
                    for (int y = 0; y <playerNumber; y++)
                    {
                        if (players[0, y] != null)
                        if (players[0, y].location == movable2.Peek().Value)
                        {

                            return new PointDistance(0, y,(int) movable2.Peek().Key);
                        }
                    }

                if (movable2.Peek().Value.X > 0 )
                    if ((movable2.Peek().Key + map[(int)movable2.Peek().Value.Y, (int)movable2.Peek().Value.X - 1].moveCost) <= moves &&
                    !movable.Contains(new Vector2(movable2.Peek().Value.X - 1, movable2.Peek().Value.Y)))
                    {
                        movable2.Add(
                            movable2.Peek().Key +
                            map[(int)movable2.Peek().Value.Y, (int)movable2.Peek().Value.X - 1].moveCost,
                            new Vector2(movable2.Peek().Value.X - 1, movable2.Peek().Value.Y)
                            );
                        movable.Add(new Vector2(movable2.Peek().Value.X - 1, movable2.Peek().Value.Y));
                    }
                if (movable2.Peek().Value.X < mapWidth - 1)
                    if ((movable2.Peek().Key +
                        map[(int)movable2.Peek().Value.Y, (int)movable2.Peek().Value.X + 1].moveCost) <= moves &&
                        !movable.Contains(new Vector2(movable2.Peek().Value.X + 1, movable2.Peek().Value.Y)))
                    {
                        movable2.Add(
                            movable2.Peek().Key +
                            map[(int)movable2.Peek().Value.Y, (int)movable2.Peek().Value.X + 1].moveCost,
                            new Vector2(movable2.Peek().Value.X + 1, movable2.Peek().Value.Y)
                            );
                        movable.Add(new Vector2(movable2.Peek().Value.X + 1, movable2.Peek().Value.Y));
                    }
                if (movable2.Peek().Value.Y > 0)

                    if ((movable2.Peek().Key +
                        map[(int)movable2.Peek().Value.Y - 1, (int)movable2.Peek().Value.X].moveCost) <= moves &&
                    !movable.Contains(new Vector2(movable2.Peek().Value.X, movable2.Peek().Value.Y - 1)))
                    {
                        movable2.Add(
                            movable2.Peek().Key +
                            map[(int)movable2.Peek().Value.Y - 1, (int)movable2.Peek().Value.X].moveCost,
                            new Vector2(movable2.Peek().Value.X, movable2.Peek().Value.Y - 1)
                            );
                        movable.Add(new Vector2(movable2.Peek().Value.X, movable2.Peek().Value.Y - 1));
                    }
                if (movable2.Peek().Value.Y < mapHeight - 1)
                    if ((movable2.Peek().Key +
                        map[(int)movable2.Peek().Value.Y + 1, (int)movable2.Peek().Value.X].moveCost) <= moves &&
                    !movable.Contains(new Vector2(movable2.Peek().Value.X, movable2.Peek().Value.Y + 1)))
                    {
                        movable2.Add(
                            movable2.Peek().Key +
                            map[(int)movable2.Peek().Value.Y + 1, (int)movable2.Peek().Value.X].moveCost,
                            new Vector2(movable2.Peek().Value.X, movable2.Peek().Value.Y + 1)
                            );
                        movable.Add(new Vector2(movable2.Peek().Value.X, movable2.Peek().Value.Y + 1));
                    }
                movable2.Remove();
            }
            return new PointDistance(-1, -1, -1);
        }
        public PointDistance checkNearestFortAI(Vector2 location, int within)
        {
            int moves = within;
            movable.Clear();
            movable2 = new PQ();
            movable.Add(location);

            movable2.Add(0, location);
            while (!movable2.empty())
            {
                
                if (map[(int)movable2.Peek().Value.Y, (int)movable2.Peek().Value.X].type == Tile.Type.Fort)
                {
                    return new PointDistance((int)movable2.Peek().Value.Y, (int)movable2.Peek().Value.X, (int)movable2.Peek().Key);
                }

                if (movable2.Peek().Value.X > 0)
                    if ((movable2.Peek().Key + map[(int)movable2.Peek().Value.Y, (int)movable2.Peek().Value.X - 1].moveCost) <= moves &&
                    !movable.Contains(new Vector2(movable2.Peek().Value.X - 1, movable2.Peek().Value.Y)))
                    {
                        movable2.Add(
                            movable2.Peek().Key +
                            map[(int)movable2.Peek().Value.Y, (int)movable2.Peek().Value.X - 1].moveCost,
                            new Vector2(movable2.Peek().Value.X - 1, movable2.Peek().Value.Y)
                            );
                        movable.Add(new Vector2(movable2.Peek().Value.X - 1, movable2.Peek().Value.Y));
                    }
                if (movable2.Peek().Value.X < mapWidth - 1)
                    if ((movable2.Peek().Key +
                        map[(int)movable2.Peek().Value.Y, (int)movable2.Peek().Value.X + 1].moveCost) <= moves &&
                        !movable.Contains(new Vector2(movable2.Peek().Value.X + 1, movable2.Peek().Value.Y)))
                    {
                        movable2.Add(
                            movable2.Peek().Key +
                            map[(int)movable2.Peek().Value.Y, (int)movable2.Peek().Value.X + 1].moveCost,
                            new Vector2(movable2.Peek().Value.X + 1, movable2.Peek().Value.Y)
                            );
                        movable.Add(new Vector2(movable2.Peek().Value.X + 1, movable2.Peek().Value.Y));
                    }
                if (movable2.Peek().Value.Y > 0)

                    if ((movable2.Peek().Key +
                        map[(int)movable2.Peek().Value.Y - 1, (int)movable2.Peek().Value.X].moveCost) <= moves &&
                    !movable.Contains(new Vector2(movable2.Peek().Value.X, movable2.Peek().Value.Y - 1)))
                    {
                        movable2.Add(
                            movable2.Peek().Key +
                            map[(int)movable2.Peek().Value.Y - 1, (int)movable2.Peek().Value.X].moveCost,
                            new Vector2(movable2.Peek().Value.X, movable2.Peek().Value.Y - 1)
                            );
                        movable.Add(new Vector2(movable2.Peek().Value.X, movable2.Peek().Value.Y - 1));
                    }
                if (movable2.Peek().Value.Y < mapHeight - 1)
                    if ((movable2.Peek().Key +
                        map[(int)movable2.Peek().Value.Y + 1, (int)movable2.Peek().Value.X].moveCost) <= moves &&
                    !movable.Contains(new Vector2(movable2.Peek().Value.X, movable2.Peek().Value.Y + 1)))
                    {
                        movable2.Add(
                            movable2.Peek().Key +
                            map[(int)movable2.Peek().Value.Y + 1, (int)movable2.Peek().Value.X].moveCost,
                            new Vector2(movable2.Peek().Value.X, movable2.Peek().Value.Y + 1)
                            );
                        movable.Add(new Vector2(movable2.Peek().Value.X, movable2.Peek().Value.Y + 1));
                    }
                movable2.Remove();
            }
            return new PointDistance(-1, -1, -1);
        }
        public InRange checkOpponentInRangeAI(Vector2 location, int within)
        {
            int moves = within;
            movable.Clear();
            movable2 = new PQ();
            movable.Add(location);
            InRange inRange;
            inRange = new InRange();
            inRange.players = new List<Point>();
            movable2.Add(0, location);
            while (!movable2.empty())
            {
                for (int y = 0; y <playerNumber; y++)
                {
                    if(players[0,y] != null)
                    if (players[0, y].location == movable2.Peek().Value)
                    {
                        
                        inRange.howMany++;

                        inRange.players.Add(new Point(0, y));
                    }
                }

                if (movable2.Peek().Value.X > 0)
                    if ((movable2.Peek().Key + map[(int)movable2.Peek().Value.Y, (int)movable2.Peek().Value.X - 1].moveCost) <= moves &&
                    !movable.Contains(new Vector2(movable2.Peek().Value.X - 1, movable2.Peek().Value.Y)))
                    {
                        movable2.Add(
                            movable2.Peek().Key +
                            map[(int)movable2.Peek().Value.Y, (int)movable2.Peek().Value.X - 1].moveCost,
                            new Vector2(movable2.Peek().Value.X - 1, movable2.Peek().Value.Y)
                            );
                        movable.Add(new Vector2(movable2.Peek().Value.X - 1, movable2.Peek().Value.Y));
                    }
                if (movable2.Peek().Value.X < mapWidth - 1)
                    if ((movable2.Peek().Key +
                        map[(int)movable2.Peek().Value.Y, (int)movable2.Peek().Value.X + 1].moveCost) <= moves &&
                        !movable.Contains(new Vector2(movable2.Peek().Value.X + 1, movable2.Peek().Value.Y)))
                    {
                        movable2.Add(
                            movable2.Peek().Key +
                            map[(int)movable2.Peek().Value.Y, (int)movable2.Peek().Value.X + 1].moveCost,
                            new Vector2(movable2.Peek().Value.X + 1, movable2.Peek().Value.Y)
                            );
                        movable.Add(new Vector2(movable2.Peek().Value.X + 1, movable2.Peek().Value.Y));
                    }
                if (movable2.Peek().Value.Y > 0)

                    if ((movable2.Peek().Key +
                        map[(int)movable2.Peek().Value.Y - 1, (int)movable2.Peek().Value.X].moveCost) <= moves &&
                    !movable.Contains(new Vector2(movable2.Peek().Value.X, movable2.Peek().Value.Y - 1)))
                    {
                        movable2.Add(
                            movable2.Peek().Key +
                            map[(int)movable2.Peek().Value.Y - 1, (int)movable2.Peek().Value.X].moveCost,
                            new Vector2(movable2.Peek().Value.X, movable2.Peek().Value.Y - 1)
                            );
                        movable.Add(new Vector2(movable2.Peek().Value.X, movable2.Peek().Value.Y - 1));
                    }
                if (movable2.Peek().Value.Y < mapHeight - 1)
                    if ((movable2.Peek().Key +
                        map[(int)movable2.Peek().Value.Y + 1, (int)movable2.Peek().Value.X].moveCost) <= moves &&
                    !movable.Contains(new Vector2(movable2.Peek().Value.X, movable2.Peek().Value.Y + 1)))
                    {
                        movable2.Add(
                            movable2.Peek().Key +
                            map[(int)movable2.Peek().Value.Y + 1, (int)movable2.Peek().Value.X].moveCost,
                            new Vector2(movable2.Peek().Value.X, movable2.Peek().Value.Y + 1)
                            );
                        movable.Add(new Vector2(movable2.Peek().Value.X, movable2.Peek().Value.Y + 1));
                    }
                movable2.Remove();
            }
            return inRange ;
        }
        public void fillShop()
        {
            shopInv.Add(weaponsetter.IronAxe());
            shopInv.Add(weaponsetter.IronLance());
            shopInv.Add(weaponsetter.IronSword());
            shopInv.Add(weaponsetter.SlimLance());
            shopInv.Add(weaponsetter.SlimSword());
            shopInv.Add(weaponsetter.SteelAxe());
            shopInv.Add(weaponsetter.SteelLance());
            shopInv.Add(weaponsetter.SteelSword());
        }
        public AIInfo AIPercieve(Vector2 location, int range)
        {
            AIInfo info = new AIInfo();
            info.nearestPlayer = checkNearestOpponentAI(location,  75);
            info.nearestFort = checkNearestFortAI(location, 20);
            info.playersInRange = checkOpponentInRangeAI(location, range);

            return info;
        }
        public void AIAttack(int y, Point target) 
        {
            playersInBattle[0] = new Vector2(1, y);
            playersInBattle[1] = new Vector2(target.X, target.Y);
            Battle();
        }
        public void AIMove(int y, Point destination)
        {
            players[1, y].location = new Vector2(destination.X, destination.Y);
        }
        public Point AIFindPositionToAttack(int y,Point target)
        {
            highlightMove(players[1, y].location, players[1, y].moves);
            if (movable.Contains(
                new Vector2(
                    players[target.X, target.Y].location.X + players[1, y].inv.FirstWeapon().range,
                    players[target.X, target.Y].location.Y )))
            {
                return new Point(
                    (int)players[target.X, target.Y].location.X + players[1, y].inv.FirstWeapon().range,
                    (int)players[target.X, target.Y].location.Y);
            }
            if (movable.Contains(
                new Vector2(
                    players[target.X, target.Y].location.X - players[1, y].inv.FirstWeapon().range,
                    players[target.X, target.Y].location.Y)))
            {
                return new Point(
                    (int)players[target.X, target.Y].location.X - players[1, y].inv.FirstWeapon().range,
                    (int)players[target.X, target.Y].location.Y);
            }
            if (movable.Contains(
                new Vector2(
                    players[target.X, target.Y].location.X,
                    players[target.X, target.Y].location.Y + players[1, y].inv.FirstWeapon().range)))
            {
                return new Point(
                    (int)players[target.X, target.Y].location.X,
                    (int)players[target.X, target.Y].location.Y + players[1, y].inv.FirstWeapon().range);
            }
            if (movable.Contains(
                new Vector2(
                    players[target.X, target.Y].location.X,
                    players[target.X, target.Y].location.Y - players[1, y].inv.FirstWeapon().range)))
            {
                return new Point(
                    (int)players[target.X, target.Y].location.X,
                    (int)players[target.X, target.Y].location.Y - players[1, y].inv.FirstWeapon().range);
            }
            return new Point(-100, -100);
        }
        public Point AIFindClosest(int y, Point target)
        {
            Point closest = new Point((int)players[1, y].location.X,(int) players[1, y].location.Y);
            
            foreach (Vector2 value in movable)
            {
                if (((closest.X - players[target.X, target.Y].location.X) * (closest.X - players[target.X, target.Y].location.X)
                    + (closest.Y - players[target.X, target.Y].location.Y) * (closest.Y - players[target.X, target.Y].location.Y)) >
                    ((value.X - players[target.X, target.Y].location.X) * (value.X - players[target.X, target.Y].location.X)
                    + (value.Y - players[target.X, target.Y].location.Y) * (value.Y - players[target.X, target.Y].location.Y)))
                {
                    closest = new Point((int)value.X,(int) value.Y);
                }
            }
            return closest;
        }
        public void Battle()
        {
            int dmg;
            int p1Acc = -100, p2Acc = -100, p1Avoid = 0, p2Avoid = 0;
            //bonus detection
            int p1Bonus = 0;
            int p1AccBonus = 0;
            players[(int)playersInBattle[0].X, (int)playersInBattle[0].Y].attacked = true;
            //determining weapon bonus
            if (null != players[(int)playersInBattle[0].X, (int)playersInBattle[0].Y].inv.FirstWeapon() && null != players[(int)playersInBattle[1].X, (int)playersInBattle[1].Y].inv.FirstWeapon())
            {
                if (players[(int)playersInBattle[0].X, (int)playersInBattle[0].Y].inv.FirstWeapon().weaponType == Weapon.triangle.axe)
                {
                    if (players[(int)playersInBattle[1].X, (int)playersInBattle[1].Y].inv.FirstWeapon().weaponType == Weapon.triangle.sword)
                    {
                        p1Bonus = -1;
                        p1AccBonus = -15;
                    }
                    else if (players[(int)playersInBattle[0].X, (int)playersInBattle[0].Y].inv.FirstWeapon().weaponType == Weapon.triangle.lance)
                    {
                        p1Bonus = 1;
                        p1AccBonus = 15;
                    }
                }
                else if (players[(int)playersInBattle[0].X, (int)playersInBattle[0].Y].inv.FirstWeapon().weaponType == Weapon.triangle.lance)
                {
                    if (players[(int)playersInBattle[1].X, (int)playersInBattle[1].Y].inv.FirstWeapon().weaponType == Weapon.triangle.sword)
                    {
                        p1Bonus = 1;
                        p1AccBonus = 15;
                    }
                    else if (players[(int)playersInBattle[0].X, (int)playersInBattle[0].Y].inv.FirstWeapon().weaponType == Weapon.triangle.axe)
                    {
                        p1Bonus = -1;
                        p1AccBonus = -15;
                    }
                }
                else if (players[(int)playersInBattle[0].X, (int)playersInBattle[0].Y].inv.FirstWeapon().weaponType == Weapon.triangle.sword)
                {
                    if (players[(int)playersInBattle[1].X, (int)playersInBattle[1].Y].inv.FirstWeapon().weaponType == Weapon.triangle.axe)
                    {
                        p1Bonus = 1;
                        p1AccBonus = 15;
                    }
                    else if (players[(int)playersInBattle[0].X, (int)playersInBattle[0].Y].inv.FirstWeapon().weaponType == Weapon.triangle.lance)
                    {
                        p1Bonus = -1;
                        p1Bonus = -15;
                    }
                }
            }
            //accuracy calculations
            if (null != players[(int)playersInBattle[0].X, (int)playersInBattle[0].Y].inv.FirstWeapon())
                p1Acc = players[(int)playersInBattle[0].X, (int)playersInBattle[0].Y].inv.FirstWeapon().hit +
                        (players[(int)playersInBattle[0].X, (int)playersInBattle[0].Y].skl * 2) +
                        (players[(int)playersInBattle[0].X, (int)playersInBattle[0].Y].luk / 2) +
                        p1AccBonus;
            if (null != players[(int)playersInBattle[1].X, (int)playersInBattle[1].Y].inv.FirstWeapon())
                p2Acc = players[(int)playersInBattle[1].X, (int)playersInBattle[1].Y].inv.FirstWeapon().hit +
                        (players[(int)playersInBattle[1].X, (int)playersInBattle[1].Y].skl * 2) +
                        (players[(int)playersInBattle[1].X, (int)playersInBattle[1].Y].luk / 2) -
                        p1AccBonus;
            p1Avoid = (players[(int)playersInBattle[0].X, (int)playersInBattle[0].Y].spd * 2) +
                (players[(int)playersInBattle[0].X, (int)playersInBattle[0].Y].luk) + map[(int)playersInBattle[0].Y, (int)playersInBattle[0].X].avoid;
            p2Avoid = (players[(int)playersInBattle[1].X, (int)playersInBattle[1].Y].spd * 2) +
               (players[(int)playersInBattle[1].X, (int)playersInBattle[1].Y].luk) + map[(int)playersInBattle[1].Y, (int)playersInBattle[1].X].avoid;


            //first player is faster or as fast as other player
            if (players[(int)playersInBattle[0].X, (int)playersInBattle[0].Y].getSpeed() >= players[(int)playersInBattle[1].X, (int)playersInBattle[1].Y].getSpeed())
            {
                if ((p1Acc - p2Avoid) > RandomNext(0, 99))
                {
                    //figure out the damage
                    dmg = players[(int)playersInBattle[0].X, (int)playersInBattle[0].Y].str +
                        players[(int)playersInBattle[0].X, (int)playersInBattle[0].Y].inv.FirstWeapon().mt +
                        p1Bonus;
                    dmg = dmg - (players[(int)playersInBattle[1].X, (int)playersInBattle[1].Y].def +
                            map[
                                (int)players[(int)playersInBattle[0].X, (int)playersInBattle[0].Y].location.Y,
                                (int)players[(int)playersInBattle[0].X, (int)playersInBattle[0].Y].location.X
                            ].def);
                    //increment the health
                    players[(int)playersInBattle[1].X, (int)playersInBattle[1].Y].curhp -= dmg;
                    //check if dead
                    if (players[(int)playersInBattle[1].X, (int)playersInBattle[1].Y].curhp <= 0)
                    {
                        players[(int)playersInBattle[1].X, (int)playersInBattle[1].Y].location = emptyVector;
                        players[(int)playersInBattle[0].X, (int)playersInBattle[0].Y].exp +=
                            ((31
                            + players[(int)playersInBattle[1].X, (int)playersInBattle[1].Y].level
                            - players[(int)playersInBattle[0].X, (int)playersInBattle[0].Y].level) / 3) + 20;
                        money[playerTurn - 1] += 100;
                        if (players[(int)playersInBattle[1].X, (int)playersInBattle[1].Y].CLASS == Player.CLIST.Thief)
                            players[(int)playersInBattle[0].X, (int)playersInBattle[0].Y].exp += 20;
                        boardScene = BoardScene.Normal;
                        return;
                    }
                    players[(int)playersInBattle[0].X, (int)playersInBattle[0].Y].exp +=
                        ((31
                        + players[(int)playersInBattle[1].X, (int)playersInBattle[1].Y].level
                        - players[(int)playersInBattle[0].X, (int)playersInBattle[0].Y].level) / 3);
                }
                else players[(int)playersInBattle[0].X, (int)playersInBattle[0].Y].exp++;
                if ((p2Acc - p1Avoid) > RandomNext(0, 99))
                {
                    dmg = players[(int)playersInBattle[1].X, (int)playersInBattle[1].Y].str +
                        players[(int)playersInBattle[1].X, (int)playersInBattle[1].Y].inv.FirstWeapon().mt -
                        p1Bonus;
                    dmg = dmg - (players[(int)playersInBattle[0].X, (int)playersInBattle[0].Y].def +
                            map[
                                (int)players[(int)playersInBattle[0].X, (int)playersInBattle[0].Y].location.Y,
                                (int)players[(int)playersInBattle[0].X, (int)playersInBattle[0].Y].location.X
                            ].def);
                    players[(int)playersInBattle[0].X, (int)playersInBattle[0].Y].curhp -= dmg;
                    if (players[(int)playersInBattle[0].X, (int)playersInBattle[0].Y].curhp <= 0)
                    {
                        players[(int)playersInBattle[0].X, (int)playersInBattle[0].Y].location = emptyVector;
                        players[(int)playersInBattle[1].X, (int)playersInBattle[1].Y].exp +=
                            ((31
                            + players[(int)playersInBattle[0].X, (int)playersInBattle[0].Y].level
                            - players[(int)playersInBattle[1].X, (int)playersInBattle[1].Y].level) / 3) + 20;
                        if (players[(int)playersInBattle[0].X, (int)playersInBattle[0].Y].CLASS == Player.CLIST.Thief)
                            players[(int)playersInBattle[1].X, (int)playersInBattle[1].Y].exp += 20;
                        money[otherTurn() - 1] += 100;
                        boardScene = BoardScene.Normal;
                        return;
                    }
                    players[(int)playersInBattle[1].X, (int)playersInBattle[1].Y].exp +=
                        ((31
                        + players[(int)playersInBattle[0].X, (int)playersInBattle[0].Y].level
                        - players[(int)playersInBattle[1].X, (int)playersInBattle[1].Y].level) / 3);
                }
                else players[(int)playersInBattle[1].X, (int)playersInBattle[1].Y].exp++;


                boardScene = BoardScene.Normal;
                return;

            }
            else
            {
                if ((p2Acc - p1Avoid) > RandomNext(0, 99))
                {
                    dmg = players[(int)playersInBattle[1].X, (int)playersInBattle[1].Y].str +
                        players[(int)playersInBattle[1].X, (int)playersInBattle[1].Y].inv.FirstWeapon().mt -
                        p1Bonus;
                    dmg = dmg - (players[(int)playersInBattle[0].X, (int)playersInBattle[0].Y].def +
                        map[
                            (int)players[(int)playersInBattle[0].X, (int)playersInBattle[0].Y].location.Y,
                            (int)players[(int)playersInBattle[0].X, (int)playersInBattle[0].Y].location.X
                        ].def);
                    players[(int)playersInBattle[0].X, (int)playersInBattle[0].Y].curhp -= dmg;
                    if (players[(int)playersInBattle[0].X, (int)playersInBattle[0].Y].curhp <= 0)
                    {
                        players[(int)playersInBattle[0].X, (int)playersInBattle[0].Y].location = emptyVector;
                        players[(int)playersInBattle[1].X, (int)playersInBattle[1].Y].exp +=
                            ((31
                            + players[(int)playersInBattle[0].X, (int)playersInBattle[0].Y].level
                            - players[(int)playersInBattle[1].X, (int)playersInBattle[1].Y].level) / 3) + 20;
                        if (players[(int)playersInBattle[0].X, (int)playersInBattle[0].Y].CLASS == Player.CLIST.Thief)
                            players[(int)playersInBattle[1].X, (int)playersInBattle[1].Y].exp += 20;
                        money[otherTurn() - 1] += 100;
                        boardScene = BoardScene.Normal;
                        return;
                    }
                    players[(int)playersInBattle[1].X, (int)playersInBattle[1].Y].exp +=
                        ((31
                        + players[(int)playersInBattle[0].X, (int)playersInBattle[0].Y].level
                        - players[(int)playersInBattle[1].X, (int)playersInBattle[1].Y].level) / 3);
                }
                else players[(int)playersInBattle[1].X, (int)playersInBattle[1].Y].exp++;
                if ((p1Acc - p2Avoid) > RandomNext(0, 99))
                {
                    dmg = players[(int)playersInBattle[0].X, (int)playersInBattle[0].Y].str +
                        players[(int)playersInBattle[0].X, (int)playersInBattle[0].Y].inv.FirstWeapon().mt +
                        p1Bonus;
                    dmg = dmg - (players[(int)playersInBattle[1].X, (int)playersInBattle[1].Y].def +
                        map[
                            (int)players[(int)playersInBattle[1].X, (int)playersInBattle[1].Y].location.Y,
                            (int)players[(int)playersInBattle[1].X, (int)playersInBattle[1].Y].location.X
                        ].def);
                    players[(int)playersInBattle[1].X, (int)playersInBattle[1].Y].curhp -= dmg;
                    if (players[(int)playersInBattle[1].X, (int)playersInBattle[1].Y].curhp <= 0)
                    {
                        players[(int)playersInBattle[1].X, (int)playersInBattle[1].Y].location = emptyVector;
                        players[(int)playersInBattle[0].X, (int)playersInBattle[0].Y].exp +=
                            ((31
                            + players[(int)playersInBattle[1].X, (int)playersInBattle[1].Y].level
                            - players[(int)playersInBattle[0].X, (int)playersInBattle[0].Y].level) / 3) + 20;
                        if (players[(int)playersInBattle[1].X, (int)playersInBattle[1].Y].CLASS == Player.CLIST.Thief)
                            players[(int)playersInBattle[0].X, (int)playersInBattle[0].Y].exp += 20;
                        money[playerTurn - 1] += 100;
                        boardScene = BoardScene.Normal;
                        return;
                    }
                    players[(int)playersInBattle[0].X, (int)playersInBattle[0].Y].exp +=
                        ((31
                        + players[(int)playersInBattle[1].X, (int)playersInBattle[1].Y].level
                        - players[(int)playersInBattle[0].X, (int)playersInBattle[0].Y].level) / 3);
                }
                else players[(int)playersInBattle[0].X, (int)playersInBattle[0].Y].exp++;


                boardScene = BoardScene.Normal;
                return;

            }
        }
        public void MapReset()
        {
            foreach (Tile value in map)
            {
                value.highlighted = false;
                value.highlightedRed = false;
            }
        }
        public static void SaveGame(SaveData saveData, string filename)
        {
            if (File.Exists(filename))
                File.Delete(filename);
            RijndaelManaged RMCrypto = new RijndaelManaged();
            FileStream stream = File.Open(filename, FileMode.Create);
            UnicodeEncoding aUE = new UnicodeEncoding();
            byte[] key = aUE.GetBytes("123Miles");
            CryptoStream streama = new CryptoStream(stream, RMCrypto.CreateEncryptor(key, key), CryptoStreamMode.Write);
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(SaveData));
                serializer.Serialize(streama, saveData);
            }
            catch (InvalidOperationException e)
            {
                e.ToString();
            }
            finally
            {
                streama.Close();
                stream.Close();
            }
        }
        public static SaveData LoadGame(string filename)
        {
            SaveData sd = new SaveData();
            FileStream aFileStream = new FileStream(filename, FileMode.Open);
            UnicodeEncoding aUE = new UnicodeEncoding();
            byte[] key = aUE.GetBytes("123Miles");
            RijndaelManaged RMCrypto = new RijndaelManaged();
            CryptoStream aCryptoStream = new CryptoStream(aFileStream, RMCrypto.CreateDecryptor(key, key), CryptoStreamMode.Read);
            XmlSerializer xmlSer = new XmlSerializer(typeof(SaveData));
            try
            {
                sd = (SaveData)xmlSer.Deserialize(aCryptoStream);
            }
            finally
            {
                aCryptoStream.Close();
                aFileStream.Close();
            }
            return sd;
        }
    }
}
