using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pacman.Sprites
{
	public class ClydeSprite : GhostSprite
	{
		public ClydeSprite(Texture2D texture2D, Vector2 startPosition, ILevelMap levelMap)
			: base(texture2D, new[]
				{
					//neutral
					new Rectangle(2, 142, 16, 16),
				
					//up
					new Rectangle(2, 142, 16, 16),
					new Rectangle(22, 142, 16, 16),

					//down
					new Rectangle(42, 142, 16, 16),
					new Rectangle(62, 142, 16, 16),

					//left
					new Rectangle(82, 142, 16, 16),
					new Rectangle(102, 142, 16, 16),
				
					//right
					new Rectangle(122, 142, 16, 16),
					new Rectangle(142, 142, 16, 16)
				}, startPosition, levelMap)
		{
			TimeInHome = 15;
		}
	}
}
