using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Pacman.Sprites
{
    public abstract class Sprite
    {
		protected float TotalElapsed;
		protected const float Fps = 6f;
		protected const float Tpf = 1 / Fps;

    	protected readonly Texture2D Texture;
		protected readonly Rectangle[] Frames;
		protected readonly ILevelMap LevelMap;

    	protected float Speed = 1f;

    	public Tile[] Borders = new[] { Tile.Border, Tile.Home, Tile.Gate };

    	public SpriteDirection StartDirection { get; protected set; }
    	public Vector2 StartPosition { get; protected set; }

    	protected int FrameIndex;
    	protected Vector2 Position;
		protected SpriteDirection NextDirection;

		public SpriteDirection Direction { get; protected set; }
    			
        protected Sprite(Texture2D texture2D, Rectangle[] frames, Vector2 startPosition, SpriteDirection startDirection, ILevelMap levelMap)
        {
        	StartPosition = startPosition;
        	Position = startPosition;
        	StartDirection = startDirection;
			Direction = startDirection;
			NextDirection = startDirection;
			Texture = texture2D;
        	Frames = frames;
			FrameIndex = GetFrameIndex();
        	LevelMap = levelMap;
        }

		public virtual void Update(GameTime gameTime)
		{
			if (Direction != NextDirection)
			{
				var previousDirection = Direction;
				var collides = false;
				try
				{
					Direction = NextDirection;
					collides = LevelMap.Collides(this);
					if (!collides)
					{
						Direction = NextDirection;
						FrameIndex = GetFrameIndex();
						Move();
					}
				}
				finally
				{
					if (collides)
					{
						Direction = previousDirection;
					}
				}
				if (!collides)
					return;
			}

			if (LevelMap.Collides(this))
				return;

			Move();
			Animate(gameTime);
		}

		public virtual void Turn(SpriteDirection direction)
		{
			NextDirection = direction;
		}

		public virtual void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(Texture, Position, Frames[FrameIndex], Color.White);
		}

		public Vector2 GetPosition()
		{
			return Position;
		}

		protected virtual int GetFrameIndex()
		{
			var currentDirection = (((int)Direction - 1) * 2) + 1;
			if (FrameIndex == currentDirection)
			{
				return FrameIndex + 1;
			}
			return currentDirection;
		}

		protected virtual void RestartAnimation()
		{
			FrameIndex = (((int)Direction - 1) * 2) + 1;
		}

		protected void Animate(GameTime gameTime)
		{
			var elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
			TotalElapsed += elapsed;
			if (TotalElapsed <= Tpf)
				return;
			
			FrameIndex = GetFrameIndex();
			TotalElapsed -= Tpf;
		}

		protected virtual void Move()
		{
			Vector2 teleportTo;
			if (LevelMap.TryTeleport(this, out teleportTo))
			{
				Position.X = teleportTo.X;
				Position.Y = teleportTo.Y;
				return;
			}

			switch (Direction)
			{
				case SpriteDirection.Up:
					Position.Y -= Speed;
					return;
				case SpriteDirection.Down:
					Position.Y += Speed;
					return;
				case SpriteDirection.Left:
					Position.X -= Speed;
					return;
				case SpriteDirection.Right:
					Position.X += Speed;
					return;
				default:
					return;
			}
		}

    	public virtual void Restart()
    	{
			Direction = StartDirection;
			NextDirection = StartDirection;
			Position = StartPosition;
			RestartAnimation();
    	}

		public bool TryGetPoint(SpriteDirection spriteDirection, out Point point)
		{
			return TryGetPoint(spriteDirection, true, false, out point);
		}

		public bool TryGetPoint(bool predictMovement, out Point point)
		{
			return TryGetPoint(Direction, predictMovement, !predictMovement, out point);
		}

		public bool TryGetPoint(out Point point)
		{
			return TryGetPoint(Direction, false, false, out point);
		}

		private bool TryGetPoint(SpriteDirection spriteDirection, bool predictMovement, bool withOffset, out Point point)
		{
			point = new Point();
			var position = new Vector2(Position.X, Position.Y);
			var speed = predictMovement ? 1 : 0;
			var offset = withOffset ? Sprites.LevelMap.TileSize / 2f : 0f;
			int nextPositionX, nextPositionY;
			switch (spriteDirection)
			{
				case SpriteDirection.Up:
					position.Y += offset;
					nextPositionX = (int)position.X;
					nextPositionY = (int)position.Y - speed;
					point.X = nextPositionX / Sprites.LevelMap.TileSize;
					point.Y = nextPositionY / Sprites.LevelMap.TileSize;
					return (nextPositionX % Sprites.LevelMap.TileSize) == 0;
				case SpriteDirection.Down:
					position.Y -= offset;
					nextPositionX = (int)position.X;
					nextPositionY = (int)position.Y + speed;
					point.X = nextPositionX / Sprites.LevelMap.TileSize;
					point.Y = (nextPositionY + Sprites.LevelMap.TileSize - 1) / Sprites.LevelMap.TileSize;
					return (nextPositionX % Sprites.LevelMap.TileSize) == 0;
				case SpriteDirection.Left:
					position.X += offset;
					nextPositionX = (int)position.X - speed;
					nextPositionY = (int)position.Y;
					point.X = nextPositionX / Sprites.LevelMap.TileSize;
					point.Y = nextPositionY / Sprites.LevelMap.TileSize;
					return (nextPositionY % Sprites.LevelMap.TileSize) == 0;
				case SpriteDirection.Right:
					position.X -= offset;
					nextPositionX = (int)position.X + speed;
					nextPositionY = (int)position.Y;
					point.X = (nextPositionX + Sprites.LevelMap.TileSize - 1) / Sprites.LevelMap.TileSize;
					point.Y = nextPositionY / Sprites.LevelMap.TileSize;
					return (nextPositionY % Sprites.LevelMap.TileSize) == 0;
				default:
					return false;
			}
		}
    }
}
