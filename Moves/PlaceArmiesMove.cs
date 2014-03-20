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
	/// At the start of every turn, a player can reinforce his regions. This
	/// is done by telling the game engine the region and the number of
	/// armies that should get added to it.
	/// </summary>
	public class PlaceArmiesMove : Move
	{
		/// <summary>
		/// Region in which the armies get placed.
		/// </summary>
		/// <value>The region.</value>
		public Region Region { get; private set; }

		/// <summary>
		/// The number of armies that get placed.
		/// </summary>
		/// <value>The armies.</value>
		public int Armies { get; private set; }

		public PlaceArmiesMove (string player, Region region, int armies) : base (player)
		{
			Region = region;
			Armies = armies;

			if (Logger.IsDebug ()) {
				Logger.Debug (string.Format ("PlaceArmiesMove:\tInitialized by player {0} who puts {1} armies into region {2}.",
					player, armies, region.Id));
			} else {
				Logger.Info ("PlaceArmiesMove:\tInitialized.");
			}
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that can be parsed by the game engine.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that can be parsed by the game engine.</returns>
		public override string Parse ()
		{
			return string.Format ("{0} place_armies {1} {2}", Player, Region.Id, Armies);
		}
	}
}

