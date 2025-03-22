using ConvenientChests.Framework.CategorizeChests.Framework;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;

namespace ConvenientChests.Framework.StashToChests {
    public class StashToNearbyChestsModule : Module {
        public StackLogic.AcceptingFunction AcceptingFunction { get; private set; }

        public StashToNearbyChestsModule(ModEntry modEntry) : base(modEntry) {}

        public override void Activate() {
            IsActive = true;

            // Acceptor
            AcceptingFunction = CreateAcceptingFunction();

            // Events
            Events.Input.ButtonPressed += OnButtonPressed;
        }

        private StackLogic.AcceptingFunction CreateAcceptingFunction() {
            if (ModConfig.CategorizeChests && ModConfig.StashToExistingStacks)
                return (chest, item) => ModEntry.CategorizeChests.ChestAcceptsItem(chest, item) || chest.ContainsItem(item);

            if (ModConfig.CategorizeChests)
                return (chest, item) => ModEntry.CategorizeChests.ChestAcceptsItem(chest, item);

            if (ModConfig.StashToExistingStacks)
                return (chest, item) => chest.ContainsItem(item);

            return (_, _) => false;
        }

        public override void Deactivate() {
            IsActive = false;

            // Events
            this.Events.Input.ButtonPressed -= OnButtonPressed;
        }

        private void TryStashNearby() {
            if (Game1.player.currentLocation == null)
                return;

            if (Game1.activeClickableMenu is ItemGrabMenu { context: Chest c }) {
                ModEntry.Log("Stash to current chest");
                StackLogic.StashToChest(c, AcceptingFunction);
            }

            else {
                ModEntry.Log("Stash to nearby chests");
                StackLogic.StashToNearbyChests(ModConfig.StashRadius, AcceptingFunction);
            }
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e) {
            if (e.Button == ModConfig.StashKey || e.Button == ModConfig.StashButton)
                TryStashNearby();
        }
    }
}