using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Linq;

namespace Pacman.Stories
{
	public class StoryMenu : Story
	{
		private float _totalElapsed;
		private const float Fps = .75f;
		private const float Tpf = 1 / Fps;
		
		private static readonly Color AuthorColor = new Color(24, 24, 255);
		private SoundEffect _soundMenu;
		private SoundEffectInstance _soundMenuInstance;
		private SpriteBatch _spriteBatch;
		private SpriteFont _font;
		private SpriteFont _gameFont;

		public StoryMenu(GraphicsDevice graphicsDevice, ContentManager content)
			: base(graphicsDevice, content) {}

		public override void LoadContent()
		{
			_soundMenu = Content.Load<SoundEffect>("pacman_finish");
			_soundMenuInstance = _soundMenu.CreateInstance();
			_spriteBatch = new SpriteBatch(GraphicsDevice);
			_font = Content.Load<SpriteFont>("pacman_font");
			_gameFont = Content.Load<SpriteFont>("gameFont");
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
			if (IsKeyPressed(Keys.Escape))
			{
				StoreKeyboardState();
				SwitchStory(null);
				return;
			}
			if (Keyboard.GetState(PlayerIndex.One).GetPressedKeys().Length > 0)
			{
				StoreKeyboardState();
				SwitchStory(PacmanGame.StoryGameName);
				return;
			}

			if (_soundMenuInstance.State == SoundState.Stopped)
			{
				_soundMenuInstance.Play();
			}

			var elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
			_totalElapsed += elapsed;
			if (_totalElapsed <= Tpf)
				return;

			_totalElapsed -= Tpf;
		}

		public override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.Black);
			_spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

			const string pacman = "PAC - MAN";
			var start = _font.MeasureString(pacman);
			_spriteBatch.DrawString(_font, pacman,
				new Vector2((GraphicsDevice.Viewport.Width / 2) + 2, 50),
				Color.Yellow, 0.0f,
				new Vector2(start.X / 2, start.Y / 2),
				1.0f, SpriteEffects.None, 0.0f);

			if (_totalElapsed < Tpf / 2)
			{
				const string pressAnyKey = "Press any key to start...";
				start = _gameFont.MeasureString(pressAnyKey);
				_spriteBatch.DrawString(_gameFont, pressAnyKey,
					new Vector2(GraphicsDevice.Viewport.Width / 2f, GraphicsDevice.Viewport.Height / 2f),
					Color.Yellow, 0.0f,
					new Vector2(start.X / 2, start.Y / 2),
					1.0f, SpriteEffects.None, 0.0f);
			}

			const string author = "Pawel Wasilewski (97918)";
			start = _gameFont.MeasureString(author);
			_spriteBatch.DrawString(_gameFont, author,
				new Vector2(GraphicsDevice.Viewport.Width / 2f, GraphicsDevice.Viewport.Height - 35),
				AuthorColor, 0.0f,
				new Vector2(start.X / 2, start.Y / 2),
				.75f, SpriteEffects.None, 0.0f);

			const string email = "pwasilewski@onet.pl";
			start = _gameFont.MeasureString(email);
			_spriteBatch.DrawString(_gameFont, email,
				new Vector2(GraphicsDevice.Viewport.Width / 2f, GraphicsDevice.Viewport.Height - 15),
				AuthorColor, 0.0f,
				new Vector2(start.X / 2, start.Y / 2),
				.75f, SpriteEffects.None, 0.0f);

			_spriteBatch.End();
		}
	}
}
