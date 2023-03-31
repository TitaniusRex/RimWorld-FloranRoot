// Assembly-CSharp, Version=1.4.8487.23678, Culture=neutral, PublicKeyToken=null
// RimWorld.IncidentWorker_ThrumboPasses
using RimWorld;
using UnityEngine;
using Verse;

public class IncidentWorker_FloranPasses : IncidentWorker
{
	protected override bool CanFireNowSub(IncidentParms parms)
	{
		Map map = (Map)parms.target;
		if (map.gameConditionManager.ConditionIsActive(GameConditionDefOf.ToxicFallout))
		{
			return false;
		}
		if (ModsConfig.BiotechActive && map.gameConditionManager.ConditionIsActive(GameConditionDefOf.NoxiousHaze))
		{
			return false;
		}
		IntVec3 cell;
		return TryFindEntryCell(map, out cell);
	}

	protected override bool TryExecuteWorker(IncidentParms parms)
	{
		Map map = (Map)parms.target;
		if (!TryFindEntryCell(map, out var cell))
		{
			return false;
		}
		PawnKindDef floran = PawnKindDefOf.Floran;
		int value = GenMath.RoundRandom(StorytellerUtility.DefaultThreatPointsNow(map) / floran.combatPower);
		int max = Rand.RangeInclusive(3, 6);
		value = Mathf.Clamp(value, 40, max);
		int num = Rand.RangeInclusive(90000, 1500000);
		IntVec3 result = IntVec3.Invalid;
		if (!RCellFinder.TryFindRandomCellOutsideColonyNearTheCenterOfTheMap(cell, map, 10f, out result))
		{
			result = IntVec3.Invalid;
		}
		Pawn pawn = null;
		for (int i = 0; i < value; i++)
		{
			IntVec3 loc = CellFinder.RandomClosewalkCellNear(cell, map, 10);
			pawn = PawnGenerator.GeneratePawn(floran);
			GenSpawn.Spawn(pawn, loc, map, Rot4.Random);
			pawn.mindState.exitMapAfterTick = Find.TickManager.TicksGame + num;
			if (result.IsValid)
			{
				pawn.mindState.forcedGotoPosition = CellFinder.RandomClosewalkCellNear(result, map, 10);
			}
		}
		SendStandardLetter("LetterLabelFloranPasses".Translate(floran.label).CapitalizeFirst(), "LetterFloranPasses".Translate(floran.label), LetterDefOf.PositiveEvent, parms, pawn);
		return true;
	}

	private bool TryFindEntryCell(Map map, out IntVec3 cell)
	{
		return RCellFinder.TryFindRandomPawnEntryCell(out cell, map, CellFinder.EdgeRoadChance_Animal + 0.2f);
	}
}
