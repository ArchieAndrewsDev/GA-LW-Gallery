// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Kismet/BlueprintFunctionLibrary.h"
#include "SaveToFile.generated.h"

/**
 * 
 */
UCLASS()
class EXPOPOSEXPORTTEST_API USaveToFile : public UBlueprintFunctionLibrary
{
	GENERATED_BODY()
		UFUNCTION(BlueprintCallable, Category = "save")
		static bool FileSaveString(FString SaveTextB, FString FileNameB);
};
