function UpdatePage($siteUrl, $SPWeb, $pageUrl, $webPartTitle)
{
    # Get reference to the home page  
    $page = $SPWeb.GetFile("Pages/$pageUrl")

    # Checkout the page  
    $page.CheckOut()

    # Get reference to the webpartmanager class  
    $webpartmanager = $SPWeb.GetLimitedWebPartManager("Pages/"+$pageUrl,   
    [System.Web.UI.WebControls.WebParts.PersonalizationScope]::Shared)

    # Iterate through webparts in webpartmanager class  
    for($i=0;$i -lt $webpartmanager.WebParts.Count;$i++)  
    {     
        # Check for the name of required web part   
        if($webpartmanager.WebParts[$i].title -eq $webPartTitle)   
        {  
            # Get reference to the web part  
            $wp=$webpartmanager.WebParts[$i];  
       
            if($wp.title -eq "Quick Links")
            {
                # Set the chrome property  
                $wp.DataProviderJSON = '{"QueryGroupName":"3cc6f4fb-5b15-42d8-b3b4-d3eff81dcf98","QueryPropertiesTemplateUrl":"sitesearch://webroot","IgnoreQueryPropertiesTemplateUrl":false,"SourceID":"8413cd39-2156-4e00-b54d-11efd9abdb89","SourceName":"Local SharePoint Results","SourceLevel":"Ssa","CollapseSpecification":"","QueryTemplate":"Path:' + $siteUrl + '/Lists/GlobalQuickLinks/DispForm.aspx?*","FallbackSort":[{"p":"AnnouncementOrder","d":0},{"p":"LinkURL","d":0}],"FallbackSortJson":"[{\"p\":\"AnnouncementOrder\",\"d\":0},{\"p\":\"LinkURL\",\"d\":0}]","RankRules":null,"RankRulesJson":"null","AsynchronousResultRetrieval":false,"SendContentBeforeQuery":true,"BatchClientQuery":true,"FallbackLanguage":-1,"FallbackRankingModelID":"","EnableStemming":true,"EnablePhonetic":false,"EnableNicknames":false,"EnableInterleaving":false,"EnableQueryRules":true,"EnableOrderingHitHighlightedProperty":false,"HitHighlightedMultivaluePropertyLimit":-1,"IgnoreContextualScope":true,"ScopeResultsToCurrentSite":false,"TrimDuplicates":false,"Properties":{"TryCache":true,"Scope":"{Site.URL}","UpdateLinksForCatalogItems":true,"EnableStacking":true,"ListId":"5cd01485-1473-49ba-b866-32132177ddd1","ListItemId":1},"PropertiesJson":"{\"TryCache\":true,\"Scope\":\"{Site.URL}\",\"UpdateLinksForCatalogItems\":true,\"EnableStacking\":true,\"ListId\":\"5cd01485-1473-49ba-b866-32132177ddd1\",\"ListItemId\":1}","ClientType":"ContentSearchRegular","UpdateAjaxNavigate":true,"SummaryLength":180,"DesiredSnippetLength":90,"PersonalizedQuery":false,"FallbackRefinementFilters":null,"IgnoreStaleServerQuery":false,"RenderTemplateId":"DefaultDataProvider","AlternateErrorMessage":null,"Title":""}';  
   
            }
            elseif ($wp.title -eq "All Quick Links")
            {
                # Set the chrome property  
                $wp.DataProviderJSON = '{"QueryGroupName":"Default","QueryPropertiesTemplateUrl":"sitesearch://webroot","IgnoreQueryPropertiesTemplateUrl":false,"SourceID":"8413cd39-2156-4e00-b54d-11efd9abdb89","SourceName":"Local SharePoint Results","SourceLevel":"Ssa","CollapseSpecification":"","QueryTemplate":"Path:' + $siteUrl + '/Lists/GlobalQuickLinks/DispForm.aspx?*","FallbackSort":[{"d":0,"p":"AnnouncementOrder"},{"d":0,"p":"LinkURL"}],"FallbackSortJson":"[{\"d\":0,\"p\":\"AnnouncementOrder\"},{\"d\":0,\"p\":\"LinkURL\"}]","RankRules":null,"RankRulesJson":"null","AsynchronousResultRetrieval":false,"SendContentBeforeQuery":true,"BatchClientQuery":true,"FallbackLanguage":-1,"FallbackRankingModelID":"","EnableStemming":true,"EnablePhonetic":false,"EnableNicknames":false,"EnableInterleaving":false,"EnableQueryRules":true,"EnableOrderingHitHighlightedProperty":false,"HitHighlightedMultivaluePropertyLimit":-1,"IgnoreContextualScope":true,"ScopeResultsToCurrentSite":false,"TrimDuplicates":false,"Properties":{"TryCache":true,"Scope":"{Site.URL}","UpdateLinksForCatalogItems":true,"EnableStacking":true,"ListId":"5cd01485-1473-49ba-b866-32132177ddd1","ListItemId":4},"PropertiesJson":"{\"TryCache\":true,\"Scope\":\"{Site.URL}\",\"UpdateLinksForCatalogItems\":true,\"EnableStacking\":true,\"ListId\":\"5cd01485-1473-49ba-b866-32132177ddd1\",\"ListItemId\":4}","ClientType":"ContentSearchRegular","UpdateAjaxNavigate":true,"SummaryLength":180,"DesiredSnippetLength":90,"PersonalizedQuery":false,"FallbackRefinementFilters":null,"IgnoreStaleServerQuery":false,"RenderTemplateId":"DefaultDataProvider","AlternateErrorMessage":null,"Title":""}';  

            }

            # Save changes to webpartmanager. This step is necessary. Otherwise changes won't be reflected  
            $webpartmanager.SaveChanges($wp);  
       
            break;   
   
        }   
    }

    # Check in and Publish the page  
    $page.CheckIn("Quick Links")  
    $page.Publish("Quick Links") 

}

function UpdateAllQuickLinksPage
{

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
$siteUrl = "https://thethread.carpetright.co.uk"


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
           $homePage = "Home.aspx"
           $quickLinksWP = "Quick Links"
           # update web parts in home page
           UpdatePage $siteUrl $subsite $homePage $quickLinksWP
           
           $allQuickLinksPage = "AllQuickLinks.aspx"
           $allQuickLinksWP = "All Quick Links"
           # update web parts in all quick links page
           UpdatePage $siteUrl $subsite $allQuickLinksPage $allQuickLinksWP
           UpdatePage $siteUrl $subsite $allQuickLinksPage $quickLinksWP

        } else
        {
            WriteToLog "    Not a valid store number.  Site ignored." "warning"
        }

        # Update the SPWeb object  
        $subsite.Update();   
    
        # Dispose SPWeb object  
        $subsite.Dispose();

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