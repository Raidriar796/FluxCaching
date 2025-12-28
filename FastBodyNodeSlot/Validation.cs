using FrooxEngine;
using FrooxEngine.CommonAvatar;
using ProtoFlux.Runtimes.Execution.Nodes.FrooxEngine.Avatar;
using Renderite.Shared;
using ResoniteModLoader;

namespace FastBodyNodeSlot;

public partial class FastBodyNodeSlot : ResoniteMod
{
    // Stores data about the node to compare for changes across updates
    private static Slot CheckForChanges(BodyNodeSlot instance, User user, BodyNode node)
    {
        bool shouldUpdate = false;
        CachedResults cachedResults;

        // Probably overkill null checks to exit early incase any of these are true
        if (user == null || user.IsDestroyed ||
            user.Root == null || user.Root.IsDestroyed ||
            user.Root.Slot == null || user.Root.Slot.IsDestroyed)
        {
            return null!;
        }

        // Creates a new CachedResults instance and adds it to the dictionary with the instance as the key
        if (!CachedBodyNodeSlots.ContainsKey(instance))
        {
            cachedResults = new(instance, user, node);
            CachedBodyNodeSlots.Add(instance, cachedResults);
        }
        // If the key already exists, simply reuse it
        else
        {
            cachedResults = CachedBodyNodeSlots[instance];
        }

        Slot slot = cachedResults.CachedSlot!;

        // Clears the cached slot if the slot is null or was destroyed
        if (cachedResults.CachedSlot != null && cachedResults.CachedSlot.IsDestroyed)
        {
            slot = null!;
            shouldUpdate = true;
        }

        // if the cached slot is assigned but the cached parent isn't, assign the parent
        if (cachedResults.CachedSlot != null && cachedResults.CachedParent == null)
        {
            CachedBodyNodeSlots[instance].CachedParent = cachedResults.CachedSlot.Parent;
            shouldUpdate = true;
        }

        // Clears the cached parent slot if the parent is destroyed
        if (cachedResults.CachedParent != null && cachedResults.CachedParent.IsDestroyed)
        {
            CachedBodyNodeSlots[instance].CachedParent = null!;
            shouldUpdate = true;
        }

        // Reassigns the parent if the parent of the cached slot changes
        if (cachedResults.CachedSlot != null && cachedResults.CachedParent != cachedResults.CachedSlot.Parent)
        {
            CachedBodyNodeSlots[instance].CachedParent = cachedResults.CachedSlot.Parent;
            shouldUpdate = true;
        }
        
        // Reassigns the user if the cached user doesn't match
        if (cachedResults.CachedUser != user)
        {
            CachedBodyNodeSlots[instance].CachedUser = user;
            shouldUpdate = true;
        }

        // Reassigns the BodyNode if the cached BodyNode doesn't match
        if (cachedResults.CachedNode != node)
        {
            CachedBodyNodeSlots[instance].CachedNode = node;
            shouldUpdate = true;
        }

        // Assigns the user's avatar object slot if it's not been assigned already
        if (cachedResults.CachedAvatarObjectSlot == null)
        {
            CachedBodyNodeSlots[instance].CachedAvatarObjectSlot = user.Root.Slot.GetComponent<AvatarObjectSlot>();
            shouldUpdate = true;
        }
        // Assigns the user's avatar object slot if the previous was destroyed
        else if (cachedResults.CachedAvatarObjectSlot.IsDestroyed)
        {
            CachedBodyNodeSlots[instance].CachedAvatarObjectSlot = user.Root.Slot.GetComponent<AvatarObjectSlot>();
            shouldUpdate = true;
        }
        // Reassigns the cached avatar if the cached avatar doesn't match the equipped avatar root on the user
        else if (cachedResults.CachedEquippedAvatar != cachedResults.CachedAvatarObjectSlot.Equipped.Value)
        {
            CachedBodyNodeSlots[instance].CachedEquippedAvatar = cachedResults.CachedAvatarObjectSlot.Equipped.Value;
            shouldUpdate = true;
        }

        // If at any point a cache invalidation or other update occured, run the usual logic to fetch the body node slot
        if (shouldUpdate == true)
        {
            slot = CustomGetBodyNodeSlot(instance, user, node);
            CachedBodyNodeSlots[instance].CachedSlot = slot;
        }

        return slot!;
    }
}
