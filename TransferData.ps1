$location = Get-Location
Copy-Item "$location\Abilities.json" -Destination "$location\src\Data\Abilities.json"
Copy-Item "$location\Clusters.json" -Destination  "$location\src\Data\Clusters.json"
Copy-Item "$location\\Heroes.json" -Destination "$location\src\Data\Heroes.json"
Copy-Item "$location\Items.json" -Destination "$location\src\Data\Items.json"
Copy-Item "$location\Modes.json" -Destination  "$location\src\Data\Modes.json"
Copy-Item "$location\\Regions.json" -Destination  "$location\src\Data\Regions.json"