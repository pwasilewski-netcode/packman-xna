using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pacman.Sprites
{
	public abstract class GhostSprite : Sprite
	{
		protected readonly Rectangle[] FrameDead = new[]
		{
			//neutral
			new Rectangle(2, 202, 16, 16),

			//up
			new Rectangle(2, 202, 16, 16),
			new Rectangle(2, 202, 16, 16),
			
			//down
			new Rectangle(22, 202, 16, 16),
			new Rectangle(22, 202, 16, 16),
			
			//left
			new Rectangle(42, 202, 16, 16),
			new Rectangle(42, 202, 16, 16),
			
			//right
			new Rectangle(62, 202, 16, 16),
			new Rectangle(62, 202, 16, 16)
		};

		protected readonly Rectangle[] FramePacmanBonus = new[]
		{
			//neutral
			new Rectangle(2, 162, 16, 16),

			//up
			new Rectangle(2, 162, 16, 16),
			new Rectangle(22, 162, 16, 16),

			//down
			new Rectangle(2, 162, 16, 16),
			new Rectangle(22, 162, 16, 16),

			//left
			new Rectangle(2, 162, 16, 16),
			new Rectangle(22, 162, 16, 16),

			//right
			new Rectangle(2, 162, 16, 16),
			new Rectangle(22, 162, 16, 16)
		};

		protected readonly Rectangle[] FramePacmanBonusRunningUp = new[]
		{
			//neutral
			new Rectangle(42, 162, 16, 16),

			//up
			new Rectangle(2, 162, 16, 16),
			new Rectangle(62, 162, 16, 16),

			//down
			new Rectangle(2, 162, 16, 16),
			new Rectangle(62, 162, 16, 16),

			//left
			new Rectangle(2, 162, 16, 16),
			new Rectangle(62, 162, 16, 16),

			//right
			new Rectangle(2, 162, 16, 16),
			new Rectangle(62, 162, 16, 16)
		};

		protected GhostSprite(Texture2D texture2D, Rectangle[] frames, Vector2 startPosition, ILevelMap levelMap)
			: base(texture2D, frames, startPosition, SpriteDirection.Up, levelMap) 
		{
			TimeInHome = 5;
		}

		public GhostStatus Status { get; protected set; }

		public int TimeInHome { get; protected set; }

		private double _elapsedTimeInHome;
		private readonly static Tile[] BordersClosed = new[] { Tile.Border, Tile.Home, Tile.Gate };
		private readonly static Tile[] BordersOpened = new[] { Tile.Border, Tile.Home };

		public void SetStatus(GhostStatus ghostStatus)
		{
			switch (ghostStatus)
			{
				case GhostStatus.Panic:
					if (Status == GhostStatus.InHome)
						return;
					Speed = .6f;
					break;
				case GhostStatus.PanicRunningUp:
					if (Status != GhostStatus.Panic)
						return;
					Speed = .65f;
					break;
				case GhostStatus.Ate:
					_elapsedTimeInHome = 0;
					Borders = BordersOpened;
					Speed = 1f;
					break;
				case GhostStatus.InHome:
					Borders = BordersClosed;
					Speed = .98f;
					break;
				default:
					Speed = .98f;
					if (Status == GhostStatus.InHome)
						Borders = BordersOpened;
					else if (Status == GhostStatus.Normal && Borders == BordersOpened && !LevelMap.IsGhostInHome(this))
						Borders = BordersClosed;
					break;
			}
			Status = ghostStatus;
		}

		public override void Restart()
		{
			_elapsedTimeInHome = 0;
			SetStatus(GhostStatus.InHome);
			base.Restart();
		}

		protected override int GetFrameIndex()
		{
			var currentDirection = (((int)NextDirection - 1) * 2) + 1;
			if (FrameIndex == currentDirection)
			{
				return FrameIndex + 1;
			}
			return currentDirection;
		}

		public override void Update(GameTime gameTime)
		{
			if (Status == GhostStatus.InHome && LevelMap.IsGhostInHome(this))
			{
				_elapsedTimeInHome += gameTime.ElapsedGameTime.TotalSeconds;
				if (_elapsedTimeInHome >= TimeInHome)
					SetStatus(GhostStatus.Normal);
			}
			else if (Status == GhostStatus.Normal)
				SetStatus(GhostStatus.Normal);

			base.Update(gameTime);
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			switch (Status)
			{
				case GhostStatus.Panic:
					spriteBatch.Draw(Texture, Position, FramePacmanBonus[FrameIndex], Color.White);
					break;
				case GhostStatus.PanicRunningUp:
					spriteBatch.Draw(Texture, Position, FramePacmanBonusRunningUp[FrameIndex], Color.White);
					break;
				case GhostStatus.Ate:
					spriteBatch.Draw(Texture, Position, FrameDead[FrameIndex], Color.White);
					break;
				default:
					base.Draw(spriteBatch);
					break;
			}
		}
	}
}
