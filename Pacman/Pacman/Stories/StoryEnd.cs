using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Pacman.Stories
{
	public class StoryEnd : Story
	{
		private SpriteFont _font;
		private SoundEffect _soundMenu;
		private SoundEffectInstance _soundMenuInstance;
		private SpriteBatch _spriteBatch;
		public int Score { get; set; }

		public StoryEnd(GraphicsDevice graphicsDevice, ContentManager content)
			: base(graphicsDevice, content) {}

		public override void LoadContent()
		{
			_spriteBatch = new SpriteBatch(GraphicsDevice);
			_font = Content.Load<SpriteFont>("gameFont");
			_soundMenu = Content.Load<SoundEffect>("pacman_finish");
			_soundMenuInstance = _soundMenu.CreateInstance();
		}

		public override void UnloadContent()
		{
			_soundMenuInstance.Dispose();
			_soundMenu.Dispose();
			_spriteBatch.Dispose();
		}

		public override void Start()
		{
			IsLoaded = true;
		}

		public override void Stop()
		{
			_soundMenuInstance.Stop();
		}

		public override void Update(GameTime gameTime)
		{
			if (IsKeyPressed(Keys.Escape) || IsKeyPressed(Keys.Enter) || IsKeyPressed(Keys.Space))
			{
				StoreKeyboardState();
				SwitchStory(PacmanGame.StoryMenuName);
				return;
			}
			StoreKeyboardState();

			if (_soundMenuInstance.State == SoundState.Stopped)
			{
				_soundMenuInstance.Play();
			}
		}

		public override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.Black);
			_spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

			const string yourScoreIs = "Your score is";
			var start = _font.MeasureString(yourScoreIs);
			_spriteBatch.DrawString(_font, yourScoreIs,
				new Vector2(GraphicsDevice.Viewport.Width / 2f, GraphicsDevice.Viewport.Height / 2f - 20f),
				Color.Yellow, 0.0f,
				new Vector2(start.X / 2, start.Y / 2),
				1.0f, SpriteEffects.None, 0.0f);

			var score = Score.ToString();
			start = _font.MeasureString(score);
			_spriteBatch.DrawString(_font, score,
				new Vector2(GraphicsDevice.Viewport.Width / 2f, GraphicsDevice.Viewport.Height /2f + 20f),
				Color.Yellow, 0.0f,
				new Vector2(start.X / 2, start.Y / 2 + 10),
				1.0f, SpriteEffects.None, 0.0f);

			_spriteBatch.End();
		}
	}
}
