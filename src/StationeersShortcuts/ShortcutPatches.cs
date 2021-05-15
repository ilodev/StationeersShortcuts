using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Inventory;
using Assets.Scripts.Objects;
using Assets.Scripts.Objects.Entities;
using HarmonyLib;
using UnityEngine;

namespace StationeersShortcuts
{
    class ShortcutPatches
    {


    }

    /* Custom shortcut key binding injection is done after KeyManager.SetupKeyBindings() method is 
     * called; this way, we can get our custom new bindings saved/load by the game during the 
     * controls initialisation without needing any extra file access.
     */ 
    [HarmonyPatch(typeof(KeyManager), "SetupKeyBindings")]
    class ShortcutInjectBindingGroup
    {
        static void Postfix()
        {
            // We need to add a custom control group for the keys to be attached to, and create 
            // the Lookout reference.
            UnityEngine.Debug.Log("Adding custom Shortcuts group");
            ControlsGroup controlsGroup1 = new ControlsGroup("ShortCuts");
            KeyManager.AddGroupLookup(controlsGroup1);

            // We will add the custom keys with default values to the KeyItem list using our new
            // created control group, however this method -due to accesibility of the class method-
            // will change the current ControlGroup name.
            ShortcutInjectBindingGroup.AddKey("Backpack 2", KeyCode.Keypad7, controlsGroup1, false);
            ShortcutInjectBindingGroup.AddKey("Backpack 3", KeyCode.Keypad8, controlsGroup1, false);
            ShortcutInjectBindingGroup.AddKey("Backpack 4", KeyCode.Keypad9, controlsGroup1, false);
            ShortcutInjectBindingGroup.AddKey("Backpack 5", KeyCode.Keypad4, controlsGroup1, false);
            ShortcutInjectBindingGroup.AddKey("Toolbelt 1", KeyCode.Keypad5, controlsGroup1, false);
            ShortcutInjectBindingGroup.AddKey("Toolbelt 2", KeyCode.Keypad6, controlsGroup1, false);
            ShortcutInjectBindingGroup.AddKey("Toolbelt 3", KeyCode.None, controlsGroup1, false);
            ShortcutInjectBindingGroup.AddKey("Toolbelt 4", KeyCode.None, controlsGroup1, false);
            ShortcutInjectBindingGroup.AddKey("Grinder", KeyCode.None, controlsGroup1, false);
            ShortcutInjectBindingGroup.AddKey("Welder", KeyCode.None, controlsGroup1, false);
            ShortcutInjectBindingGroup.AddKey("Hand Drill", KeyCode.None, controlsGroup1, false);
            ShortcutInjectBindingGroup.AddKey("Wrench", KeyCode.None, controlsGroup1, false);
            ShortcutInjectBindingGroup.AddKey("Cable Cutters", KeyCode.None, controlsGroup1, false);
            ShortcutInjectBindingGroup.AddKey("Screwdriver", KeyCode.None, controlsGroup1, false);
            ShortcutInjectBindingGroup.AddKey("Crowbar", KeyCode.None, controlsGroup1, false);

            // TODO ADD other tools
            //ShortcutInjectBindingGroup.AddKey("Weapon", KeyCode.None, controlsGroup1, false);
            //ShortcutInjectBindingGroup.AddKey("Water", KeyCode.None, controlsGroup1, false);


            // We need to restore the name of the control group back to its correct string
            controlsGroup1.Name = "ShortCuts";
        }

        /* Custom method to add keys to a ControlGroup. We 'hijack' the control group lookup function 
         * that will also save the name of they key in the list for us
         */ 
        private static void AddKey(string assignmentName,
            KeyCode keyCode,
            ControlsGroup controlsGroup,
            bool hidden = false
        )
        {
            // This is just because of the accessibility to change the assigned name, we use 
            // the control group for that.
            controlsGroup.Name = assignmentName;
            KeyManager.AddGroupLookup(controlsGroup);

            // Now Create the key, add its looup string name, and save it in the allkeys list, to ensure
            // is being saved/load by the game config initialisation function.
            KeyItem keyItem = new KeyItem(assignmentName, keyCode, hidden);
            KeyManager.KeyItemLookup.Add(assignmentName, keyItem);
            KeyManager.AllKeys.Add(keyItem);
        }
    }


    /* Custom Postfix to add the shortcut text to the slot display if available.
     */
    [HarmonyPatch(typeof(Slot), "RefreshSlotDisplay", new Type[] { typeof(bool) })]
    class UpdateSlotShortCutText
    {
        static void Postfix(Slot __instance)
        {
            if (__instance == null) return;

            // Find parent element to check whether it is a controlled slot or not.
            Human Character = InventoryManager.ParentHuman;
            if (Character == null) return;

            if (Character.BackpackSlot.Occupant != null && Character.BackpackSlot.Occupant == __instance.Parent)
            {
                UnityEngine.Debug.Log("Refresh backpack slot: " + __instance.DisplayName.ToString());
                // Find our current index in the backpack
                if (__instance == __instance.Parent.Slots[1])
                {
                    TextForSlotId(__instance, "Backpack 2", 1);
                }
                if (__instance == __instance.Parent.Slots[2])
                {
                    TextForSlotId(__instance, "Backpack 3", 2);
                }
                if (__instance == __instance.Parent.Slots[3])
                {
                    TextForSlotId(__instance, "Backpack 4", 3);
                }
                if (__instance == __instance.Parent.Slots[4])
                {
                    TextForSlotId(__instance, "Backpack 5", 4);
                }

            }

            if (Character.ToolbeltSlot.Occupant != null && Character.ToolbeltSlot.Occupant == __instance.Parent)
            {
                UnityEngine.Debug.Log("Refresh toolbelt slot: " + __instance.DisplayName.ToString());
                // Find our current index in the toolbelt
                if (__instance == __instance.Parent.Slots[0])
                {
                    TextForSlotId(__instance, "Toolbelt 1", 0);
                }
                if (__instance == __instance.Parent.Slots[1])
                {
                    TextForSlotId(__instance, "Toolbelt 2", 1);
                }
                if (__instance == __instance.Parent.Slots[2])
                {
                    TextForSlotId(__instance, "Toolbelt 3", 2);
                }
                if (__instance == __instance.Parent.Slots[3])
                {
                    TextForSlotId(__instance, "Toolbelt 4", 3);
                }
            }

        }

        static void TextForSlotId(Slot __instance, string text, int index)
        {
            UnityEngine.Debug.Log("Found Slot: " + __instance.DisplayName.ToString());
            KeyCode test = KeyManager.GetKey(text);
            if (test != KeyCode.None && __instance.Display != null)
                __instance.Display.SlotDisplayButton.Text.text = test.ToString();
        }

    }

}