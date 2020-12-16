using Microsoft.Xna.Framework;

namespace Pacman.Sprites
{
	public interface ILevelMap
	{
		void Eat(Point point);
		bool Collides(Sprite sprite);
		bool Collides(Sprite sprite, SpriteDirection spriteDirection);
		bool TryTeleport(Sprite sprite, out Vector2 teleportOutPosition);
		bool IsGhostInHome(GhostSprite ghostSprite);
	}
}
