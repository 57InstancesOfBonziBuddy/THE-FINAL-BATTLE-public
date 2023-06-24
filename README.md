Everything is done according to the challenges in the book, but some deviations were added during "Making it yours":

  Inventories are split in two between gear and items
  
  Attack Modifiers are technically a type of item, just one that can't explicitly be used or equipped/unequipped
  
  Each character has a unique Action List. This allows restricting actions for certain characters, such as Amaroks being unable to use items, since they're animals.

  I could probably replace the enum lists (inventories, Action Lists) with class instances but at this point I'm too deep down the hole.

  Attacks can crit using similar logic to Accuracy

  Characters can have multiple status effects applied to them at once, which are tracked and resolved individually using a List of tuples: (StatusEffect, duration)

  Stone Amaroks carry a piece of equipment called Stone Claw, which adds a debuff on crit and makes them much more formidable.

  Removed the Do Nothing action from all enemies' action list to make enemies more of a threat. The player can still choose to do nothing.

  Some additional items, skills and attacks.

I kept the theming appropriate to the book, as this is the final challenge and really kind of felt like the end of a journey.
Also I really, really wanted to add an item called "OOP-sie Daisy".
