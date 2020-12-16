using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Pacman.Stories;

namespace Pacman
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class PacmanGame : Game
    {
		public static readonly string StoryGameName = typeof(StoryGame).Name;
		public static readonly string StoryMenuName = typeof(StoryMenu).Name;
		public static readonly string StoryEndName = typeof(StoryEnd).Name;
    	public const int InGameDashboardSize = 100;

// ReSharper disable UnaccessedField.Local
		private readonly GraphicsDeviceManager _graphics;
// ReSharper restore UnaccessedField.Local
    	private Dictionary<string, Story> _stories;
    	private string _storyName;

    	private Story CurrentStory
    	{
    		get { return _stories[_storyName]; }
    	}

        public PacmanGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

// ReSharper disable RedundantOverridenMember
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
        	var storyGame = new StoryGame(GraphicsDevice, Content);
			storyGame.ChangingStory += ChangingStory;
        	var storyMenu = new StoryMenu(GraphicsDevice, Content);
        	storyMenu.ChangingStory += ChangingStory;
			var storyEnd = new StoryEnd(GraphicsDevice, Content);
			storyEnd.ChangingStory += ChangingStory;
			_stories = new Dictionary<string, Story>
			{
				{ StoryGameName, storyGame },
				{ StoryMenuName, storyMenu },
				{ StoryEndName, storyEnd }
			};
			_storyName = StoryMenuName;
			CurrentStory.Start();
            base.Initialize();
        }

		private void ChangingStory(object sender, ChangeStoryEventArgs e)
		{
			CurrentStory.Stop();
			if (string.IsNullOrEmpty(e.NextStoryName))
			{
				Exit();
				return;
			}
			
			_storyName = e.NextStoryName;
			if (CurrentStory is StoryEnd)
			{
				((StoryEnd)CurrentStory).Score = ((StoryGame)sender).GetScore();
			}
			CurrentStory.Start();
		}

// ReSharper restore RedundantOverridenMember

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
        	foreach (var story in _stories)
        	{
        		story.Value.LoadContent();
        	}
        	var mapSize = ((StoryGame)_stories[StoryGameName]).GetMapSize();
			_graphics.PreferredBackBufferHeight = mapSize.Y;
			_graphics.PreferredBackBufferWidth = mapSize.X + InGameDashboardSize;
			_graphics.ApplyChanges();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
			foreach (var story in _stories)
			{
				story.Value.UnloadContent();
			}
		}

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
			if (CurrentStory.IsLoaded)
				CurrentStory.Update(gameTime);

        	base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
			if (CurrentStory.IsLoaded)
				CurrentStory.Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}
