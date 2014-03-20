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
	/// Every round exists of different moves. Players move alternating, and
	/// normally do two moves per turn: place armies and attack.
	/// </summary>
	public abstract class Move
	{
		/// <summary>
		/// Every move belongs to a player.
		/// </summary>
		/// <value>The player.</value>
		public string Player { get; private set; }

		public Move (string player)
		{
			Player = player;
		}

		public abstract string Parse ();
	}
}

