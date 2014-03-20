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
using System.Collections.Generic;

namespace AIChallengeFramework
{
	/// <summary>
	/// To build your own bot, simply implement this interface with its methods.
	/// </summary>
	public interface IBot
	{
		/// <summary>
		/// At the start of the game, the game engine presents you with twelve
		/// regions from which you can choose your starting position. You MUST
		/// return 6 preferred starting regions. If you fail to match this number,
		/// the parser will choose for you.
		/// </summary>
		/// <returns>The six starting regions.</returns>
		/// <param name="regions">Regions.</param>
		List<Region> PreferredStartingRegions (List<Region> regions);

		/// <summary>
		/// The game engine asks the bot every turn which regions he wants to
		/// put his new units. You do not have to put those new units on the
		/// field, and can simply return an empty list, but in this case you
		/// loose the units forever.
		/// </summary>
		/// <returns>The armies.</returns>
		List<PlaceArmiesMove> PlaceArmies (int armies);

		/// <summary>
		/// The game engine asks the bot every turn for his attack or transfers
		/// moves. Create a list of your moves and return for further processing
		/// by the parser.
		/// </summary>
		/// <returns>The or transfer.</returns>
		List<AttackTransferMove> AttackOrTransfer ();
	}
}

