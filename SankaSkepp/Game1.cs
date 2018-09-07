using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using System.Diagnostics;

namespace SankaSkepp
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D missTexture, hitTexture, boatTexture, boatFTexture, boatNTexture;
        SpriteFont font;

        Vector2 screenSize, menuSize;
        GridHandler grid;
        List<Vector2> boatPlace;
        ButtonState prevMouseState;
        KeyboardState prevKeyState;
        BoatHandler playerBoats;
        ComputerPlayer computerBoats;
        enum State : Byte { Start, Place, Player, Computer, End };
        State currState;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            screenSize = new Vector2(800, 600);
            menuSize = new Vector2(200, 0);

            graphics.PreferredBackBufferHeight = (int)screenSize.Y;
            graphics.PreferredBackBufferWidth = (int)screenSize.X;
            graphics.ApplyChanges();

            grid = new GridHandler(graphics, 10, 10, Window.ClientBounds.Width - (int)menuSize.X, Window.ClientBounds.Height - (int)menuSize.Y);
            boatPlace = new List<Vector2>();
            currState = 0;

            IsMouseVisible = true;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            missTexture = Content.Load<Texture2D>("ring");
            hitTexture = Content.Load<Texture2D>("explosion");
            boatTexture = Content.Load<Texture2D>("boat");
            boatFTexture = Content.Load<Texture2D>("boat_front");
            boatNTexture = Content.Load<Texture2D>("boat_nil");
            font = Content.Load<SpriteFont>("font");

            playerBoats = new BoatHandler(boatFTexture, boatTexture, boatNTexture, missTexture, hitTexture, grid.Cell);
            computerBoats = new ComputerPlayer(boatFTexture, boatTexture, boatNTexture, missTexture, hitTexture, grid.Cell);
        }

        protected override void UnloadContent()
        {
            
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (currState == State.Start)
            {
                KeyboardState keys = Keyboard.GetState();

                if (keys.IsKeyDown(Keys.Space) && prevKeyState.IsKeyUp(Keys.Space))
                {
                    grid.UpdateGrid();
                    playerBoats.UpdateCell(grid.Cell);
                    computerBoats.UpdateCell(grid.Cell);
                    currState = State.Place;
                } 
                
                if (keys.IsKeyDown(Keys.Up) && prevKeyState.IsKeyUp(Keys.Up))
                {
                    grid.NumHorizontal++;
                    grid.NumVertical++;
                } 
                else if (keys.IsKeyDown(Keys.Down) && prevKeyState.IsKeyUp(Keys.Down))
                {
                    grid.NumHorizontal--;
                    grid.NumVertical--;
                }

                if (keys.IsKeyDown(Keys.Tab) && prevKeyState.IsKeyUp(Keys.Tab))
                {
                    computerBoats.LimitOn = !computerBoats.LimitOn;
                    playerBoats.LimitOn = !playerBoats.LimitOn;
                }

                prevKeyState = keys;
            }
            else if (currState == State.Place)
            {
                var mouse = Mouse.GetState();
                KeyboardState keys = Keyboard.GetState();

                if (keys.IsKeyDown(Keys.Space) && prevKeyState.IsKeyUp(Keys.Space) && playerBoats.Sum == 16)
                {
                    computerBoats.RandomLayout(grid.NumVertical, grid.NumHorizontal);
                    currState = State.Player;
                }
                prevKeyState = keys;

                if (mouse.LeftButton == ButtonState.Pressed && prevMouseState == ButtonState.Released)
                {
                    Vector2 loc = grid.WichCell(mouse.X, mouse.Y);
                    if (!boatPlace.Contains(loc) && loc.X != -1)
                    {
                        boatPlace.Add(loc);
                    }
                }
                prevMouseState = mouse.LeftButton;

                if (boatPlace.Count == 1 && grid.Screen.Contains(mouse.X, mouse.Y))
                {
                    playerBoats.UpdateTemp(boatPlace[0], grid.WichCell(mouse.X, mouse.Y));
                }
                else if (boatPlace.Count == 2 && (boatPlace[0].X == boatPlace[1].X ^ boatPlace[0].Y == boatPlace[1].Y))
                {
                    playerBoats.Place(boatPlace);
                    boatPlace.Clear();
                }
            }
            else if (currState == State.Player)
            {
                computerBoats.HasFired = false;

                var mouse = Mouse.GetState();
                KeyboardState keys = Keyboard.GetState();

                if (mouse.LeftButton == ButtonState.Pressed && prevMouseState == ButtonState.Released && !playerBoats.HasFired)
                {
                    Vector2 loc = grid.WichCell(mouse.X, mouse.Y);
                    playerBoats.HasFired = computerBoats.FireAt(loc);
                }
                prevMouseState = mouse.LeftButton;

                if (keys.IsKeyDown(Keys.Space) && prevKeyState.IsKeyUp(Keys.Space) && playerBoats.HasFired)
                {
                    currState = State.Computer;
                    if (!playerBoats.Alive || !computerBoats.Alive)
                        currState = State.End;
                }

                prevKeyState = keys;
            }
            else if (currState == State.Computer)
            {
                playerBoats.HasFired = false;
                KeyboardState keys = Keyboard.GetState();

                if (!computerBoats.HasFired)
                    computerBoats.HasFired = computerBoats.Fire(playerBoats, grid.NumVertical, grid.NumHorizontal);

                if (keys.IsKeyDown(Keys.Space) && prevKeyState.IsKeyUp(Keys.Space) && computerBoats.HasFired)
                {
                    currState = State.Player;
                    if (!playerBoats.Alive || !computerBoats.Alive)
                        currState = State.End;
                }
                
                prevKeyState = keys;
            }
            else if (currState == State.End)
            {
                KeyboardState keys = Keyboard.GetState();

                if (keys.IsKeyDown(Keys.Space) && prevKeyState.IsKeyUp(Keys.Space))
                {
                    playerBoats.Reset();
                    computerBoats.Reset();
                    currState = State.Start;
                }  

                prevKeyState = keys;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkBlue);

            spriteBatch.Begin();
            Vector2 textSize, textPos = Vector2.Zero;
            string text;
            float textScale;

            if (currState == State.Start)
            {
                text = "Sanka skepp";
                textSize = font.MeasureString(text);
                textPos.X = (Window.ClientBounds.Width - textSize.X) / 2;
                textPos.Y = 10;

                spriteBatch.DrawString(
                    font, 
                    text, 
                    textPos, 
                    Color.White,
                    0f,
                    Vector2.Zero,
                    Vector2.One,
                    SpriteEffects.None,
                    0f
                );

                textScale = 1.5f;

                text = "Grid Size " + grid.NumVertical + "x" + grid.NumHorizontal;
                textSize = font.MeasureString(text) / textScale;
                textPos.X = (Window.ClientBounds.Width - textSize.X) / 2;
                textPos.Y += 140;

                spriteBatch.DrawString(
                    font,
                    text,
                    textPos,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    Vector2.One / textScale,
                    SpriteEffects.None,
                    0f
                );

                text = "Game mode: " + (playerBoats.LimitOn ? "Standard" : "Lego");
                textSize = font.MeasureString(text) / textScale;
                textPos.X = (Window.ClientBounds.Width - textSize.X) / 2;
                textPos.Y += 30;

                spriteBatch.DrawString(
                    font,
                    text,
                    textPos,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    Vector2.One / textScale,
                    SpriteEffects.None,
                    0f
                );

                textScale = 2f;

                text = (playerBoats.LimitOn ? "Each player has 2 ships of size 2 and one each of size 3-5" : "Each player has 16 ship pieces to freely construct ships");
                textSize = font.MeasureString(text) / textScale;
                textPos.X = (Window.ClientBounds.Width - textSize.X) / 2;
                textPos.Y += 40;

                spriteBatch.DrawString(
                    font,
                    text,
                    textPos,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    Vector2.One / textScale,
                    SpriteEffects.None,
                    0f
                );

                textScale = 1.5f;

                text = "Use up & down keys to edit grid size";
                textSize = font.MeasureString(text) / textScale;
                textPos.X = (Window.ClientBounds.Width - textSize.X) / 2;
                textPos.Y += 60;

                spriteBatch.DrawString(
                    font,
                    text,
                    textPos,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    Vector2.One / textScale,
                    SpriteEffects.None,
                    0f
                );

                text = "Use tab to toggle game mode";
                textSize = font.MeasureString(text) / textScale;
                textPos.X = (Window.ClientBounds.Width - textSize.X) / 2;
                textPos.Y += 30;

                spriteBatch.DrawString(
                    font,
                    text,
                    textPos,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    Vector2.One / textScale,
                    SpriteEffects.None,
                    0f
                );

                text = "Press space to continue";
                textSize = font.MeasureString(text) / textScale;
                textPos.X = (Window.ClientBounds.Width - textSize.X) / 2;
                textPos.Y += 30;

                spriteBatch.DrawString(
                    font,
                    text,
                    textPos,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    Vector2.One / textScale,
                    SpriteEffects.None,
                    0f
                );
            }
            else if (currState == State.Place)
            {
                grid.Draw(spriteBatch);
                playerBoats.DrawShips(spriteBatch);
                if (boatPlace.Count == 1)
                {
                    playerBoats.DrawTemp(spriteBatch);
                }

                if (playerBoats.Sum < 16)
                {
                    textScale = 2f;

                    text = "Please place your boats";
                    textSize = font.MeasureString(text) / textScale;
                    textPos.X = grid.Screen.Width + (((Window.ClientBounds.Width - grid.Screen.Width) - textSize.X) / 2);
                    textPos.Y = 180;

                    spriteBatch.DrawString(
                        font,
                        text,
                        textPos,
                        Color.White,
                        0f,
                        Vector2.Zero,
                        Vector2.One / textScale,
                        SpriteEffects.None,
                        0f
                    );

                    text = "by clicking on the grid";
                    textSize = font.MeasureString(text) / textScale;
                    textPos.X = grid.Screen.Width + (((Window.ClientBounds.Width - grid.Screen.Width) - textSize.X) / 2);
                    textPos.Y += 15;

                    spriteBatch.DrawString(
                        font,
                        text,
                        textPos,
                        Color.White,
                        0f,
                        Vector2.Zero,
                        Vector2.One / textScale,
                        SpriteEffects.None,
                        0f
                    );
                }
                else
                {
                    textScale = 2f;

                    text = "Can't place more boats";
                    textSize = font.MeasureString(text) / textScale;
                    textPos.X = grid.Screen.Width + (((Window.ClientBounds.Width - grid.Screen.Width) - textSize.X) / 2);
                    textPos.Y = 180;

                    spriteBatch.DrawString(
                        font,
                        text,
                        textPos,
                        Color.Red,
                        0f,
                        Vector2.Zero,
                        Vector2.One / textScale,
                        SpriteEffects.None,
                        0f
                    );

                    text = "continue by pressing space";
                    textSize = font.MeasureString(text) / textScale;
                    textPos.X = grid.Screen.Width + (((Window.ClientBounds.Width - grid.Screen.Width) - textSize.X) / 2);
                    textPos.Y += 15;

                    spriteBatch.DrawString(
                        font,
                        text,
                        textPos,
                        Color.Red,
                        0f,
                        Vector2.Zero,
                        Vector2.One / textScale,
                        SpriteEffects.None,
                        0f
                    );
                }
            }
            else if (currState == State.Player)
            {
                grid.Draw(spriteBatch);
                computerBoats.DrawHits(spriteBatch);

                if (playerBoats.HasFired)
                {
                    text = "continue by pressing space";
                    textScale = 2f;
                    textSize = font.MeasureString(text) / textScale;
                    textPos.X = grid.Screen.Width + (((Window.ClientBounds.Width - grid.Screen.Width) - textSize.X) / 2);
                    textPos.Y = 180;

                    spriteBatch.DrawString(
                        font,
                        text,
                        textPos,
                        Color.Red,
                        0f,
                        Vector2.Zero,
                        Vector2.One / textScale,
                        SpriteEffects.None,
                        0f
                    );
                }
                else
                {
                    text = "Click To fire";
                    textScale = 2f;
                    textSize = font.MeasureString(text) / textScale;
                    textPos.X = grid.Screen.Width + (((Window.ClientBounds.Width - grid.Screen.Width) - textSize.X) / 2);
                    textPos.Y = 180;

                    spriteBatch.DrawString(
                        font,
                        text,
                        textPos,
                        Color.White,
                        0f,
                        Vector2.Zero,
                        Vector2.One / textScale,
                        SpriteEffects.None,
                        0f
                    );
                }
            }
            else if (currState == State.Computer)
            {
                grid.Draw(spriteBatch);
                playerBoats.DrawHits(spriteBatch);

                textScale = 2f;

                text = "The computer has played";
                textSize = font.MeasureString(text) / textScale;
                textPos.X = grid.Screen.Width + (((Window.ClientBounds.Width - grid.Screen.Width) - textSize.X) / 2);
                textPos.Y = 180;

                spriteBatch.DrawString(
                    font,
                    text,
                    textPos,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    Vector2.One / textScale,
                    SpriteEffects.None,
                    0f
                );

                text = "continue by pressing space";
                textSize = font.MeasureString(text) / textScale;
                textPos.X = grid.Screen.Width + (((Window.ClientBounds.Width - grid.Screen.Width) - textSize.X) / 2);
                textPos.Y += 15;

                spriteBatch.DrawString(
                    font,
                    text,
                    textPos,
                    Color.Red,
                    0f,
                    Vector2.Zero,
                    Vector2.One / textScale,
                    SpriteEffects.None,
                    0f
                );
            }
            else if (currState == State.End)
            {
                text = "Sanka skepp";
                textSize = font.MeasureString(text);
                textPos.X = (Window.ClientBounds.Width - textSize.X) / 2;
                textPos.Y = 10;

                spriteBatch.DrawString(
                    font,
                    text,
                    textPos,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    Vector2.One,
                    SpriteEffects.None,
                    0f
                );

                textScale = 1.5f;

                text = playerBoats.Alive ? "You Win!!!" : "You Lost...";
                textSize = font.MeasureString(text) / textScale;
                textPos.X = (Window.ClientBounds.Width - textSize.X) / 2;
                textPos.Y += 140;

                spriteBatch.DrawString(
                    font,
                    text,
                    textPos,
                    playerBoats.Alive ? Color.Green : Color.Red,
                    0f,
                    Vector2.Zero,
                    Vector2.One / textScale,
                    SpriteEffects.None,
                    0f
                );

                text = "Press space to restart";
                textSize = font.MeasureString(text) / textScale;
                textPos.X = (Window.ClientBounds.Width - textSize.X) / 2;
                textPos.Y += 30;

                spriteBatch.DrawString(
                    font,
                    text,
                    textPos,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    Vector2.One / textScale,
                    SpriteEffects.None,
                    0f
                );
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
