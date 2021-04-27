// Fill out your copyright notice in the Description page of Project Settings.

#include "Misc/FileHelper.h"
#include "Misc/Paths.h"
#include "SaveToFile.h"
bool USaveToFile::FileSaveString(FString SaveTextB, FString FileNameB)
{
	return FFileHelper::SaveStringToFile(SaveTextB, *(FPaths::GameDir() + FileNameB));
}
