using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pacman.Sprites
{
	public class InkySprite : GhostSprite
	{
		public InkySprite(Texture2D texture2D, Vector2 startPosition, ILevelMap levelMap)
			: base(texture2D, new[]
				{
					//neutral
					new Rectangle(2, 122, 16, 16),
				
					//up
					new Rectangle(2, 122, 16, 16),
					new Rectangle(22, 122, 16, 16),

					//down
					new Rectangle(42, 122, 16, 16),
					new Rectangle(62, 122, 16, 16),

					//left
					new Rectangle(82, 122, 16, 16),
					new Rectangle(102, 122, 16, 16),
				
					//right
					new Rectangle(122, 122, 16, 16),
					new Rectangle(142, 122, 16, 16)
				}, startPosition, levelMap)
		{
			TimeInHome = 12;
		}
	}
}
