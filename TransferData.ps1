$location = Get-Location
Copy-Item "$location\tool\bin\Release\netcoreapp3.1\Abilities.json" -Destination "$location\src\Data\Abilities.json"
Copy-Item "$location\tool\bin\Release\netcoreapp3.1\Clusters.json" -Destination  "$location\src\Data\Clusters.json"
Copy-Item "$location\tool\bin\Release\netcoreapp3.1\Heroes.json" -Destination "$location\src\Data\Heroes.json"
Copy-Item "$location\tool\bin\Release\netcoreapp3.1\Items.json" -Destination "$location\src\Data\Items.json"
Copy-Item "$location\tool\bin\Release\netcoreapp3.1\Modes.json" -Destination  "$location\src\Data\Modes.json"
Copy-Item "$location\tool\bin\Release\netcoreapp3.1\Regions.json" -Destination  "$location\src\Data\Regions.json"