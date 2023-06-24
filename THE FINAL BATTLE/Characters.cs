using CharacterActions;
using Items;

namespace Characters;

public abstract class Character
{
    public abstract string Name { get; }
    public abstract int MaxHP { get; }
    public abstract int HP { get; set; }
    public abstract Modifier? DefenseMod { get; }
    public abstract Attack StandardAttack { get; }
    public abstract Faction Faction { get; }
    public abstract List<Actions> ActionList { get; set; }
    public abstract Gear EquippedGear { get; set; }
    public abstract List<(Status, int)> StatusEffects { get; set; }

    public abstract event Action<Character>? hasDied;

    public abstract void TakeDamage(int damage);
}

public class TrueProgrammer : Character
{
    public override string Name { get; }
    public override int MaxHP { get; } = 25;
    public override int HP { get; set; }
    public override Modifier? DefenseMod { get; } = new ObjectSight();
    public override Attack StandardAttack { get; } = new Punch();
    public override Faction Faction { get; } = Faction.Good;
    public override List<Actions> ActionList { get; set; } = new List<Actions> { Actions.StandardAtk, Actions.Special, Actions.UseItem, Actions.Equip, Actions.Nothing};
    public override Gear EquippedGear { get; set; } = Gear.Sword;
    public override List<(Status, int)> StatusEffects { get; set; } = new List<(Status, int)>();

    public override event Action<Character>? hasDied;

    public TrueProgrammer()
    {
        Name = GetName();
        HP = MaxHP;
    }


    public override void TakeDamage(int damage)
    {
        HP = Math.Clamp(HP - damage, 0, MaxHP);
        if (HP == 0) hasDied(this);
    }

    public string GetName()
    {
        Console.WriteLine("Before we get started with this final battle: What is your name, brave hero?");
        string? name = Console.ReadLine();

        while (name == null)
        {
            Console.WriteLine("I didn't catch that, sorry. What is your name?");
            name = Console.ReadLine();
        }
        return name;
    }
}

public class VinFletcher : Character
{
    public override string Name { get; } = "VIN FLETCHER";
    public override int MaxHP { get; } = 15;
    public override int HP { get; set; }
    public override Modifier? DefenseMod { get; } = new Dodge();
    public override Attack StandardAttack { get; } = new Punch();
    public override Faction Faction { get; } = Faction.Good;
    public override List<Actions> ActionList { get; set; } = new List<Actions> { Actions.StandardAtk, Actions.Special, Actions.UseItem, Actions.Equip, Actions.Nothing};
    public override Gear EquippedGear { get; set; } = Gear.VinBow;
    public override List<(Status, int)> StatusEffects { get; set; } = new List<(Status, int)>();

    public override event Action<Character>? hasDied;

    public VinFletcher()
    {
        HP = MaxHP;
    }

    public override void TakeDamage(int damage)
    {
        HP = Math.Clamp(HP - damage, 0, MaxHP);
        if (HP == 0) hasDied(this);
    }

}
public class Skeleton : Character
{
    public override string Name { get;}
    public override int MaxHP { get;} = 7;
    public override int HP { get; set; }
    public override Modifier? DefenseMod { get; } = null;
    public override Attack StandardAttack { get; } = new BoneCrunch();
    public override Faction Faction { get; } = Faction.Evil;
    public override List<Actions> ActionList { get; set; } = new List<Actions> { Actions.StandardAtk, Actions.UseItem, Actions.Equip};
    public override Gear EquippedGear { get; set; } = Gear.None;
    public override List<(Status, int)> StatusEffects { get; set; } = new List<(Status, int)>();

    public override event Action<Character>? hasDied;

    public Skeleton(string name, Gear equipment)
    {
        Name = name;
        HP = MaxHP;
        EquippedGear = equipment;
    }

    public override void TakeDamage(int damage)
    {
        HP = Math.Clamp(HP - damage, 0, MaxHP);
        if (HP == 0) hasDied(this);
    }
}

public class StoneAmarok : Character
{
    public override string Name { get; }
    public override int MaxHP { get; } = 6;
    public override int HP { get; set; }
    public override Modifier DefenseMod { get; } = new StoneArmor();
    public override Attack StandardAttack { get; } = new Bite();
    public override Faction Faction { get; } = Faction.Evil;
    public override List<Actions> ActionList { get; set; } = new List<Actions> { Actions.StandardAtk, Actions.Special};
    public override Gear EquippedGear { get; set; } = Gear.StoneClaw;
    public override List<(Status, int)> StatusEffects { get; set; } = new List<(Status, int)>();

    public override event Action<Character>? hasDied;

    public StoneAmarok(string name)
    {
        Name = name;
        HP = MaxHP;
    }

    public override void TakeDamage(int damage)
    {
        HP = Math.Clamp(HP - damage, 0, MaxHP);
        if (HP == 0) hasDied(this);
    }
}
public class UncodedOne : Character
{
    public override string Name { get; } = "THE UNCODED ONE";
    public override int MaxHP { get; } = 45;
    public override int HP { get; set; }
    public override Modifier? DefenseMod { get; } = null;
    public override Attack StandardAttack { get; } = new Unravel();
    public override Faction Faction { get; } = Faction.Evil;
    public override List<Actions> ActionList { get; set; } = new List<Actions> { Actions.StandardAtk, Actions.UseItem};
    public override Gear EquippedGear { get; set; } = Gear.None;
    public override List<(Status, int)> StatusEffects { get; set; } = new List<(Status, int)>();

    public override event Action<Character>? hasDied;

    public UncodedOne()
    {
        HP = MaxHP;
    }
    public override void TakeDamage(int damage)
    {
        HP = Math.Clamp(HP - damage, 0, MaxHP);
        if (HP == 0) hasDied(this);
    }
}

public enum Faction { Good, Evil }
public enum Status { None, Poison, Regen, Bleed}