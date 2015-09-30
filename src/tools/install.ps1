param($installPath, $toolsPath, $package, $project)

$packageId = $package.Id
$packageVersion = $package.Version
$packageFolder = "$packageId.$packageVersion"
$toolsDestPath = "$installPath\..\..\tools"
$toolsDestPath = [System.IO.Path]::GetFullPath($toolsDestPath)
if (![System.IO.Directory]::Exists($toolsDestPath)) {
  [System.IO.Directory]::CreateDirectory($toolsDestPath)
}

$targetsFile = "TransformNuGetContent.targets"
$assemblyFile = "NuGetContentGenerator.dll"

[System.IO.File]::Copy("$toolsPath\$targetsFile", "$toolsDestPath\$targetsFile", $true)
[System.IO.File]::Copy("$toolsPath\$assemblyFile", "$toolsDestPath\$assemblyFile", $true)

$buildProject = Get-MSBuildProject

$existingImports = $buildProject.Xml.Imports |
	Where-Object { $_.Project -like "*\$targetsFile" }
	
if ($existingImports) {
	$existingImports |
		ForEach-Object {
			$buildProject.Xml.RemoveChild($_) | Out-Null
		}
}
$buildProject.Xml.AddImport("`$(SolutionDir)\tools\$targetsFile") | Out-Null
$project.Save()