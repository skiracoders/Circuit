using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Circuit
{
    public class MonoGameBackend : Game
    {
        private Texture2D texture;
        private Texture2D unitTexture;
        private GraphicsDeviceManager graphics;
        private GraphicsDevice device;
        private SpriteBatch spriteBatch;
        private SpriteFont font;
        public MonoGameBackend()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {

            // TODO: Add your initialization logic here
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 512;
            graphics.ApplyChanges();
            device = graphics.GraphicsDevice;
            texture = new Texture2D(device, 1024, 512, false, SurfaceFormat.Color);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = this.Content.Load<SpriteFont>("ShareTechMono");
            unitTexture = this.Content.Load<Texture2D>("Unit0x1");
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            /*
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            */
            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null);
            spriteBatch.Draw(unitTexture, new Vector2(512, 256), Color.White);
            spriteBatch.DrawString(font, "Hello Circuit", new Vector2(0, 0), Color.White);
            spriteBatch.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
