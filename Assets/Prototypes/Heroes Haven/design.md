# Design

## Travelers
- Can barter for help
  - Give a weapon, maybe they'll help you hunt that night
  - Or maybe they clear out a dangerous / locked area and then you can expand in that direction
  - Or they spend the day as a temporary resisdent with better stats


- People can decide to leave (even after staying)
  - Not enough food
  - Not enough shelther 
  - Too crowded
  - Too noisy
  - Too many drunks
  - Too dangerous

- You can also exile people for any reason


Examples
Hunting
- Hunting a rabbit and skinning it and bringing its meat may be more sanity and socially acceptable, but might lead to predators following you back to town
- Capturing it live and killing it in town is safer, but might gross out neighbors

Town Thief
-Someone might take traveler requests and trade, but has low honesty and ends up always taking things owned by others (either town residents or other travelers) -- so you banish them. If they return to town,it's up to the player to remember -- or chat them up to get their history


## Needs
- There's no real failure state, but opportunity costs.
- If you don't make gear for the traveler in time, they won't help you out or settle
- Or if you don't produce enough food, your settlers will ignore orders to satisfy their survival needs first, and you miss out on what you want to craft for the day

So hunger reaching a critical level doesn't need to produce a failure screen—or even necessarily a punishment in the conventional sense. It can simply reorder the simulation's priorities underneath you.

You tell Mara to finish the guest house. She gets hungry enough and effectively says, nope. She goes looking for food. Maybe she takes the berries you were saving for a traveler. Maybe she spends three hours fishing. Maybe your construction deadline gets missed.

Nothing broke. The settlement is still there.

That could even become one of the most important ways the characters express themselves. Personality could affect when they override you. A dutiful person works dangerously long. A self-interested person bails early. A communal person stops their task because someone else is starving.

Now neglect isn't "you lose."

Neglect is you surrender some control of the day's story.

## Upcoming
Goals and Needs
The goal is what the person wants to do, possibly a native Task. The need is what they think will help / fills in the gaps of what they already have

Skill and Knowledge
What is known and what can be done (or to what quality can it be done)

## Efficiency

Claimed / In Use -- to prevent NPCs from trying to occupy the same activity, space, resource, etc


Think through CampFire provides heat, provides light with CampFire Burning. Those two capabilities are active when the state on the instance is burning. Maybe ActiveConditions

An item's utility will become the Capability Value * Normalized Skill (0.5 - 1.5) + Grade Modifier (0.8 - 1.2) * Condition Modifier.

And it measures use. So an Axe cutting against a tree is the rate at which is cuts, where the tree defines its extraction time.

effectiveCapability =
    baseCapability
    * gradeModifier
    * conditionModifier
    * skillModifier;

extractionTime =
    extraction.Work / effectiveCapability;

F = .8, C = 1.0, A = 1.2, S = 1.3

## Resource Scarcity
- Resources will exist infinitely, but their extraction time and limit of one person per item makes the time commitment create opportunity costs