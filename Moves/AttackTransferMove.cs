//
//  Copyright 2014  jdno
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
using System;

namespace AIChallengeFramework
{
	/// <summary>
	/// The game engine does not distinguish between attacking another country
	/// and reinforcing it. In both cases, it expects the source and target
	/// region, and the amount of armies you want to move. If the country is
	/// owned by your enemy, it will get attacked. If it is owned by you, it
	/// gets reinforced.
	/// </summary>
	public class AttackTransferMove : Move
	{
		/// <summary>
		/// The region from which to move the armies.
		/// </summary>
		/// <value>The source region.</value>
		public Region SourceRegion { get; private set; }

		/// <summary>
		/// The region to which to move the armies.
		/// </summary>
		/// <value>The target region.</value>
		public Region TargetRegion { get; private set; }

		/// <summary>
		/// The amount of armies that gets moved.
		/// </summary>
		/// <value>The armies.</value>
		public int Armies { get; private set; }

		public AttackTransferMove (string player, Region sourceRegion, Region targetRegion, int armies) : base (player)
		{
			SourceRegion = sourceRegion;
			TargetRegion = targetRegion;
			Armies = armies;
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that can be parsed by the game engine.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that can be parsed by the game engine.</returns>
		public override string Parse ()
		{
			return string.Format ("{0} attack/transfer {1} {2} {3}", Player, SourceRegion.Id, TargetRegion.Id, Armies);
		}
	}
}

