using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Pacman.Sprites;
using System.Linq;

namespace Pacman.Stories
{
	public class StoryGame : Story
	{
		private const int PointsUp = 10000;
		private const int PointsDot = 10;
		private const int PointsBigDot = 50;
		private const int PointsGhost = 200;
		private const int PacmanBonusTime = 10;
		private const int PacmanBonusRunningUpTime = 2;

		private Texture2D _texture, _texturePath;
		private SpriteFont _font;
		private SoundEffect _soundStart;
		private SoundEffectInstance _soundStartInstance;
		private SoundEffect _soundEating;
		private SoundEffect _soundGhost;
		private SoundEffectInstance _soundGhostInstance;
		private SoundEffect _soundDead;
		private SoundEffectInstance _soundDeadInstance;
		private SoundEffect _soundFinish;

		private SpriteBatch _spriteBatch;

		private double _ghostsTimeInHome;
		private double _pacmanBonusTime;
		private int _pacmanBonusFactor;
		private int _pacmanScore;
		private int _pacmanLives;
		private bool _freeze = true;

		private LevelMap _levelMap;
		private PacmanSprite _pacmanSprite;
		private GhostSprite[] _ghostSprites;
		private readonly Random _random = new Random();

		public StoryGame(GraphicsDevice graphicsDevice, ContentManager content)
			: base(graphicsDevice, content) { }

		private void NewGame()
		{
			_freeze = false;
			_ghostsTimeInHome = 0;
			_pacmanBonusFactor = 0;
			_pacmanBonusTime = 0;
			_pacmanScore = 0;
			_pacmanLives = 2;
			_levelMap.LoadMap();
		}

		private void PacmanResurection()
		{
			if (_soundDeadInstance.State == SoundState.Playing)
				return;

			if (_pacmanLives < 0)
			{
				GameOver();
				return;
			}
			Thread.Sleep(1000);
			_freeze = false;
			_ghostsTimeInHome = 0;
			_pacmanBonusFactor = 0;
			_pacmanBonusTime = 0;
			_pacmanSprite.Restart();
			Array.ForEach(_ghostSprites, g => g.Restart());
			_soundStartInstance.Play();
		}

		public Point GetMapSize()
		{
			return new Point(_levelMap.MapWidth, _levelMap.MapHeight);
		}

		public int GetScore()
		{
			return _pacmanScore;
		}

		public override void LoadContent()
		{
			_texture = new Texture2D(GraphicsDevice, 1, 1, true, SurfaceFormat.Color);
			_texture.SetData(new[] { Color.Black });
			_texturePath = new Texture2D(GraphicsDevice, 1, 1, true, SurfaceFormat.Color);
			_texturePath.SetData(new[] { Color.GreenYellow });
			_font = Content.Load<SpriteFont>("gameFont");
			_spriteBatch = new SpriteBatch(GraphicsDevice);
			_levelMap = new LevelMap(Content, "levelmap.txt");
			_pacmanSprite = _levelMap.CreatePacmanSprite();
			_ghostSprites = new GhostSprite[]
			{
				_levelMap.CreateBlinkySprite(),
				_levelMap.CreatePinkySprite(),
				_levelMap.CreateInkySprite(),
				_levelMap.CreateClydeSprite()
			};
			_soundStart = Content.Load<SoundEffect>("pacman_start");
			_soundStartInstance = _soundStart.CreateInstance();
			_soundEating = Content.Load<SoundEffect>("pacman_eating");
			_soundGhost = Content.Load<SoundEffect>("pacman_ghost");
			_soundGhostInstance = _soundGhost.CreateInstance();
			_soundGhostInstance.IsLooped = true;
			_soundDead = Content.Load<SoundEffect>("pacman_dead");
			_soundDeadInstance = _soundDead.CreateInstance();
			_soundFinish = Content.Load<SoundEffect>("pacman_finish");
		}

		public override void UnloadContent()
		{
			_spriteBatch.Dispose();
			_levelMap.Dispose();
			_soundStartInstance.Dispose();
			_soundStart.Dispose();
			_soundEating.Dispose();
			_soundGhostInstance.Dispose();
			_soundGhost.Dispose();
			_soundDeadInstance.Dispose();
			_soundDead.Dispose();
			_soundFinish.Dispose();
		}

		public override void Start()
		{
			NewGame();
			_pacmanSprite.Restart();
			Array.ForEach(_ghostSprites, g => g.Restart());
			_soundStartInstance.Play();
			IsLoaded = true;
		}

		public override void Stop()
		{
			_freeze = true;
			_soundStartInstance.Stop();
			_soundGhostInstance.Stop();
		}

		public override void Update(GameTime gameTime)
		{
			if (IsKeyPressed(Keys.Escape))
			{
				StoreKeyboardState();
				SwitchStory(PacmanGame.StoryMenuName);
				return;
			}
			StoreKeyboardState();

			var keyboardState = Keyboard.GetState(PlayerIndex.One);
			if (keyboardState.IsKeyDown(Keys.Up))
			{
				_pacmanSprite.Turn(SpriteDirection.Up);
			}
			else if (keyboardState.IsKeyDown(Keys.Down))
			{
				_pacmanSprite.Turn(SpriteDirection.Down);
			}
			else if (keyboardState.IsKeyDown(Keys.Left))
			{
				_pacmanSprite.Turn(SpriteDirection.Left);
			}
			else if (keyboardState.IsKeyDown(Keys.Right))
			{
				_pacmanSprite.Turn(SpriteDirection.Right);
			}

			if (_freeze)
			{
				if (_pacmanSprite.IsDead)
					_pacmanSprite.Update(gameTime);
				else
					PacmanResurection();
				return;
			}

			if (_soundStartInstance.State == SoundState.Playing)
				return;

			_ghostsTimeInHome += gameTime.ElapsedGameTime.TotalSeconds;

			if (_pacmanBonusTime > 0)
			{
				_pacmanBonusTime -= gameTime.ElapsedGameTime.TotalSeconds;
			}
			else
			{
				_pacmanBonusTime = 0;
				_pacmanBonusFactor = 0;
				if (_soundGhostInstance.State == SoundState.Playing)
				{
					_soundGhostInstance.Stop();
				}
			}

			Array.ForEach(_ghostSprites, g => UpdateGhost(g, gameTime));
			UpdatePacman(gameTime);

			if (_levelMap.CountDots() == 0)
			{
				NextLevel();
			}
		}

		private void UpdateGhost(GhostSprite ghostSprite, GameTime gameTime)
		{
			switch (ghostSprite.Status)
			{
				case GhostStatus.Normal:
					UpdateGhostAsNormal(ghostSprite);
					break;
				case GhostStatus.Panic:
					if (_pacmanBonusTime <= PacmanBonusRunningUpTime)
						ghostSprite.SetStatus(GhostStatus.PanicRunningUp);
					UpdateGhostAsPanic(ghostSprite);
					break;
				case GhostStatus.PanicRunningUp:
					if (_pacmanBonusTime <= 0)
						ghostSprite.SetStatus(GhostStatus.Normal);
					UpdateGhostAsPanic(ghostSprite);
					break;
				case GhostStatus.Ate:
					UpdateGhostAsAte(ghostSprite);
					break;
				case GhostStatus.InHome:
					UpdateGhostAsInHome(ghostSprite);
					break;
				default:
					break;
			}

			ghostSprite.Update(gameTime);
		}

		private void UpdateGhostAsNormal(GhostSprite ghostSprite)
		{
			SpriteDirection nextDirection;
			var path = _levelMap.GetSpritePath(ghostSprite, _pacmanSprite, out nextDirection);
			if (path == null)
				return;

			ghostSprite.Turn(nextDirection);
		}

		private void UpdateGhostAsPanic(GhostSprite ghostSprite)
		{
			Point point;
			if (!ghostSprite.TryGetPoint(false, out point))
				return;

			var directions = _levelMap.GetNeighborDirections(point, ghostSprite).ToList();
			if (directions.Count > 0)
			{
				if (directions.Count == 1)
					ghostSprite.Turn(directions[0]);
				else
				{
					directions = directions.Where(d => !LevelMap.AreOppositeDirections(ghostSprite.Direction, d)).ToList();
					if (directions.Count > 1)
						directions = directions.Where(d => d != ghostSprite.Direction).ToList();

					ghostSprite.Turn(directions[_random.Next(0, directions.Count)]);
				}
			}
		}

		private void UpdateGhostAsAte(GhostSprite ghostSprite)
		{
			SpriteDirection homeDirection;
			var homePoint = new Point((int)(ghostSprite.StartPosition.X / LevelMap.TileSize), (int)(ghostSprite.StartPosition.Y / LevelMap.TileSize));
			var path = _levelMap.GetSpritePath(ghostSprite, homePoint, out homeDirection);
			if (path == null)
				return;

			if (path.Length == 1)
				ghostSprite.SetStatus(GhostStatus.InHome);
			else
				ghostSprite.Turn(homeDirection);
		}

		private void UpdateGhostAsInHome(GhostSprite ghostSprite)
		{
			Point point;
			if (!ghostSprite.TryGetPoint(false, out point))
				return;

			var directions = _levelMap.GetNeighborDirections(point, ghostSprite).ToList();
			if (directions.Count > 0)
			{
				if (directions.Count == 1)
					ghostSprite.Turn(directions[0]);
				else
				{
					directions = directions.Where(d => !LevelMap.AreOppositeDirections(ghostSprite.Direction, d)).ToList();
					if (directions.Count > 1)
						directions = directions.Where(d => d != ghostSprite.Direction).ToList();

					ghostSprite.Turn(directions[_random.Next(0, directions.Count)]);
				}
			}
		}

		private void UpdatePacman(GameTime gameTime)
		{
			_pacmanSprite.Update(gameTime);
			CheckPacmanScore();
			Array.ForEach(_ghostSprites, CheckPacmanGhostCollision);
		}

		private void CheckPacmanScore()
		{
			Point point;
			if (!_pacmanSprite.TryGetPoint(false, out point))
				return;

			var tile = _levelMap.GetTile(point);
			switch (tile)
			{
				case Tile.Dot:
					_soundEating.Play();
					_levelMap.Eat(point);
					AddPoints(PointsDot);
					break;
				case Tile.BigDot:
					_soundEating.Play();
					_levelMap.Eat(point);
					Array.ForEach(_ghostSprites, g => g.SetStatus(GhostStatus.Panic));
					AddPoints(PointsBigDot);
					if (_pacmanBonusTime <= 0)
					{
						_soundGhostInstance.Play();
						_pacmanBonusTime = 0;
					}
					_pacmanBonusTime += PacmanBonusTime;
					break;
				default:
					break;
			}
		}

		private void PacmanDies()
		{
			Stop();
			Thread.Sleep(1000);
			_soundDeadInstance.Play();
			_pacmanSprite.Dead();
			_pacmanLives--;
		}

		private void AddPoints(int points)
		{
			_pacmanLives += ((_pacmanScore + points) / PointsUp) - (_pacmanScore / PointsUp);
			_pacmanScore += points;
		}

		private void CheckPacmanGhostCollision(GhostSprite ghost)
		{
			const int offset = LevelMap.TileSize / 4;
			const int size = LevelMap.TileSize / 2;
			var pacmanPosition = _pacmanSprite.GetPosition();
			var pacmanRectangle = new Rectangle((int)pacmanPosition.X + offset, (int)pacmanPosition.Y + offset, size, size);
			var ghostPosition = ghost.GetPosition();
			var ghostRectangle = new Rectangle((int)ghostPosition.X + offset, (int)ghostPosition.Y + offset, size, size);
			if (!pacmanRectangle.Intersects(ghostRectangle))
				return;

			if (ghost.Status == GhostStatus.Normal)
			{
				PacmanDies();
			}
			else if (ghost.Status == GhostStatus.Panic || ghost.Status == GhostStatus.PanicRunningUp)
			{
				_soundEating.Play();
				ghost.SetStatus(GhostStatus.Ate);
				AddPoints(++_pacmanBonusFactor * PointsGhost);
			}
		}

		private void GameOver()
		{
			SwitchStory(PacmanGame.StoryEndName);
		}

		private void NextLevel()
		{
			SwitchStory(PacmanGame.StoryEndName);
		}

		public override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.Black);

			_spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

			_levelMap.Draw(_spriteBatch);

			if (_freeze == _pacmanSprite.IsDead)
				_pacmanSprite.Draw(_spriteBatch);

			if (!_freeze)
				Array.ForEach(_ghostSprites, g => g.Draw(_spriteBatch));

			_spriteBatch.Draw(
				_texture,
				new Rectangle(
					GraphicsDevice.Viewport.Width - PacmanGame.InGameDashboardSize, 0,
					PacmanGame.InGameDashboardSize, GraphicsDevice.Viewport.Height),
				Color.Black);

			var score = _pacmanScore.ToString();
			var textCenter = _font.MeasureString(score);
			var drawPosition = new Vector2(_levelMap.MapWidth + (PacmanGame.InGameDashboardSize / 2), 20);
			_spriteBatch.DrawString(_font, score, drawPosition, Color.Yellow, 0f, new Vector2(textCenter.X / 2, textCenter.Y / 2), 1f, SpriteEffects.None, 0f);

			drawPosition.X = GraphicsDevice.Viewport.Width - PacmanGame.InGameDashboardSize + LevelMap.TileSize;
			drawPosition.Y += (2 * LevelMap.TileSize);
			_pacmanSprite.DrawAvatar(_spriteBatch, drawPosition);
			drawPosition.X += 20;
			drawPosition.Y -= 3;
			var lives = _pacmanLives >= 0 ? _pacmanLives.ToString() : "0";
			_spriteBatch.DrawString(_font, string.Concat("x", lives), drawPosition, Color.Yellow, 0f, Vector2.Zero, .8f, SpriteEffects.None, 0f);

			drawPosition.X = GraphicsDevice.Viewport.Width - PacmanGame.InGameDashboardSize;
			drawPosition.Y += 140;

			if (_soundStartInstance.State == SoundState.Playing)
			{
				const string getReady = "Get ready!";
				textCenter = _font.MeasureString(getReady);
				_spriteBatch.DrawString(_font, getReady, drawPosition, Color.Yellow, 0f, new Vector2(textCenter.X / 2, textCenter.Y / 2), 1f, SpriteEffects.None, 0f);
			}
			else if (_pacmanSprite.IsDead && _pacmanLives < 0)
			{
				const string gameOver = "Game over!";
				textCenter = _font.MeasureString(gameOver);
				_spriteBatch.DrawString(_font, gameOver, drawPosition, Color.Red, 0f, new Vector2(textCenter.X / 2, textCenter.Y / 2), 1f, SpriteEffects.None, 0f);
			}
			_spriteBatch.End();
		}
	}
}
