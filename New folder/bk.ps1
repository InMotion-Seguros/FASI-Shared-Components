Write-Host "Numb Args>" $args.Length;
$index = 0;
$FolderBase = "";
$NameControl = "";

foreach($arg in $args.Split("@@")) {
    Write-Host $arg
	if($index -eq 0) {
        $FolderBase = $arg;
	}else {
       $NameControl = $arg;
	}
    $index = $index + 1;
}

$NameControl = $NameControl +  ".Source";

Add-Type -Assembly 'System.IO.Compression.FileSystem'
$nameFile = $NameControl + '.zip'.ToLower();
$zipFilePath = $FolderBase + '\'+ $nameFile;
Remove-Item -Path $zipFilePath
$zip = [System.IO.Compression.ZipFile]::Open($zipFilePath, 'update')
$compressionLevel = [System.IO.Compression.CompressionLevel]::Fastest
$files = Get-ChildItem -Path $FolderBase -include *.* -Recurse;
foreach( $file in $files)
{
    if($file.FullName.ToLower() -notlike "*\bin*" -and $file.FullName.ToLower() -notlike "*\packages*" -and $file.FullName.ToLower() -notlike "*\obj*" -and $file.FullName.ToLower() -notlike "*$nameFile*" )
    {
        Write-Host $file.FullName.replace($FolderBase + '\','');
        [System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($zip, $file.FullName, $file.FullName.replace($FolderBase ,''), $compressionLevel);
    }
    
}
$zip.Dispose()
 