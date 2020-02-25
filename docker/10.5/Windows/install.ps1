if('~/.mlw' | Test-Path){Remove-Item ~/.mlw -Recurse -Force}
#!Remove-Item '~/.mlw' -Recurse -ErrorAction Ignore
#!New-Item -ItemType "directory" -Path "~/.mlw" -ErrorAction SilentlyContinue
mkdir ~/.mlw | out-null
'version: "3.5"
services:
   zmm:
    image: store/softwareag/mlw-zmm:10.5 
    container_name: zmm
    command: /bin/bash -c "dotnet App.dll"
    working_dir: /publish
    volumes:
      - shared-content:/ZMOD
      - ./Code:/ZMOD/Code
      - ./Data:/ZMOD/Data
      - ./Models:/ZMOD/Models 
      - /var/run/docker.sock:/var/run/docker.sock
    ports:
      - "7008:7008"
      - "7007:7007"  
    expose:
       - "7008"
       - "7007"  
    restart: always
    networks:
      test:
        ipv4_address: "172.20.0.3"
networks:
  test:
    name: global-zmod-net
    ipam:
      driver: default
      config:
       - subnet: 172.20.0.0/24
volumes:
  shared-content:
' > '~/.mlw/docker-compose.yml'

'$COMPOSE_CONFIG = "~/.mlw/docker-compose.yml"
$trackPath = "~/.mlw/track"
$path = $pwd.path
$url = "http://localhost:7007/"

Function forceStopMLW{
	docker container rm -f zmk1 | out-null
	Write-Host "Stopped zmk1" -ForegroundColor green
	docker network disconnect -f global-zmod-net zmm | out-null
	docker network rm global-zmod-net | out-null
	Write-Host "Stopped global-zmod-net" -ForegroundColor green
	docker container rm -f zmm | out-null
	Write-Host "Stopped zmm" -ForegroundColor green
}
Function stopMLW{
	If($(docker inspect --format . zmm) -eq "."){
		Try{
			$runningPath = get-content -Path $trackPath -ErrorAction Ignore
			# Write-Host $runningPath
			Set-Location $runningPath
			docker container rm -f zmk1
			Get-Content $COMPOSE_CONFIG | envsubst | docker-compose -f - down
			Clear-Content -Path $trackPath -ErrorAction Ignore
			Set-Location $path
			Write-Host "MLW is stopped now !!!" -ForegroundColor green						
		}
		Catch{
			Write-Host "Unable to Stop MLW. Trying Force Stop" -ForegroundColor red
			forceStopMLW
		}
	}
	Else{
		Write-Host "MLW is not Running !!!" -ForegroundColor green
	}
}
Function openBrowser{
	explorer $url
}
Function startMLW{
	Write-Host "Starting MLW" -ForegroundColor green
	# Set-Location $args[0]
	Get-Content $COMPOSE_CONFIG | envsubst | docker-compose -f - up -d
	$args[0] | Out-File -FilePath $trackPath
	Write-Host "MLW is started at path: $args" -ForegroundColor green
	Start-Sleep 1
	openBrowser
	# Write-Host "Type MLW e to open browser" -ForegroundColor yellow
}

If($args[0] -eq "down" -Or $args[0] -eq "stop"){
	Write-Host "Stopping MLW !!!" -ForegroundColor green
	stopMLW
}

ElseIf($args[0] -eq "up" -Or !$args[0] -Or $args[0] -eq "." -Or $args[0] -eq "start"){
	If($args[1]){
	#!TODO : Update Path if given 2nd Argument (path)		
	}
	If($(docker inspect --format . zmm) -eq "."){
		$runningPath = get-content -Path $trackPath -ErrorAction Ignore
		Write-Host "MLW is already running at path: $runningPath." -ForegroundColor green
		Write-Host "Type MLW e to open browser" -ForegroundColor yellow
		If($path -eq $runningPath){Exit}
		$YorN = Read-Host "Do you want to stop it and start a new from current directory ? (Y/N)"
		If($YorN -eq "Y" -Or $YorN -eq "y"){
			stopMLW
			startMLW $path
		}
		ElseIf($YorN -eq "N" -Or $YorN -eq "n"){
			Write-Output "Aborting ..."
		}
		Else{
			Write-Output "Invalid Argument"
		}
	}	
	Else{
		startMLW $path
	}
}
ElseIf($args[0] -eq "e"){
	If($(docker inspect --format . zmm) -eq "."){openBrowser}
	Else{Write-Host "MLW is not runnning. try MLW up" -ForegroundColor red}
}
Else {Write-Output "Invalid Argument"}

' > '~/.mlw/mlw.ps1'

If(!(test-path $profile)){
  New-Item -ItemType File -Force -Path $profile | out-null
}
Else{
  Set-Content -Path $profile -Value (get-content -Path $profile -ErrorAction Ignore | Select-String -Pattern 'new-alias mlw' -NotMatch)
}
add-content -path $profile -value "new-alias mlw ~/.mlw/mlw.ps1"

start-process powershell
stop-process -Id $PID
