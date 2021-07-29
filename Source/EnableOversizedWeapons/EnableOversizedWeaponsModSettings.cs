using Verse;

namespace EnableOversizedWeapons
{
    public class EnableOversizedWeaponsModSettings : ModSettings
    {
        public bool removeNorthDrawOffsetFromEquipment = true;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<bool>(ref removeNorthDrawOffsetFromEquipment, "removeNorthDrawOffsetFromEquipment", true, false);
        }
    }
}
