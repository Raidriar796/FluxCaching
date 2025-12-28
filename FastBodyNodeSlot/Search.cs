using ResoniteModLoader;
using FrooxEngine;
using FrooxEngine.CommonAvatar;
using ProtoFlux.Runtimes.Execution.Nodes.FrooxEngine.Avatar;
using Renderite.Shared;

namespace FastBodyNodeSlot;

public partial class FastBodyNodeSlot : ResoniteMod
{
    private static Slot CustomGetBodyNodeSlot(BodyNodeSlot instance, User user, BodyNode node)
    {
        CachedResults cachedResults;
        BipedRig bipedRig = null!;

        // Returns early if the dictionary does not have the BodyNodeSlot tracked yet
        if (CachedBodyNodeSlots.ContainsKey(instance))
        {
            cachedResults = CachedBodyNodeSlots[instance];
        }
        else
        {
            return null!;
        }

        // Recreation of the original GetBodyNodeSlot method's null checking
        Slot slot;

	      if (user == null)
	      {
		        slot = null!;
	      }
	      else
	      {
		        UserRoot root2 = user.Root;
		        slot = (root2 != null) ? root2.Slot : null!;
	      }

	      Slot root = slot;

	      if (root == null)
	      {
		        return null!;
	      }

	      if (node == BodyNode.NONE)
	      {
		        return null!;
	      }

        // Stores for the first time the biped rig is searched to avoid searching again if it's null
        if (!cachedResults.IsBipedRigSearched)
        {
            cachedResults.CachedBipedRig = root.GetComponentInChildren<BipedRig>();
            bipedRig = cachedResults.CachedBipedRig;
            CachedBodyNodeSlots[instance].CachedBipedRig = cachedResults.CachedBipedRig;
            CachedBodyNodeSlots[instance].IsBipedRigSearched = true;
        }
        else if (cachedResults.CachedBipedRig == null)
        {
            bipedRig = null!;
        }
        else if (cachedResults.CachedBipedRig.IsDestroyed)
        {
            cachedResults.CachedBipedRig = root.GetComponentInChildren<BipedRig>();
            bipedRig = cachedResults.CachedBipedRig;
            CachedBodyNodeSlots[instance].CachedBipedRig = cachedResults.CachedBipedRig;
        }
        else
        {
            bipedRig = cachedResults.CachedBipedRig;
        }

	      Slot bone = (bipedRig != null) ? bipedRig.TryGetBone(node) : null!;

	      if (bone != null)
	      {
		        return bone;
	      }

	      AvatarObjectSlot avatarSlot = root.FindSlotForNodeInChildren(node);

	      if (avatarSlot != null)
	      {
		        return avatarSlot.Slot;
	      }

	      return null!;
    }
}
