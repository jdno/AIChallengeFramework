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
	/// This is a small logging facility that can be used for troubleshooting.
	/// </summary>
	public static class Logger
	{
		/// <summary>
		/// These are the log level supported by this project.
		/// </summary>
		public enum Severity {
			OFF = 0,
			ERROR = 1,
			INFO = 2,
			DEBUG = 3
		}

		/// <summary>
		/// Choose the severity you would like to have logged.
		/// </summary>
		public static Severity LogLevel { get; private set; }

		/// <summary>
		/// Initialize this instance.
		/// </summary>
		public static void Initialize ()
		{
			LogLevel = Severity.ERROR;
			Logger.Info ("Logger:\tInitialized.");
		}

		/// <summary>
		/// Efficiently check if the log level is DEBUG.
		/// </summary>
		/// <returns><c>true</c> if is DEBUG; otherwise, <c>false</c>.</returns>
		public static bool IsDebug ()
		{
			if (LogLevel == Severity.DEBUG) {
				return true;
			} else {
				return false;
			}
		}

		/// <summary>
		/// Log the message only if the log level is error.
		/// </summary>
		/// <param name="message">Message.</param>
		public static void Error (string message)
		{
			if (LogLevel >= Severity.ERROR)
				print (message);
		}

		/// <summary>
		/// Log the message only if the log level is info or error.
		/// </summary>
		/// <param name="message">Message.</param>
		public static void Info (string message)
		{
			if (LogLevel >= Severity.INFO)
				print (message);
		}

		/// <summary>
		/// Log the message only if the log level is debug, info or error.
		/// </summary>
		/// <param name="message">Message.</param>
		public static void Debug (string message)
		{
			if (LogLevel >= Severity.DEBUG)
				print (message);
		}

		/// <summary>
		/// Print the specified message.
		/// </summary>
		/// <param name="message">Message.</param>
		private static void print (string message) {
			Console.Error.WriteLine (message);
		}
	}
}

