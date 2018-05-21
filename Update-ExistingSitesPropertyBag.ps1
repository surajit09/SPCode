function GetSPQuery($storeID)
{
	$spQuery = New-Object Microsoft.SharePoint.SPQuery
    $camlQuery ="<Where><Eq><FieldRef Name='CRStoreID' /><Value Type='String'>$storeID</Value></Eq></Where>"
    $spQuery.Query = $camlQuery
    $spQuery.RowLimit = 100
    WriteToLog "    Query: $($spQuery.Query)" "info"
	return $spQuery
}

function AddPropertyBagValue($pbWeb, $pbKey, $pbValue)
{
    $pbWeb.Properties.Remove($pbKey)
    $pbWeb.AllProperties.Remove($pbKey)
    $pbWeb.Update()
	$pbWeb.Properties.Update()

    $pbWeb.Properties.Add($pbKey, $pbValue)
    $pbWeb.AllProperties.Add($pbKey, $pbValue)
	
    $pbWeb.Update()
	$pbWeb.Properties.Update()
}

function WriteToLog($message, $type)
{
    
    if($type -eq "warning")
    {
        $fontColour = "Yellow"
    } elseif($type -eq "error")
    {
        $fontColour = "Red"
    } elseif($type -eq "info")
    {
        $fontColour = "Green"
    } else
    {
        $fontColour = "White"
    }

    Write-Host $message -ForegroundColor $fontColour
    Write-Output $message | Out-File $Logfilepath -Append
}

function GetStoreNumber($storeUrl)
{
    $splitSN = $storeUrl.Split("/")
	return $splitSN[3]
}

# Set the current script location to a variable
$ScriptRootLoc = Split-Path -Parent $MyInvocation.MyCommand.Path

# Initiate a location for the log file
$Logfilepath = $ScriptRootLoc + "\deployment.log"
Write-Host "Full deployment log will be output to the following location:" -foregroundcolor Cyan
Write-Host $Logfilepath -foregroundcolor Gray
Write-Host ""

# Invoke the SharePoint PowerShell command extension script
. "C:\Program Files\Common Files\Microsoft Shared\Web Server Extensions\15\CONFIG\POWERSHELL\Registration\sharepoint.ps1"

# Script Variables
$siteUrl = "https://thethread.carpetright.co.uk/"
$keyDiv = "StoreDivision"
$keyType = "SiteType"

$site = Get-SPSite $siteUrl

try
{
    $web = $site.RootWeb
    $subsites = $web.Webs

    foreach ($subsite in $subsites)
    {
		
        WriteToLog "Processing Site: $($subSite.Title)"
        
        # Get Store Number from URL
        $storeNumber = GetStoreNumber $subSite.Url
        WriteToLog "    Store Number: $storeNumber" "info"

		# Check store number is a number and not a string - avoids other non-store sites.
        if($storeNumber -match "^[0-9]+$")
        {
           # Get SPList item from stores list (if it does not exist then its not a valid store site)
           $listStores = $site.RootWeb.GetList("/Lists/Stores")
           
           $query = GetSPQuery $storeNumber
           $spListItems = $listStores.GetItems($query)

		   # If we get one single item then carry on - else error
           if($spListItems.Count -eq 1)
           {

                $storesListItem = $spListItems[0]

                # Set correct property bag values on the site
                $div = $storesListItem["CRStoreDivision"]

				AddPropertyBagValue $subsite $keyDiv $div

				AddPropertyBagValue $subsite $keyType "Store Site"

                # Get a new SPWeb object to validate the changes have persisted.
                $webToValidate = Get-SPWeb $subsite.Url

                $webToValidate.IndexedPropertyKeys.Add($keyDiv)
                $webToValidate.IndexedPropertyKeys.Add($keyType)
                $webToValidate.Update()

                Write-Output "PROPERTIES" | Out-File $Logfilepath -Append
                Write-Output $webToValidate.Properties | Out-File $Logfilepath -Append
                
                Write-Output "ALL PROPERTIES" | Out-File $Logfilepath -Append
                Write-Output $webToValidate.AllProperties | Out-File $Logfilepath -Append

                Write-Output "INDEXED PROPERTY KEYS" | Out-File $Logfilepath -Append
                Write-Output $webToValidate.IndexedPropertyKeys | Out-File $Logfilepath -Append

                $webToValidate.Dispose()

                WriteToLog "    Property bag updated succesfully" "info"

           } elseif($spListItems.Count -eq 0)
           {
                 WriteToLog "    Error: No entries found in Stores List for $storeNumber" "error"
           } elseif($spListItems.Count -gt 1)
           {
                WriteToLog "    Error: Multiple entries found in Stores List for $storeNumber" "error"
           }
        } else
        {
            WriteToLog "    Not a valid store number.  Site ignored." "warning"
        }

        WriteToLog "Site processed succesfully"
        WriteToLog ""

	}
} catch [Exception] 
{
	$errString = $_.Exception.ToString()
	Write-Output "ERROR: $errString" 
} finally
{
	$site.Dispose()
}