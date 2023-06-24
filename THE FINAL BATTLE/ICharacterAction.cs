using Characters;
using Players;
using Items;
using System.Xml.Linq;

namespace CharacterActions;

/* Contains all logic involved with PERFORMING ACTIONS. All attacks are self-contained and handle their own stats, excluding target selection, which is handled via PLAYERS.
 * All character actions are routed through ICharacterAction to streamline execution.
 */
public interface ICharacterAction
{
    public string Name { get; }
    void Run(Character character, Player? player = null, List<Character>? hostiles = null, List<Character>? friendlies = null);
}

public class DoNothing : ICharacterAction
{
    public string Name { get; } = "Do Nothing";
    public void Run(Character character, Player? player = null, List<Character>? hostiles = null, List<Character>? friendlies = null)
    {
        Random random = new Random();
        int number = random.Next(10);
        
        switch (number)
        {
            case 0: Console.WriteLine($"{character.Name} is, uh, they- uhm... so eh... yeah."); return;
            case 1: Console.WriteLine($"{character.Name} is just kinda looking around..."); return;
            case 2: Console.WriteLine($"{character.Name} is taking a quick powernap."); return;
            case 3: Console.WriteLine($"{character.Name} is wondering whether they left the oven on..."); return;
            case 4: Console.WriteLine($"{character.Name}'s brain has temporarily evacuated the area."); return;
            case 5: Console.WriteLine($"{character.Name} is rebooting..."); return;
            case 6: Console.WriteLine($"{character.Name} is struck by a sudden onset of existential dread..."); return;
            case 7: Console.WriteLine($"{character.Name} has remembered something embarrassing from {random.Next(20)} years ago..."); return;
            case 8: Console.WriteLine($"{character.Name} is taking a quick phone call."); return;
            case 9: Console.WriteLine($"{character.Name} is just not feeling it right now."); return;
        }
    }
}

public class UseItem : ICharacterAction
{
    public string Name { get; } = "Use Item";
    
    public void Run(Character character, Player? player, List<Character>? hostiles, List<Character>? friendlies)
    {
        if (player.Inventory.Count == 0) return;
        if (player.GetType() == typeof(HumanPlayer)) Display.PrintInventory(player.Inventory);
        Item itemType = player.Inventory[player.PickItem(player.Inventory.Count)];
        Consumable? item = ReturnItem(itemType);
        item.Use(character, player, friendlies, hostiles);
        player.Inventory.Remove(itemType);
    }
    public  static Consumable? ReturnItem(Item item)
    {
        return item switch
        {
            Item.HealthPotion => new HealthPotion(),
            Item.Poison => new PoisonFlask(),
            Item.RegenPotion => new RegenPotion(),
            Item.Soup => new SimulaSoup(),
            _ => null
        };
    }
}
public class Equip : ICharacterAction
{
    public string Name { get; } = "Equip";

    //Displays the list of all gear in the player's inventory, lets them pick one, then equips it to the character and sets them up to use a Special move if they couldn't previously.
    public void Run(Character character, Player? player, List<Character>? hostiles = null, List<Character>? friendlies = null)
    {
        if (player.GearInventory.Count == 0) return;
        if (player.GetType() == typeof(HumanPlayer)) Display.PrintGear(player.GearInventory);
        Gear gearType = player.GearInventory[player.PickItem(player.GearInventory.Count)];
        if (character.EquippedGear != Gear.None)
        {
            Console.WriteLine($"{character.Name} has unequipped {ReturnGear(character.EquippedGear).Name}...");
            player.GearInventory.Add(character.EquippedGear);
        }
        character.EquippedGear = gearType;
        Console.WriteLine($"{character.Name} has equipped {ReturnGear(gearType).Name}!");

        if (!character.ActionList.Contains(Actions.Special)) character.ActionList.Insert(1, Actions.Special);
        player.GearInventory.Remove(gearType);
    }
    public static Equippable? ReturnGear (Gear gear)
    {
        return gear switch
        {
            Gear.Sword => new Sword(),
            Gear.Dagger => new Dagger(),
            Gear.VinBow => new VinBow(),
            Gear.OopsieDaisy => new OopsieDaisy(),
            Gear.StoneClaw => new StoneClaw(),
            Gear.None => new None(),
            _ => null
        };
    }
}

public class SpecialAttack : ICharacterAction
{
    public string Name { get; }
    private Equippable _Item { get; }
    public SpecialAttack(Equippable item)
    {
        _Item = item;
        Name = item.Skill.Name;
    }
    public void Run(Character character, Player? player, List<Character>? hostiles, List<Character>? friendlies)
    {
        _Item.Skill.Run(character, player, hostiles, friendlies);
    }
}

public abstract class Attack : ICharacterAction
{
    public abstract string Name { get; }
    public abstract DamageType Type { get; }
    public abstract int BaseDMG { get; }
    public abstract int TrueDMG { get; set; }
    public abstract double Accuracy { get; }
    public abstract double CritRate { get; }
    public abstract void Run(Character character, Player? player = null, List<Character>? hostiles = null, List<Character>? friendlies = null);
}

public class Punch : Attack
{
    public override string Name { get; } = "PUNCH";
    public override DamageType Type { get; } = DamageType.Normal;
    public override int BaseDMG { get; } = 1;
    public override int TrueDMG { get; set; } = 1;
    public override double Accuracy { get; } = 1.0;
    public override double CritRate { get; } = 0.1;
    public override void Run(Character character, Player? player, List<Character>? hostiles, List<Character>? friendlies = null)
    {
        Character target = player.PickTarget(hostiles);
        Display.PrintAttack(character, target, this);
        AttackUtilities.ResolveAttack(character, target, this);
    }
}
public class BoneCrunch : Attack
{
    Random random = new Random();
    public override string Name { get; } = "BONE CRUNCH";
    public override DamageType Type { get; } = DamageType.Normal;
    public override int BaseDMG { get; } = 0;
    public override int TrueDMG { get; set; }
    public override double Accuracy { get; } = 1.0;
    public override double CritRate { get; } = 0.2;
    public override void Run(Character character, Player? player, List<Character>? hostiles, List<Character>? friendlies = null)
    {
        TrueDMG = BaseDMG + random.Next(2);
        Character target = player.PickTarget(hostiles);
        Display.PrintAttack(character, target, this);
        AttackUtilities.ResolveAttack(character, target, this);
    }
}
public class Bite : Attack
{
    public override string Name { get; } = "BITE";
    public override DamageType Type { get; } = DamageType.Normal;
    public override int BaseDMG { get; } = 1;
    public override int TrueDMG { get; set; } = 1;
    public override double Accuracy { get; } = 1.0;
    public override double CritRate { get; } = 0.3;
    public override void Run(Character character, Player? player, List<Character>? hostiles, List<Character>? friendlies = null)
    {
        Character target = player.PickTarget(hostiles);
        Display.PrintAttack(character, target, this);
        AttackUtilities.ResolveAttack(character, target, this);
    }
}

public class Unravel : Attack
{
    Random random = new Random();
    public override string Name { get; } = "UNRAVEL";
    public override DamageType Type { get; } = DamageType.Decoding;
    public override int BaseDMG { get; } = 0;
    public override int TrueDMG { get; set; }
    public override double Accuracy { get; } = 1.0;
    public override double CritRate { get; } = 0.4;

    public override void Run(Character character, Player? player, List<Character>? hostiles, List<Character>? friendlies = null)
    {
        TrueDMG = BaseDMG + random.Next(7);
        Character target = player.PickTarget(hostiles);
        Display.PrintAttack(character, target, this);
        AttackUtilities.ResolveAttack(character, target, this);
    }
}
public class Slash : Attack
{
    public override string Name { get; } = "SLASH";
    public override DamageType Type { get; } = DamageType.Normal;
    public override int BaseDMG { get; } = 2;
    public override int TrueDMG { get; set; } = 2;
    public override double Accuracy { get; } = 1.0;
    public override double CritRate { get; } = 0.1;
    public override void Run(Character character, Player? player, List<Character>? hostiles, List<Character>? friendlies = null)
    {
        Character target = player.PickTarget(hostiles);
        Display.PrintAttack(character, target, this);
        AttackUtilities.ResolveAttack(character, target, this);
    }
}
public class Stab : Attack
{
    public override string Name { get; } = "STAB";
    public override DamageType Type { get; } = DamageType.Normal;
    public override int BaseDMG { get; } = 1;
    public override int TrueDMG { get; set; } = 1;
    public override double Accuracy { get; } = 1.0;
    public override double CritRate { get; } = 0.1;
    public override void Run(Character character, Player? player, List<Character>? hostiles, List<Character>? friendlies = null)
    {
        Character target = player.PickTarget(hostiles);
        Display.PrintAttack(character, target, this);
        AttackUtilities.ResolveAttack(character, target, this);
    }
}
public class Rend : Attack
{
    public override string Name { get; } = "REND";
    public override DamageType Type { get; } = DamageType.Normal;
    public override int BaseDMG { get; } = 2;
    public override int TrueDMG { get; set; } = 2;
    public override double Accuracy { get; } = 1.0;
    public override double CritRate { get; } = 0.2;
    public override void Run(Character character, Player? player, List<Character>? hostiles, List<Character>? friendlies = null)
    {
        Character target = player.PickTarget(hostiles);
        Display.PrintAttack(character, target, this);
        AttackUtilities.ResolveAttack(character, target, this);
    }
}
public class QuickShot : Attack
{
    public override string Name { get; } = "QUICK SHOT";
    public override DamageType Type { get; } = DamageType.Normal;
    public override int BaseDMG { get; } = 3;
    public override int TrueDMG { get; set; } = 3;
    public override double Accuracy { get; } = 0.5;
    public override double CritRate { get; } = 0.5;
    public override void Run(Character character, Player? player, List<Character>? hostiles, List<Character>? friendlies = null)
    {
        Character target = player.PickTarget(hostiles);
        Display.PrintAttack(character, target, this);
        AttackUtilities.ResolveAttack(character, target, this);
    }
}

public class BigOopsie : Attack
{
    public override string Name { get; } = "BIG OOPSIE";
    public override DamageType Type { get; } = DamageType.Decoding;
    public override int BaseDMG { get; } = 0;
    public override int TrueDMG { get; set; }
    public override double Accuracy { get; } = 0.9;
    public override double CritRate { get; } = 0.0;
    public override void Run(Character character, Player? player, List<Character>? hostiles, List<Character>? friendlies)
    {
        TrueDMG = hostiles.Count + friendlies.Count;
        Character target = player.PickTarget(hostiles);
        Display.PrintAttack(character, target, this);
        AttackUtilities.ResolveAttack(character, target, this);
    }
}
public static class AttackUtilities
{
    public static void ResolveAttack(Character user, Character target, Attack attack)
    {
        Random random = new Random();
        double hitChance = random.NextDouble();
        double critChance = random.NextDouble();
        if (attack.Accuracy >= hitChance)
        {
            int damage = CheckCrit(target, attack, critChance);
            target.TakeDamage(damage);
            if (target.HP > 0) Display.PrintAttackResolution(target, attack, damage);
        }
        else Console.WriteLine($"{user.Name} MISSED!");
    }
    
    private static int CheckCrit(Character target, Attack attack, double critChance)
    {
        if (attack.CritRate > critChance)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("It's a CRIT!");
            if (attack.GetType() == typeof(Rend))
            {
                target.StatusEffects.Add((Status.Bleed, 2));
                Console.WriteLine($"{target.Name} is bleeding!");
            }
            Console.ForegroundColor = ConsoleColor.White;
            return ApplyModifier(target, attack) * 2;
        }
        else return ApplyModifier(target, attack);
    }
    private static int ApplyModifier(Character target, Attack attack)
    {
        if (target.DefenseMod != null)
        {
            int modifiedDamage = target.DefenseMod.ApplyModifier(attack);
            if(attack.TrueDMG - modifiedDamage != 0) Console.WriteLine($"{target.DefenseMod.Name} has reduced damage by {target.DefenseMod.Mod}!");
            return modifiedDamage;
        }
        else return attack.TrueDMG;
    }
}

public enum Actions {Nothing, StandardAtk, Special, UseItem, Equip}
public enum DamageType { Normal, Decoding }