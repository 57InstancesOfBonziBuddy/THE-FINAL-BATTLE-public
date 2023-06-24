using CharacterActions;
using Characters;
using Players;

namespace Items;

public abstract class ItemType
{
    public abstract string Name { get; }
    public abstract string Description { get; }
}
public abstract class Consumable : ItemType
{
    public abstract void Use(Character character, Player player, List<Character> friendlies, List<Character>? hostiles);
}

public class HealthPotion : Consumable
{
    public override string Name { get; } = "Health Potion";
    public override string Description { get; } = "Restores a friendly character's HP by 10, or until full.";
    public override void Use(Character character, Player player, List<Character> friendlies, List<Character>? hostiles = null)
    {
        character = player.PickTarget(friendlies);
        int healing = Math.Clamp(10, 0, character.MaxHP - character.HP);
        character.HP = Math.Clamp(character.HP + 10, 0, character.MaxHP);
        Console.WriteLine($"{character.Name} has been healed by {healing}. They are now at {character.HP}/{character.MaxHP}");
    }
}
public class PoisonFlask : Consumable
{
    public override string Name { get; } = "Poison Flask";
    public override string Description { get; } = "Poison that deals 1 damage per round for 6 rounds.";
    public override void Use(Character character, Player player, List<Character> friendlies, List<Character>? hostiles)
    {
        character = player.PickTarget(hostiles);
        character.StatusEffects.Add((Status.Poison, 6));
        Console.WriteLine($"{character.Name} has been poisoned!");
    }
}
public class RegenPotion : Consumable
{
    public override string Name { get; } = "Regen Potion";
    public override string Description { get; } = "Potion that regenerates 2 HP over 8 rounds";
    public override void Use(Character character, Player player, List<Character> friendlies, List<Character>? hostiles)
    {
        character = player.PickTarget(friendlies);
        character.StatusEffects.Add((Status.Regen, 8));
        Console.WriteLine($"{character.Name} is regenerating!");
    }
}
public class SimulaSoup: Consumable
{
    public override string Name { get; } = "Simula's Soup";
    public override string Description { get; } = "Delicious soup in a pretty mason jar. Fully heals a party member.";
    public override void Use(Character character, Player player, List<Character> friendlies, List<Character>? hostiles)
    {
        character = player.PickTarget(friendlies);
        character.HP = character.MaxHP;
        Console.WriteLine($"{character.Name} is fully healed!");
    }
}

public abstract class Equippable : ItemType
{
    public abstract ICharacterAction Skill { get; }
}

public class None : Equippable
{
    public override string Name { get; } = "None";
    public override string Description { get; } = "Not carrying any weapons.";
    public override ICharacterAction Skill { get; } = null;
}
public class Sword : Equippable
{
    public override string Name { get; } = "Sword";
    public override string Description { get; } = "Grants character the 'SLASH' attack, which deals 2 damage.";
    public override ICharacterAction Skill { get; } = new Slash();
}
public class Dagger : Equippable
{
    public override string Name { get; } = "Dagger";
    public override string Description { get; } = "Grants character the 'STAB' attack, which deals 1 damage.";
    public override ICharacterAction Skill { get; } = new Stab();
}
public class VinBow : Equippable
{
    public override string Name { get; } = "Vin's Bow";
    public override string Description { get; } = "Grants character the 'Quick Shot' attack, which deals 3 damage at 50% accuracy.";
    public override ICharacterAction Skill { get; } = new QuickShot();
}
public class OopsieDaisy : Equippable
{
    public override string Name { get; } = "OOP-sie Daisy";
    public override string Description { get; } = "A magic flower grown near the Fountain of Objects. Glows brighter the more objects are nearby...";
    public override ICharacterAction Skill { get;} = new BigOopsie();
}
public class StoneClaw : Equippable
{ 
    public override string Name { get; } = "Stone Claw";
    public override string Description { get; } = "The tough claw of a stone Amarok. Inflicts bleed on crit.";
    public override ICharacterAction Skill { get; } = new Rend();
}

public abstract class Modifier : ItemType
{
    public abstract int Mod { get; }
    public abstract int ApplyModifier(Attack attack);
}
public class StoneArmor : Modifier
{
    public override string Name { get; } = "STONE ARMOR";
    public override string Description { get; } = "Magically-enhanced, rocky skin that reduces ALL incoming damage by 1.";
    public override int Mod { get; } = 1;
    public override int ApplyModifier(Attack attack)
    {
        return Math.Clamp(attack.TrueDMG - 1, 0, 99);
    }
}
public class ObjectSight : Modifier
{
    public override string Name { get; } = "OBJECT SIGHT";
    public override string Description { get; } = "The ability to perceive the types and objects that make up the world. Grants resistance to DECODING damage.";
    public override int Mod { get; } = 2;
    public override int ApplyModifier(Attack attack)
    {
        if (attack.Type == DamageType.Decoding) return Math.Clamp(attack.TrueDMG - Mod, 0, 99);
        else return Math.Clamp(attack.TrueDMG, 0, 99);
    }
}
public class Dodge : Modifier
{
    private Random Random = new Random();
    public override string Name { get; } = "DODGE";
    public override string Description { get; } = "Vin be nimble, Vin be quick, dodge your hit and punch your [CENSORED]";
    public override int Mod { get; } = 0;
    public override int ApplyModifier(Attack attack)
    {
        if (Random.Next(5) == 3)
        {
            Console.WriteLine("The attack was dodged!");
            return 0;
        }
        else return Math.Clamp(attack.TrueDMG, 0, 99);
    }
}

public enum Item { HealthPotion, RegenPotion, Poison, Soup }
public enum Gear { None, Sword, Dagger, VinBow, OopsieDaisy, StoneClaw }