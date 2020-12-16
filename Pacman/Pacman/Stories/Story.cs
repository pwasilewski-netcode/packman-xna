using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Pacman.Stories
{
	public abstract class Story
	{
		protected readonly GraphicsDevice GraphicsDevice;
		protected readonly ContentManager Content;
		private KeyboardState _keyboardState = Keyboard.GetState(PlayerIndex.One);

		protected Story(GraphicsDevice graphicsDevice, ContentManager content)
		{
			GraphicsDevice = graphicsDevice;
			Content = content;
			IsLoaded = false;
		}

		public bool IsLoaded { get; protected set; }

		public abstract void LoadContent();

		public abstract void UnloadContent();

		public abstract void Start();
		
		public abstract void Stop();

		public abstract void Update(GameTime gameTime);

		public abstract void Draw(GameTime gameTime);

		public event EventHandler<ChangeStoryEventArgs> ChangingStory;

		protected void SwitchStory(string nextStoryName)
		{
			var isLoaded = IsLoaded;
			IsLoaded = false;

			if (ChangingStory == null)
			{
				IsLoaded = isLoaded;
				return;
			}

			ChangingStory(this, new ChangeStoryEventArgs(nextStoryName));
		}

		protected void StoreKeyboardState()
		{
			_keyboardState = Keyboard.GetState(PlayerIndex.One);
		}

		protected bool IsKeyPressed(Keys key)
		{
			return Keyboard.GetState(PlayerIndex.One).IsKeyUp(key) && _keyboardState.IsKeyDown(key);
		}
	}
}
