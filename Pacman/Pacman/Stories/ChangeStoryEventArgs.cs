using System;

namespace Pacman.Stories
{
	public class ChangeStoryEventArgs : EventArgs
	{
		public string NextStoryName { get; private set; }

		public ChangeStoryEventArgs(string nextStoryName)
		{
			NextStoryName = nextStoryName;
		}
	}
}
