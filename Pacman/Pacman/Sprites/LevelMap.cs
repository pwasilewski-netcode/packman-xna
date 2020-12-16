using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Pacman.Sprites
{
	public class LevelMap : ILevelMap, IDisposable
	{
		public const int TileSize = 16;

		private static readonly Rectangle BorderRectangle = new Rectangle(42, 22, TileSize, TileSize);
		private static readonly Rectangle GateRectangle = new Rectangle(62, 22, TileSize, TileSize);
		private static readonly Rectangle DotRectangle = new Rectangle(42, 60, TileSize, TileSize);
		private static readonly Rectangle BigDotRectangle = new Rectangle(42, 42, TileSize, TileSize);
		private readonly Texture2D _texture;
		private readonly string _mapFileName;

		private const char Border = '=';
		private const char Gate = '_';
		private const char Home = '-';
		private const char Dot = '.';
		private const char BigDot = '*';
		private const char StartPacman = 'S';
		private const char Teleport = '0';
		private const char StartBlinky = 'B';
		private const char StartPinky = 'P';
		private const char StartInky = 'I';
		private const char StartClyde = 'C';

		private Rectangle _ghostsHome;
		private Point _teleport1, _teleport2;
		private Tile[,] _grid;
		private int _gridLengthX, _gridLengthY;
		private Vector2 _pacmanStartPosition;
		private Vector2 _blinkyStartPosition;
		private Vector2 _pinkyStartPosition;
		private Vector2 _inkyStartPosition;
		private Vector2 _clydeStartPosition;

		public int MapHeight { get; private set; }
		public int MapWidth { get; private set; }

		public LevelMap(ContentManager content, string fileName)
		{
			_texture = content.Load<Texture2D>("pacman_sprites");
			_mapFileName = fileName;
			LoadMap();
		}

		public void LoadMap()
		{
			var mapReader = File.OpenText(_mapFileName);
			var height = mapReader.ReadLine();
			var width = mapReader.ReadLine();
			if (height == null || width == null)
			{
				_grid = null;
				return;
			}
			_grid = new Tile[int.Parse(width), int.Parse(height)];
			_gridLengthX = _grid.GetLength(0);
			_gridLengthY = _grid.GetLength(1);
			MapWidth = _gridLengthX * TileSize;
			MapHeight = _gridLengthY * TileSize;
			bool ghostsHomeIsSet = false;
			_ghostsHome = new Rectangle();
			for (var y = 0; y < _gridLengthY; y++)
			{
				var mapLine = mapReader.ReadLine();
				if (mapLine == null)
				{
					_grid = null;
					return;
				}
				for (var x = 0; x < _gridLengthX; x++)
				{
					var tile = GetTileFromChar(mapLine[x]);
					if (tile == Tile.Teleport)
					{
						if (_teleport1 == default(Point))
						{
							_teleport1 = new Point(x, y);
						}
						else if (_teleport2 == default(Point))
						{
							_teleport2 = new Point(x, y);
						}
					}
					else if (tile == Tile.Home)
					{
						if (!ghostsHomeIsSet)
						{
							ghostsHomeIsSet = true;
							_ghostsHome.X = x;
							_ghostsHome.Y = y;
							_ghostsHome.Width = x;
							_ghostsHome.Height = y;
						}
						else
						{
							if (x < _ghostsHome.X)
								_ghostsHome.X = x;
							else if (x > _ghostsHome.Width)
								_ghostsHome.Width = x;

							if (y < _ghostsHome.Y)
								_ghostsHome.Y = y;
							else if (y > _ghostsHome.Height)
								_ghostsHome.Height = y;

						}
					}
					_grid[x, y] = tile;
					if (tile == Tile.StartPacman)
					{
						_pacmanStartPosition = new Vector2(x * TileSize, y * TileSize);
					}
					else if (tile == Tile.StartBlinky)
					{
						_blinkyStartPosition = new Vector2(x * TileSize, y * TileSize);
					}
					else if (tile == Tile.StartPinky)
					{
						_pinkyStartPosition = new Vector2(x * TileSize, y * TileSize);
					}
					else if (tile == Tile.StartInky)
					{
						_inkyStartPosition = new Vector2(x * TileSize, y * TileSize);
					}
					else if (tile == Tile.StartClyde)
					{
						_clydeStartPosition = new Vector2(x * TileSize, y * TileSize);
					}
				}
			}
			_ghostsHome.Width -= _ghostsHome.X;
			_ghostsHome.Height -= _ghostsHome.Y;
		}

		public PacmanSprite CreatePacmanSprite()
		{
			return new PacmanSprite(_texture, _pacmanStartPosition, this);
		}

		public BlinkySprite CreateBlinkySprite()
		{
			return new BlinkySprite(_texture, _blinkyStartPosition, this);
		}

		public PinkySprite CreatePinkySprite()
		{
			return new PinkySprite(_texture, _pinkyStartPosition, this);
		}

		public InkySprite CreateInkySprite()
		{
			return new InkySprite(_texture, _inkyStartPosition, this);
		}

		public ClydeSprite CreateClydeSprite()
		{
			return new ClydeSprite(_texture, _clydeStartPosition, this);
		}

		private static Tile GetTileFromChar(char tileChar)
		{
			switch (tileChar)
			{
				case Border:
					return Tile.Border;
				case Gate:
					return Tile.Gate;
				case Home:
					return Tile.Home;
				case Dot:
					return Tile.Dot;
				case BigDot:
					return Tile.BigDot;
				case StartPacman:
					return Tile.StartPacman;
				case StartBlinky:
					return Tile.StartBlinky;
				case StartPinky:
					return Tile.StartPinky;
				case StartInky:
					return Tile.StartInky;
				case StartClyde:
					return Tile.StartClyde;
				case Teleport:
					return Tile.Teleport;
				default:
					return Tile.None;
			}
		}

		public Tile GetTile(Point point)
		{
			if (point.X < 0)
				point.X = 0;
			else if (point.X >= _gridLengthX)
				point.X = _gridLengthX - 1;

			if (point.Y < 0)
				point.Y = 0;
			else if (point.Y >= _gridLengthY)
				point.Y = _gridLengthY - 1;

			return _grid[point.X, point.Y];
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			var position = Vector2.Zero;

			for (var x = 0; x < _gridLengthX; x++)
			{
				for (var y = 0; y < _gridLengthY; y++)
				{
					position.X = x * TileSize;
					position.Y = y * TileSize;
					switch (_grid[x, y])
					{
						case Tile.Dot:
							spriteBatch.Draw(_texture, position, DotRectangle, Color.White);
							break;
						case Tile.BigDot:
							spriteBatch.Draw(_texture, position, BigDotRectangle, Color.White);
							break;
						case Tile.Border:
						case Tile.Home:
							spriteBatch.Draw(_texture, position, BorderRectangle, Color.White);
							break;
						case Tile.Gate:
							spriteBatch.Draw(_texture, position, GateRectangle, Color.White);
							break;
						default:
							break;
					}
				}
			}
		}

		public void Eat(Point point)
		{
			if (point.X < 0 || point.X >= _gridLengthX || point.Y < 0 || point.Y >= _gridLengthY)
				return;

			if (_grid[point.X, point.Y] == Tile.Dot || _grid[point.X, point.Y] == Tile.BigDot)
				_grid[point.X, point.Y] = Tile.None;
		}

		public bool Collides(Sprite sprite)
		{
			Point point;
			if (!sprite.TryGetPoint(true, out point))
				return true;
			return sprite.Borders.Contains(GetTile(point));
		}

		public bool Collides(Sprite sprite, SpriteDirection spriteDirection)
		{
			Point point;
			if (!sprite.TryGetPoint(spriteDirection, out point))
				return true;
			return sprite.Borders.Contains(GetTile(point));
		}

		public bool TryTeleport(Sprite sprite, out Vector2 teleportOutPosition)
		{
			teleportOutPosition = Vector2.Zero;
			Point point;
			if (!sprite.TryGetPoint(true, out point) || GetTile(point) != Tile.Teleport)
				return false;

			if (point.X < 0)
				point.X = 0;
			else if (point.X >= _gridLengthX)
				point.X = _gridLengthX - 1;

			if (point.Y < 0)
				point.Y = 0;
			else if (point.Y >= _gridLengthY)
				point.Y = _gridLengthY - 1;

			var position = sprite.GetPosition();
			switch (sprite.Direction)
			{
				case SpriteDirection.Up:
					if (position.Y + TileSize < point.Y * TileSize)
					{
						teleportOutPosition = (point.Equals(_teleport1))
							? new Vector2(_teleport2.X * TileSize, _teleport2.Y * TileSize + TileSize)
							: new Vector2(_teleport1.X * TileSize, _teleport1.Y * TileSize + TileSize);
						return true;
					}
					return false;
				case SpriteDirection.Down:
					if (position.Y - TileSize > point.Y * TileSize)
					{
						teleportOutPosition = (point.Equals(_teleport1))
							? new Vector2(_teleport2.X * TileSize, _teleport2.Y * TileSize - TileSize)
							: new Vector2(_teleport1.X * TileSize, _teleport1.Y * TileSize - TileSize);
						return true;
					}					return false;
				case SpriteDirection.Left:
					if (position.X + TileSize < point.X * TileSize)
					{
						teleportOutPosition = (point.Equals(_teleport1))
							? new Vector2(_teleport2.X * TileSize + TileSize, _teleport2.Y * TileSize)
							: new Vector2(_teleport1.X * TileSize + TileSize, _teleport1.Y * TileSize);
						return true;
					}
					return false;
				case SpriteDirection.Right:
					if (position.X - TileSize > point.X * TileSize)
					{
						teleportOutPosition = (point.Equals(_teleport1))
							? new Vector2(_teleport2.X * TileSize - TileSize, _teleport2.Y * TileSize)
							: new Vector2(_teleport1.X * TileSize - TileSize, _teleport1.Y * TileSize);
						return true;
					}
					return false;
				default:
					return false;
			}
		}

		public int CountDots()
		{
			return (from Tile t in _grid
					   let c = (t == Tile.Dot || t == Tile.BigDot) ? 1 : 0
			           select c).Sum();
		}

		private static Point GetPointOffset(SpriteDirection spriteDirection)
		{
			var point = new Point();
			switch (spriteDirection)
			{
				case SpriteDirection.Up:
					point.Y--;
					break;
				case SpriteDirection.Down:
					point.Y++;
					break;
				case SpriteDirection.Left:
					point.X--;
					break;
				case SpriteDirection.Right:
					point.X++;
					break;
				default:
					break;
			}
			return point;
		}

		public IEnumerable<SpriteDirection> GetNeighborDirections(Point point, GhostSprite sprite)
		{
			var directions = new List<SpriteDirection>();
			if ((point.X + 1) < _gridLengthX && !sprite.Borders.Contains(_grid[point.X + 1, point.Y]))
				directions.Add(SpriteDirection.Right);
			if ((point.X - 1) >= 0 && !sprite.Borders.Contains(_grid[point.X - 1, point.Y]))
				directions.Add(SpriteDirection.Left);
			if ((point.Y + 1) < _gridLengthY && !sprite.Borders.Contains(_grid[point.X, point.Y + 1]))
				directions.Add(SpriteDirection.Down);
			if ((point.Y - 1) >= 0 && !sprite.Borders.Contains(_grid[point.X, point.Y - 1]))
				directions.Add(SpriteDirection.Up);

			return directions;
		}

		public static bool AreOppositeDirections(SpriteDirection direction1, SpriteDirection direction2)
		{
			if ((direction1 == SpriteDirection.Left && direction2 == SpriteDirection.Right)
				|| (direction1 == SpriteDirection.Right && direction2 == SpriteDirection.Left))
				return true;

			if ((direction1 == SpriteDirection.Up && direction2 == SpriteDirection.Down)
				|| (direction1 == SpriteDirection.Down && direction2 == SpriteDirection.Up))
				return true;

			return false;
		}

		private static int GetDistance(Point point1, Point point2)
		{
			return Math.Abs(point1.X - point2.X) + Math.Abs(point1.Y - point2.Y);
		}

		private static Point GetMinPoint(IList<Point> points, int[,] f, int[,] h)
		{
			var minPoint = points[0];
			var minF = f[minPoint.X, minPoint.Y];
			for (var i = 1; i < points.Count; i++)
			{
				var point = points[i];
				var value = f[point.X, point.Y];
				if (value < minF)
				{
					minF = value;
					minPoint = point;
				}
				else if (value == minF && h[point.X, point.Y] < h[minPoint.X, minPoint.Y])
				{
					minPoint = point;
				}
			}
			return minPoint;
		}

		private static Point[] GetPath(Point?[,] pathPointMap, Point point)
		{
			var path = new List<Point>();
			var p = new Point?(point);
			while (p.HasValue)
			{
				path.Add(p.Value);
				p = pathPointMap[p.Value.X, p.Value.Y];
			}
			path.Reverse();
			return path.ToArray();
		}

		private Point NormalizePoint(Point point)
		{
			if (point.X < 0)
				point.X = 0;
			else if (point.X >= _gridLengthX)
				point.X = _gridLengthX - 1;

			if (point.Y < 0)
				point.Y = 0;
			else if (point.Y >= _gridLengthY)
				point.Y = _gridLengthY - 1;
			return point;
		}

		public Point[] GetSpritePath(GhostSprite sprite, Point endPoint, out SpriteDirection nearestDirection)
		{
			nearestDirection = sprite.Direction;
			Point startPoint;
			if (!sprite.TryGetPoint(false, out startPoint))
				return null;

			endPoint = NormalizePoint(endPoint);
			var g = new int[_gridLengthX, _gridLengthY];//Path length
			var h = new int[_gridLengthX, _gridLengthY];//Heuristic path length
			var f = new int[_gridLengthX, _gridLengthY];//Path length + heuristic path length
			var visitedPoints = new bool[_gridLengthX, _gridLengthY];
			var pointsToVisit = new HashSet<Point> { startPoint };
			var pathPointMap = new Point?[_gridLengthX, _gridLengthY];
			h[startPoint.X, startPoint.Y] = GetDistance(startPoint, endPoint);
			f[startPoint.X, startPoint.Y] = h[startPoint.X, startPoint.Y];

			while (pointsToVisit.Count > 0)
			{
				var point = GetMinPoint(pointsToVisit.ToList(), f, h);
				if (point == endPoint)
					break;

				pointsToVisit.Remove(point);
				visitedPoints[point.X, point.Y] = true;
				var neighborPoints = GetNeighborDirections(point, sprite)
					.Select(GetPointOffset)
					.Select(p => NormalizePoint(new Point(p.X + point.X, p.Y + point.Y)))
					.Where(p => visitedPoints[p.X, p.Y] == false);
				foreach (var p in neighborPoints)
				{
					var tentativeG = g[point.X, point.Y] + 1;
					if (!pointsToVisit.Add(p) && tentativeG >= g[p.X, p.Y])
						continue;

					pathPointMap[p.X, p.Y] = point;
					g[p.X, p.Y] = tentativeG;
					h[p.X, p.Y] = GetDistance(p, endPoint);
					f[p.X, p.Y] = tentativeG + h[p.X, p.Y];
				}
			}

			var path = GetPath(pathPointMap, endPoint);
			if (path.Length < 2)
				return path;

			var nextPoint = path[1];
			if (startPoint.X > nextPoint.X)
				nearestDirection = SpriteDirection.Left;
			else if (startPoint.X < nextPoint.X)
				nearestDirection = SpriteDirection.Right;
			else if (startPoint.Y > nextPoint.Y)
				nearestDirection = SpriteDirection.Up;
			else
				nearestDirection = SpriteDirection.Down;

			return path;
		}

		public Point[] GetSpritePath(GhostSprite sprite1, Sprite sprite2, out SpriteDirection nearestDirection)
		{
			nearestDirection = sprite1.Direction;
			Point endPoint;
			return !sprite2.TryGetPoint(false, out endPoint)
				? null
				: GetSpritePath(sprite1, endPoint, out nearestDirection);
		}

		public bool IsGhostInHome(GhostSprite ghostSprite)
		{
			Point point;
			if (!ghostSprite.TryGetPoint(out point))
				return false;// ghostSprite.Status == GhostStatus.Ate || ghostSprite.Status == GhostStatus.InHome;

			return _ghostsHome.Intersects(new Rectangle(point.X, point.Y, 1, 1));
		}

		#region Implementation of IDisposable

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		/// <filterpriority>2</filterpriority>
		public void Dispose()
		{
			_texture.Dispose();
		}

		#endregion
	}
}
