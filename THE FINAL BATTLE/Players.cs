using CharacterActions;
using Characters;
using Management;
using Items;

namespace Players;

/* This namespace contains all logic involved with PLAYER INTERFACING.
 * Anything where a player, or the AI, would make a choice is contained to here.
 */


public abstract class Player
{
    public abstract List<Character> Party { get; set; }
    public abstract List<Item> Inventory { get; set; }
    public abstract List<Gear> GearInventory { get; set; }
    public abstract Actions PickAction(Character character);
    public abstract Character PickTarget(List<Character> targetList);
    public abstract int PickItem(int inventorySize);
}

public class AIPlayer : Player
{
    private readonly Random random = new Random();

    private bool Heal = false;
    private bool EquipStuff = false;

    public override List<Character> Party { get; set; } = new List<Character>();
    public override List<Item> Inventory { get; set; } = new List<Item>();
    public override List<Gear> GearInventory { get; set; } = new List<Gear>();
    
    public override Actions PickAction(Character character)
    {
        if (CheckHeal())
        {
            int useHeal = random.Next(5);
            if (useHeal == 1 && character.ActionList.Contains(Actions.UseItem) && Inventory.Contains(Item.HealthPotion))
            {
                Heal = true;
                return Actions.UseItem;
            }
        }
        else if (CheckEquip())
        {
            int equipSomething = random.Next(2);
            if (equipSomething == 1 && character.ActionList.Contains(Actions.Equip) && GearInventory.Count > 0)
            {
                EquipStuff = true;
                return Actions.Equip;
            }
        }
        Actions choice = character.ActionList[random.Next(character.ActionList.Count)];

        if (choice == Actions.StandardAtk && character.ActionList.Contains(Actions.Special))
        {
            return Actions.Special;
        }

        return choice;
    }
    public override Character PickTarget(List<Character> targetList)
    {
        if (Heal && Inventory.Contains(Item.HealthPotion)) return HealWounded();
        else if (EquipStuff && GearInventory.Count > 0) return EquipSomeone();

        int input = random.Next(targetList.Count);
        return targetList[input];
    }
    public override int PickItem(int inventorySize)
    {
        if (Heal) return Inventory.IndexOf(Item.HealthPotion);
        int input = random.Next(inventorySize);
        return input;
    }
    private bool CheckHeal()
    {
        foreach (Character ally in Party)
        {
            if (ally.HP < ally.MaxHP / 2)
            {
                return true;
            }
        }
        return false;
    }
    private Character HealWounded()
    {
        foreach(Character ally in Party)
        {
            if (ally.HP < ally.MaxHP / 2) 
            {
                Heal = false;
                return ally; 
            }
        }
        //Fallback case: Return a random character from the party.
        return Party[random.Next(Party.Count)];
    }
    private bool CheckEquip()
    {
        foreach (Character ally in Party)
        {
            if (ally.EquippedGear == Gear.None)
            {
                return true;
            }
        }
        return false;
    }
    private Character EquipSomeone()
    {
        Character guyToEquip = Party[random.Next(Party.Count)];
        foreach (Character ally in Party)
        {
            if (ally.EquippedGear == Gear.None)
            {
                EquipStuff = false;
                guyToEquip = ally;
            }
        }
        return guyToEquip;
    }
}


public class HumanPlayer : Player
{
    public override List<Character> Party { get; set; } = new List<Character>();
    public override List<Item> Inventory { get; set; } = new List<Item>();
    public override List<Gear> GearInventory { get; set; } = new List<Gear>();

    public override Actions PickAction(Character character)
    {
        Display.PrintActions(character);
        int input = Convert.ToInt32(Console.ReadLine());
        while (input < 1 || input > character.ActionList.Count)
        {
            Console.WriteLine("You can't do that.\n");
            input = Convert.ToInt32(Console.ReadLine());
        }
        return character.ActionList[input - 1];
    }
    public override Character PickTarget(List<Character> targetList)
    {
        Display.PrintTargetList(targetList);
        int input = Convert.ToInt32(Console.ReadLine());
        while (input > targetList.Count || input <= 0)
        {
            Console.WriteLine($"Sorry, that target doesn't exist. Pick again.\n");
            Display.PrintTargetList(targetList);
            input = Convert.ToInt32(Console.ReadLine());
        }
        return targetList[input - 1];
    }
    public override int PickItem(int inventorySize)
    {
        int input = Convert.ToInt32(Console.ReadLine());
        while (input < 1 || input > inventorySize)
        {
            Console.WriteLine($"You don't have an item in slot {input}. Pick something else!\n");
            input = Convert.ToInt32(Console.ReadLine());
        }
        return input - 1;
    }
}