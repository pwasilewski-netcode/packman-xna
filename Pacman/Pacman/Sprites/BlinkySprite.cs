using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pacman.Sprites
{
	public class BlinkySprite : GhostSprite
	{
		public BlinkySprite(Texture2D texture2D, Vector2 startPosition, ILevelMap levelMap)
			: base(texture2D, new[]
				{
					//neutral
					new Rectangle(2, 82, 16, 16),
				
					//up
					new Rectangle(2, 82, 16, 16),
					new Rectangle(22, 82, 16, 16),

					//down
					new Rectangle(42, 82, 16, 16),
					new Rectangle(62, 82, 16, 16),

					//left
					new Rectangle(82, 82, 16, 16),
					new Rectangle(102, 82, 16, 16),
				
					//right
					new Rectangle(122, 82, 16, 16),
					new Rectangle(142, 82, 16, 16)
				}, startPosition, levelMap)
		{
			TimeInHome = 3;
		}

	}
}
