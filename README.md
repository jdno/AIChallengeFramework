# AIChallengeFramework

AIChallengeFramework can be used to implement C# bots for Warlight's AI
challenge. It handles the communication with the game engine and provides
useful information about the state of the game.

Find out more about the challenge here:

http://theaigames.com/competitions/warlight-ai-challenge

## Disclaimer

This is a custom framework for the challenge, and in no way officially or in
any other way related to either Warlight or Conquest.

## Usage

1. Create a console application and add a reference to this library
2. Create your bot's class file, and add the namespace AIChallengeFramework
3. Have your bot extend the class Bot
4. Start your application with something like this:

````C#
public static void Main(String[] args)
{
	State state = new State ();
	YourBot bot = new YourBot (state);

	state.MyBot = bot;
	Parser parser = new Parser (bot, state);

	parser.Run ();
}
````

## Customization

If you want to customize the behavior of the framework, you have several options:

1. Extend the corresponding classes
2. Fork the repository

Below is a list of methods that you can override by default:

* Bot
    * virtual public List<Region> PreferredStartingRegions (List<Region> regions)
    * virtual public List<PlaceArmiesMove> PlaceArmies ()
    * virtual public List<AttackTransferMove> AttackOrTransfer ()
* Continent
    * virtual public int Priority ()

If you need more control, you have to fork the repository and create your own
version of the framework.

## Documentation

Visit the project's Wiki on GitHub:

https://github.com/jdno/AIChallengeFramework/wiki

## Issue reporting

If you are experiencing any problems, please open an issue on GitHub:
https://github.com/jdno/AIChallengeFramework/issues

## License

The project is licensed under the Apache License, Version 2.0

## Copyright

2014 jdno (https://github.com/jdno/)

## Enjoy

Have fun & good luck!