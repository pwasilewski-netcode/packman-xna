using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Pacman.Sprites
{
	public class PacmanSprite : Sprite
	{
		public bool IsDead { get; private set; }

		private readonly Rectangle[] _frameDead = new[]
		{
			new Rectangle(2, 241, 16, 16),
			new Rectangle(2, 241, 16, 16),
			new Rectangle(2, 241, 16, 16),
			new Rectangle(22, 241, 16, 16),
			new Rectangle(42, 241, 16, 16),
			new Rectangle(62, 242, 16, 16),
			new Rectangle(82, 243, 16, 16),
			new Rectangle(102, 244, 16, 16),
			new Rectangle(122, 244, 16, 16),
			new Rectangle(142, 244, 16, 16),
			new Rectangle(162, 244, 16, 16),
			new Rectangle(182, 245, 16, 16),
			new Rectangle(202, 245, 16, 16),
			new Rectangle(202, 245, 16, 16),
			new Rectangle(202, 245, 16, 16),
			new Rectangle(202, 245, 16, 16)
		};

		public PacmanSprite(Texture2D texture2D, Vector2 startPosition, ILevelMap levelMap)
			: base(texture2D, new[]
				{
					//neutral
					new Rectangle(42, 2, 16, 16),
				
					//up
					new Rectangle(2, 42, 16, 16),
					new Rectangle(22, 42, 16, 16),

					//down
					new Rectangle(2, 61, 16, 16),
					new Rectangle(22, 60, 16, 16),

					//left
					new Rectangle(2, 2, 16, 16),
					new Rectangle(22, 2, 16, 16),
				
					//right
					new Rectangle(1, 22, 16, 16),
					new Rectangle(21, 22, 16, 16)
				}, startPosition, SpriteDirection.Right, levelMap) { }

		public override void Restart()
		{
			IsDead = false;
			base.Restart();
		}

		public void Dead()
		{
			IsDead = true;
			FrameIndex = 0;
		}

		protected override int GetFrameIndex()
		{
			if (IsDead)
			{
				return FrameIndex + 1;
			}
			return base.GetFrameIndex();
		}

		public override void Update(GameTime gameTime)
		{
			if (IsDead)
			{
				Animate(gameTime);
				if (FrameIndex >= _frameDead.Length)
					IsDead = false;
				return;
			}
			base.Update(gameTime);
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			if (IsDead)
				spriteBatch.Draw(Texture, Position, _frameDead[FrameIndex], Color.White);
			else
				base.Draw(spriteBatch);
		}

		public void DrawAvatar(SpriteBatch spriteBatch, Vector2 position)
		{
			spriteBatch.Draw(Texture, position, Frames[7], Color.White);
		}
	}
}
