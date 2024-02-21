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
$fileTrackInfo = $FolderBase + '\track.info';

$items = [System.Collections.ArrayList]::new()

$currentDate = Get-Date -Format yyyy.MM.dd.HH:mm:ss.fff 

$items.add("***************Information****************") 
$items.Add("Computer name:"+$env:computername )
$items.Add("Usernamme:"+$env:UserName )
$items.Add("UserDomain:" + $env:UserDomain )
$items.Add("Date:" + $currentDate )
$items.Add("IP:" + (Invoke-WebRequest ifconfig.me/ip).Content.Trim() )

$items.add("******************************************") 


if(Test-Path $zipFilePath)
{
    (Get-Item $zipFilePath).IsReadOnly = $false;
    Remove-Item -Path $zipFilePath ;
}


$zip = [System.IO.Compression.ZipFile]::Open($zipFilePath, 'update')
$compressionLevel = [System.IO.Compression.CompressionLevel]::Fastest
$files = Get-ChildItem -Path $FolderBase -include *.* -Recurse;
foreach( $file in $files)
{
    if($file.FullName.ToLower() -notlike "*\bin*" -and $file.FullName.ToLower() -notlike "*\packages*" -and $file.FullName.ToLower() -notlike "*\obj*" -and $file.FullName.ToLower() -notlike "*$nameFile*" )
    {
        $itemName  = $file.FullName.replace($FolderBase,'');
        $items.Add($itemName); 
        [System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($zip, $file.FullName, $itemName, $compressionLevel);
    }
    
}

if(Test-Path $fileTrackInfo)
{
    (Get-Item $fileTrackInfo).IsReadOnly = $false
}

New-Item -Path $FolderBase -Name "track.info" -ItemType "file" -Value ($items -join "`r`n") -Force

$zip.Dispose()
 
