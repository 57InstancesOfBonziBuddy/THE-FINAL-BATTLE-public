using Characters;
using Players;
using Items;
using Management;
using CharacterActions;

Console.Title = "The Final Battle";
Display display = new Display();
Random random = new Random();
GameManager gameManager = GameModeSelect();

RunGame();



void RunGame()
{
    gameManager.BuildHeroInventory();
    while (gameManager.GameOver == false) {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"Battle {gameManager.BattleNumber} begins!");
        gameManager.EvilPlayer.Party = gameManager.BuildBaddieParty();
        foreach (Character character in gameManager.EvilPlayer.Party) gameManager.CheckPreEquipped(character);
        gameManager.BuildBaddieInventory();
        while (gameManager.InBattle)
        {
            gameManager.Battle();
        }
       gameManager.CheckWinner();
    }
}

GameManager GameModeSelect()
{
    Console.WriteLine("Select your game mode! Player 1 will control the heroes, player 2 will be in charge of the Uncoded One's forces.\n1 - Player vs. Computer (default)\n2 - Computer vs. Computer\n3 - Player vs. Player");
    int input = Convert.ToInt32(Console.ReadLine());
    while(input > 3 || input < 1)
    {
        Console.WriteLine("That is not an option. Please pick one of the following:\n1 - Player vs. Computer (default)\n2 - Computer vs. Computer\n3 - Player vs. Player");
        input = Convert.ToInt32(Console.ReadLine());
    }
    return input switch
    {
        1 => new GameManager(new HumanPlayer(), new AIPlayer(), display),
        2 => new GameManager(new AIPlayer(), new AIPlayer(), display),
        3 => new GameManager(new HumanPlayer(), new HumanPlayer(), display),
        _ => new GameManager(new HumanPlayer(), new AIPlayer(), display)
    } ;
}

public record Display
{
    private readonly System.Text.StringBuilder text = new System.Text.StringBuilder();

    public void DisplayStatus(Character activeCharacter, GameManager gm)
    {
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.WriteLine($"############################## BATTLE {gm.BattleNumber} ##############################");
        Console.ForegroundColor = ConsoleColor.White;
        RenderHeroParty(gm.HeroPlayer.Party, activeCharacter);
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("------------------------------- VERSUS -------------------------------");
        Console.ForegroundColor= ConsoleColor.White;
        RenderBaddieParty(gm.EvilPlayer.Party, activeCharacter);
    }
    public void RenderHeroParty(List<Character> party, Character activeCharacter)
    {
        foreach (Character character in party)
        {
            if (character == activeCharacter) Console.ForegroundColor = ConsoleColor.Yellow;
            text.Append($"{character.Name} [{Equip.ReturnGear(character.EquippedGear).Name}]");
            for (int i = 0; i < 25 - text.Length; i++)
                text.Append(' ');
            text.Append($"{character.HP}/{character.MaxHP} HP");
            Console.Write(text.ToString());
            if (character.HP < character.MaxHP / 4)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(" [!]");
            }
            Console.WriteLine("");
            Console.ForegroundColor= ConsoleColor.White;
            text.Clear();
        }
    }

    public void RenderBaddieParty(List<Character> party, Character activeCharacter)
    {
        foreach (Character character in party)
        {
            if (character == activeCharacter) Console.ForegroundColor = ConsoleColor.Magenta;
            for (int i = 0; i < 53 - character.Name.Length; i++)
                text.Append(' ');
            text.Append($"[{Equip.ReturnGear(character.EquippedGear).Name}] {character.Name}");
            text.Append($"  {character.HP}/{character.MaxHP} HP");
            Console.WriteLine(text.ToString());
            Console.ForegroundColor = ConsoleColor.White;
            text.Clear();
        }
    }
    public static void PrintAttack(Character user, Character target, Attack attack)
    {
        Console.WriteLine($"{user.Name} used {attack.Name} on {target.Name}!");
    }
    public static void PrintAttackResolution(Character target, Attack attack, int damage)
    {
        Console.WriteLine($"{attack.Name} dealt {damage} damage to {target.Name}!");
        Console.WriteLine($"{target.Name} is now at {target.HP}/{target.MaxHP} HP.");
    }
    public static void PrintTargetList(List<Character> targets)
    {
        Console.WriteLine("Pick a target!");
        foreach (Character character in targets)
        {
            Console.WriteLine($"{targets.IndexOf(character) + 1} - {character.Name}");
        }
    }
    public static void PrintActions(Character character)
    {
        Console.WriteLine("What will you do?");
        int index = 1;
        foreach (Actions action in character.ActionList)
        {
            ICharacterAction option = Utilities.TakeAction(character, action);
            Console.WriteLine($"{index} - {option.Name}");
            index++;
        }
    }
    public static void PrintInventory(List<Item> inventory)
    {
        Console.WriteLine("Pick an item to use:");
        int index = 1;
        foreach (Item item in inventory)
        {
            Consumable currentItem = UseItem.ReturnItem(item);
            Console.Write($"{index} - {currentItem.Name}    Effect: ");
            ShowDescription(currentItem.Description);
            index++;
        }
    }
    public static void PrintGear(List<Gear> gearList)
    {
        Console.WriteLine("Select the item you wish to equip to this character:");
        int index = 1;
        foreach (Gear item in gearList)
        {
            Equippable currentGear = Equip.ReturnGear(item);
            Console.Write($"{index} - {currentGear.Name}    Effect: ");
            ShowDescription(currentGear.Description);
            index++;
        }
    }
    private static void ShowDescription(string text)
    {
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine(text);
        Console.ForegroundColor = ConsoleColor.White;
    }
}