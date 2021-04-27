// Fill out your copyright notice in the Description page of Project Settings.

using UnrealBuildTool;
using System.Collections.Generic;

public class ExpoPosExportTestTarget : TargetRules
{
	public ExpoPosExportTestTarget(TargetInfo Target) : base(Target)
	{
		Type = TargetType.Game;

		ExtraModuleNames.AddRange( new string[] { "ExpoPosExportTest" } );
	}
}
