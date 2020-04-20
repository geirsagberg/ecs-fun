using System;
using EcsFun.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Input.InputListeners;
using MonoGame.Extended.Sprites;
using Myra;
using Myra.Graphics2D.UI;

namespace EcsFun
{
    public class EcsGame : Game
    {
        private readonly GraphicsDeviceManager graphics;
        private SpriteBatch? spriteBatch;
        private RenderSystem? renderSystem;
        private World? world;
        private SpawnSystem? spawnSystem;
        private readonly Random random;
        private MouseListener mouseListener;
        private ControlSystem controlSystem;
        private SharedState sharedState;

        public EcsGame()
        {
            graphics = new GraphicsDeviceManager(this) {
                PreferredBackBufferWidth = 1280,
                PreferredBackBufferHeight = 800
            };
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
            random = new Random();
            Window.ClientSizeChanged += (sender, args) => {
                graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
                graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
                graphics.ApplyChanges();
            };
        }

        protected override void Initialize()
        {
            mouseListener = new MouseListener();
            Components.Add(new InputListenerComponent(this, mouseListener));

            base.Initialize();
        }

        protected override void LoadContent()
        {
            MyraEnvironment.Game = this;

            spriteBatch = new SpriteBatch(GraphicsDevice);

            sharedState = new SharedState();

            var sprites = new Sprites(new Sprite(Content.Load<Texture2D>("anki")),
                new Sprite(Content.Load<Texture2D>("bronch")));

            spawnSystem = new SpawnSystem(sharedState);
            renderSystem = new RenderSystem(sharedState, spriteBatch, sprites);
            controlSystem = new ControlSystem(sharedState, mouseListener, random, graphics, sprites);
            var playerSystem = new PlayerSystem();
            var physicsSystem = new PhysicsSystem(graphics);

            world = new WorldBuilder()
                .AddSystem(controlSystem)
                .AddSystem(spawnSystem)
                .AddSystem(playerSystem)
                .AddSystem(physicsSystem)
                .AddSystem(renderSystem)
                .Build();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            world?.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            world?.Draw(gameTime);
            Desktop.Render();

            base.Draw(gameTime);
        }
    }

    public class Sprites
    {
        public Sprites(Sprite anki, Sprite bronch)
        {
            Anki = anki;
            Bronch = bronch;
        }

        public Sprite Anki { get; }
        public Sprite Bronch { get; }
    }
}