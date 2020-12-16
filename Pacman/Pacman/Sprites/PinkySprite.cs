using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pacman.Sprites
{
	public class PinkySprite : GhostSprite
	{
		public PinkySprite(Texture2D texture2D, Vector2 startPosition, ILevelMap levelMap)
			: base(texture2D, new[]
				{
					//neutral
					new Rectangle(2, 102, 16, 16),
				
					//up
					new Rectangle(2, 102, 16, 16),
					new Rectangle(22, 102, 16, 16),

					//down
					new Rectangle(42, 102, 16, 16),
					new Rectangle(62, 102, 16, 16),

					//left
					new Rectangle(82, 102, 16, 16),
					new Rectangle(102, 102, 16, 16),
				
					//right
					new Rectangle(122, 102, 16, 16),
					new Rectangle(142, 102, 16, 16)
				}, startPosition, levelMap)
		{
			TimeInHome = 8;
		}
	}
}
