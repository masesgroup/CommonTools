param ($baseUrl = "https://masesgroup.com", $baseFolder = ".", $inSitemapFile = "sitemap.xml", $outSitemapFile = "sitemap.xml")

if ($inSitemapFile -eq $null)
{
    $inSitemapFile = "sitemap.xml"
}

$data = [xml](get-content -raw $inSitemapFile)
foreach ($url in $data.urlset.url ) {
    if ($url.loc.StartsWith($baseUrl) ) {
        $fullPath = Join-Path $baseFolder $url.loc.Substring($baseUrl.Length)
        if ([System.IO.File]::Exists($fullPath))
        {
            $path = [System.IO.Path]::GetDirectoryName($fullPath)
            $fileName = [System.IO.Path]::GetFileName($fullPath)
            $lastMod = @(cd $path) | Out-String
            $lastMod = @(git log -1 --pretty="format:%cI" $fileName) | Out-String
            if ($lastMod -ne $null)
            {
                $url.lastmod = $lastMod.ToString().Trim()
            }
        }
    }
}
$data.save($outSitemapFile)
