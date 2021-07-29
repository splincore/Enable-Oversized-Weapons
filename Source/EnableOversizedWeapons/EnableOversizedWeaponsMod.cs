using UnityEngine;
using Verse;

namespace EnableOversizedWeapons
{
    public class EnableOversizedWeaponsMod : Mod
    {
        EnableOversizedWeaponsModSettings enableOversizedWeaponsModSettings;
        public EnableOversizedWeaponsMod(ModContentPack content) : base(content)
        {
            enableOversizedWeaponsModSettings = GetSettings<EnableOversizedWeaponsModSettings>();
        }
        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);
            listingStandard.CheckboxLabeled("Remove drawOffsetNorth from equipment on the ground (NEEDS GAME RESTART!!!)", ref enableOversizedWeaponsModSettings.removeNorthDrawOffsetFromEquipment, "Remove drawOffsetNorth from equipment on the ground");
            listingStandard.End();
        }

        public override string SettingsCategory()
        {
            return "Enable Oversized Weapons";
        }
    }
}
