using Characters;
using CharacterActions;
using Players;
using Items;

namespace Management;


//Keeps track of various critical stats of the game used to determine when the game ends and who won, as well as which faction is controlled by which type of player.
public class GameManager
{
    public Player HeroPlayer { get; }
    public Player EvilPlayer { get; }
    private Display _Display;
    
    public Faction ActiveFaction { get; set; } = Faction.Good;
    public Player ActivePlayer { get; set; }
    public Character ActiveCharacter { get; set; }


    public bool InBattle { get; set; } = true;
    public int BattleNumber { get; set; } = 1;
    public bool GameOver { get; set; } = false;

    public GameManager(Player heroPlayer, Player evilPlayer, Display display)
    {
        HeroPlayer = heroPlayer;
        EvilPlayer = evilPlayer;
        _Display = display;
        ActivePlayer = HeroPlayer;
        HeroPlayer.Party = BuildHeroParty();
    }
    public void CharacterTurn(Player player, List<Character> friendlies, List<Character> hostiles)
    {
        Console.WriteLine($"It is {ActiveCharacter.Name}'s turn...");
        ICharacterAction action = Utilities.TakeAction(ActiveCharacter, player.PickAction(ActiveCharacter));
        //restricts available targets based on action type
        while (action.GetType() == typeof(UseItem) && ActivePlayer.Inventory.Count == 0)
        {
            Console.WriteLine("You don't have any items to use!\n");
            action = Utilities.TakeAction(ActiveCharacter, player.PickAction(ActiveCharacter));
        }
        while (action.GetType() == typeof(Equip) && ActivePlayer.GearInventory.Count == 0)
        {
            Console.WriteLine("You don't have any gear to equip!\n");
            action = Utilities.TakeAction(ActiveCharacter, player.PickAction(ActiveCharacter));
        }

        switch (action)
        {
            case DoNothing:
                action.Run(ActiveCharacter); break;
            case Attack:
                action.Run(ActiveCharacter, player, hostiles); break;
            case SpecialAttack:
                action.Run(ActiveCharacter, player, hostiles, friendlies); break;
            case UseItem:
                action.Run(ActiveCharacter, player, hostiles, friendlies); break;
            case Equip:
                action.Run(ActiveCharacter, player); break;
        }
        Utilities.ResolveStatusEffects(ActiveCharacter);
        Console.WriteLine("");
        Thread.Sleep(1000);
    }
    public void EndTurn()
    {
        if (HeroPlayer.Party.Count == 0 || EvilPlayer.Party.Count == 0) InBattle = false;

        if (ActiveFaction == Faction.Good)
        {
            ActiveFaction = Faction.Evil;
            ActivePlayer = EvilPlayer;
        }
        else if (ActiveFaction == Faction.Evil)
        {
            ActiveFaction = Faction.Good;
            ActivePlayer = HeroPlayer;
        }
    }
    public void PlayerTurn(Player player, List<Character> activeParty, List<Character> hostileParty)
    {
        foreach (Character character in activeParty)
        {
            ActiveCharacter = character;
            if (player.GetType() == typeof(HumanPlayer)) _Display.DisplayStatus(character, this);
            CharacterTurn(player, activeParty, hostileParty);
            if (EvilPlayer.Party.Count == 0) break;
        }
        EndTurn();
    }
    public void Battle()
    {
        if (ActiveFaction == Faction.Good)
        {
            PlayerTurn(HeroPlayer, HeroPlayer.Party, EvilPlayer.Party);
        }
        else if (ActiveFaction == Faction.Evil)
        {
            PlayerTurn(EvilPlayer, EvilPlayer.Party, HeroPlayer.Party);
        }
    }
    public void CheckWinner()
    {
        if (HeroPlayer.Party.Count > 0 && EvilPlayer.Party.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"Wave {BattleNumber} has been defeated!\n");
            BattleNumber++;
            if (BattleNumber >= 5)
            {
                GameOver = true;
                Console.WriteLine("The last of the Uncoded One's forces have fallen. The thing itself crumbles, stutters, and finally erupts in a fantastic, glitchy explosion. You saved the day!");
            }
            else
            {
                InBattle = true;
                ActiveFaction = Faction.Good;
                LootInventory();
            }
            
        }
        else if (HeroPlayer.Party.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("The heroes have fallen! The Uncoded One's forces have prevailed!");
            GameOver = true;
        }
    }
    public  List<Character> BuildHeroParty()
    {
        List<Character> party = new List<Character>
        {
            new TrueProgrammer(),
            new VinFletcher()
        };
        Mortalize(party);

        return party;
    }
    public  List<Character> BuildBaddieParty()
    {
        switch (BattleNumber)
        {
            case 1:
                List<Character> party1 = new List<Character> { new Skeleton("SKELETON", Gear.Dagger), new StoneAmarok("STONE AMAROK") };
                Mortalize(party1);
                return party1;
            case 2:
                List<Character> party2 = new List<Character>
                {
                    new Skeleton("SKELETON A", Gear.None),
                    new Skeleton("SKELETON B", Gear.None),
                    new Skeleton("SKELETON C", Gear.None)
                };
                Mortalize(party2);
                return party2;
            case 3:
                List<Character> party3 = new List<Character>
                {
                    new StoneAmarok("STONE AMAROK A"), 
                    new StoneAmarok("STONE AMAROK B") 
                };
                Mortalize(party3);
                return party3;
            case 4:
                List<Character> party4 = new List<Character> { new UncodedOne(), new Skeleton("BONER THE TERRIBLE", Gear.Sword), new StoneAmarok("POOCH"), new StoneAmarok("SNUFFLES") };
                Mortalize(party4);
                return party4;
            default: return new List<Character>();
        }
    }

    public void BuildHeroInventory()
    {
        HeroPlayer.Inventory = new List<Item> { Item.Soup, Item.HealthPotion, Item.RegenPotion, Item.RegenPotion, Item.Poison};
        HeroPlayer.GearInventory = new List<Gear> { Gear.OopsieDaisy };
    }

    public void BuildBaddieInventory()
    {
        switch (BattleNumber)
        {
            case 1:
                EvilPlayer.Inventory = new List<Item> { Item.HealthPotion };
                EvilPlayer.GearInventory = new List<Gear>();
                break;
            case 2:
                EvilPlayer.Inventory = new List<Item> { Item.RegenPotion, Item.Poison, Item.Poison };
                EvilPlayer.GearInventory = new List<Gear> { Gear.Dagger, Gear.Dagger};
                break;
            case 3:
                EvilPlayer.Inventory = new List<Item> { Item.HealthPotion, Item.HealthPotion };
                EvilPlayer.GearInventory = new List<Gear> ();
                break;
            default:
                EvilPlayer.Inventory = new List<Item>();
                EvilPlayer.GearInventory = new List<Gear>();
                break;
        };
    }
    public void LootInventory()
    {
        if (EvilPlayer.Inventory.Count > 0)
        {
            Console.WriteLine("You looted the following items:");
            foreach (Item item in EvilPlayer.Inventory)
            {
                Consumable lootedItem = UseItem.ReturnItem(item);
                Console.Write($"{lootedItem.Name}\n");
                HeroPlayer.Inventory.Add(item);
            }
        }
        Console.WriteLine("");
        if (EvilPlayer.GearInventory.Count > 0)
        {
            Console.WriteLine("You looted the following equippable gear:");
            foreach (Gear item in EvilPlayer.GearInventory)
            {
                Equippable lootedGear = Equip.ReturnGear(item);
                Console.Write($"{lootedGear.Name}\n");
                HeroPlayer.GearInventory.Add(item);
            }
        }
        Console.WriteLine("");
    }
    public void CheckPreEquipped(Character character)
    {
        if (character.EquippedGear != Gear.None) character.ActionList.Insert(1, Actions.Special);
    }
    private void OnDeath(Character character) => RemoveDead(character);
    private void RemoveDead(Character deadBoi)
    {
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.WriteLine($"{deadBoi.Name} has perished!");
        Console.ForegroundColor = ConsoleColor.White;

        if (deadBoi.Faction == Faction.Good)
        {
            LootCorpse(deadBoi, EvilPlayer.GearInventory);
            HeroPlayer.Party.Remove(deadBoi);
        }
        else if (deadBoi.Faction == Faction.Evil)
        {
            LootCorpse(deadBoi, HeroPlayer.GearInventory);
            EvilPlayer.Party.Remove(deadBoi);
        }
    }
    private void Mortalize(List<Character> party)
    {
        for (int i = 0; i < party.Count; i++)
        {
            party[i].hasDied += OnDeath;
        }
    }
    private void LootCorpse(Character corpse, List<Gear> inventory)
    {
        if (corpse.EquippedGear != Gear.None)
        {
            inventory.Add(corpse.EquippedGear);
            Console.WriteLine($"Plundered {Equip.ReturnGear(corpse.EquippedGear).Name}!");
        }
    }
}

public static class Utilities
{ 
    public static ICharacterAction TakeAction(Character character, Actions action)
    {
        return action switch
        {
            Actions.Nothing => new DoNothing(),
            Actions.StandardAtk => character.StandardAttack,
            Actions.Special => new SpecialAttack(Equip.ReturnGear(character.EquippedGear)),
            Actions.UseItem => new UseItem(),
            Actions.Equip => new Equip(),
            _ => new DoNothing()
        };
    }
    public static void ResolveStatusEffects(Character character)
    {
        List<(Status, int)> toBeRemoved = new List<(Status, int)>();
        for (int i = 0; i < character.StatusEffects.Count; i++) 
        {
            (Status, int) status = character.StatusEffects[i];
            Status currentEffect = status.Item1;
            int duration = status.Item2;
            if (duration <= 0) return;
            switch (currentEffect)
            {
                case Status.Poison:
                    character.TakeDamage(1);
                    Console.WriteLine($"{character.Name} took damage from poison!");
                    break;
                case Status.Regen:
                    character.HP = Math.Clamp(character.HP + 2, 0 , character.MaxHP);
                    Console.WriteLine($"{character.Name} regenerated some health!");
                    break;
                case Status.Bleed:
                    character.TakeDamage(2);
                    Console.WriteLine($"{character.Name} took damage from bleeding!");
                    break;
                default: break;
            }
            duration--;
            if (duration != 0) character.StatusEffects[i] = (currentEffect, duration);
            else toBeRemoved.Add(status);
        }
        foreach ((Status, int) effect in toBeRemoved)
        {
            character.StatusEffects.Remove(effect);
            Console.WriteLine($"{character.Name} has recovered from {effect.Item1}.");
        }
    }
}
