param($installPath, $toolsPath, $package, $project)

$packageId = $package.Id
$packageVersion = $package.Version
$packageFolder = "$packageId.$packageVersion"
$toolsDestPath = "$installPath\..\..\tools"
$toolsDestPath = [System.IO.Path]::GetFullPath($toolsDestPath)

$targetsFile = "TransformNuGetContent.targets"
$assemblyFile = "NuGetContentGenerator.dll"

[System.IO.File]::Delete("$toolsDestPath\$targetsFile")
[System.IO.File]::Delete("$toolsDestPath\$assemblyFile")

$buildProject = Get-MSBuildProject

$existingImports = $buildProject.Xml.Imports |
	Where-Object { $_.Project -like "*\$targetsFile" }
	
if ($existingImports) {
	$existingImports |
		ForEach-Object {
			$buildProject.Xml.RemoveChild($_) | Out-Null
		}
	$project.Save()
}