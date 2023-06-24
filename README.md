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

Some things I noticed that probably aren't ideal but also don't really want to/can fix at the moment:
- There is currently no way to back out of an action once it has been selected.
- My input verification is bare-bones. The game will make sure you don't input a number that leads nowhere, but crashes when exposed to non-number inputs.
- My `CharacterTurn()` method is chunky and could do with some more abstraction. I could probably change it to put the action selection and the inventory checks into their own respective methods to make it a bit cleaner. This probably applies to several other methods throughout the project as well.
   - On the point of actions and inventory, there's probably a really simple way to do all of those lists as class instances instead of enums that ***then*** get transformed into class instances anyway, but by the time I realized that that may be a better solution it would've required a deep rewrite of the core logic.
- While I strived to keep main() as clean as possible, it still contains two unsorted methods. It just kind of made more sense to me that way, as they're the methods that are core to setting up and running the game.
- I wanted to get a little fancier with the status effects and items, but the way I set the lists and checks up made it more difficult than I felt I could pull off. I wanted to add stuns, power ups and defense buffs, but the attack logic and all that was so spread apart, implementing it would've taken several more hours or days at the pace I've been going. A good example of that is the Stone Claw's `Rend` attack, the effect of which I had to awkwardly squeeze into a method belonging to a whole other class.
- I *tried* to keep most of the display stuff contained to a `Display` class, but couldn't really get around sprinkling some individual lines here and there in with the logic. I'd have like a dozen single-line methods that only get used once otherwise, which would be harder to read than the plain `WriteLine`s
