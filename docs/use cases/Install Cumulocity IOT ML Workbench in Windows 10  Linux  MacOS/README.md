# **Install Cumulocity IOT ML Workbench in Windows 10 / Linux / MacOS with docker**

# Feature tags
## 10.5 (Latest)
- docker pull store/softwareag/mlw-zmm:10.5
- docker pull store/softwareag/mlw-zmk:10.5

## 1.38.2
- docker pull store/softwareag/zementis-modeler-zmm:1.38.2
- docker pull store/softwareag/zementis-modeler-zmk:1.38.2
- docker pull store/softwareag/zementis-modeler-nginx:1.38.2

# Pre-requisites
Please, follow installation instructions as given below.
- Keep 30 GB memory space for Cumulocity IOT ML Workbench application.
- Docker 18.09 CE onwards
- Docker-compose version 1.23.2

## Note :
Installation time may take up to 20 mins depending on the machine type and/or internet connectivity.
you can extract or pull images in parallel by opening multiple commands prompts tab.

# Windows 10

## Steps to install Cumulocity IOT ML Workbench in Windows 10 Machine with docker :

**Step 1**: The first step is to download the docker installer from https://hub.docker.com/editions/community/docker-ce-desktop-windows

**Step 2**: Before installation, we need to turn on Hyper-V in “Turn Windows features on or off”
- Search for “Turn Windows features on or off” in global search and turn on Hyper-V. Restart your computer

**Step 3**: Once this is done, we need to start Docker Desktop with administrator privilege

**Step 4**: Docker icon will be visible on the system tray now
- Next step is to go to docker settings by right clicking on docker icon
- In those settings, tick the C drive in Shared Drives tab

**Step 5**: Now docker is up and running. Next step is to install Cumulocity IOT ML Workbench docker images

**Step 6**: Open PowerShell in administrator mode and say “docker login”. Supply your docker username and password to log in. After the login is successful, please type the below commands to pull the Cumulocity IOT ML Workbench docker images 


| Version | Command to pull zmm| Command to pull zmk |
| --- | --- | --- |
| 10.5 | docker pull store/softwareag/mlw-zmm:10.5  | docker pull store/softwareag/mlw-zmk:10.5 |
| 1.38.2 | docker pull store/softwareag/zementis-modeler-zmm:1.38.2 | docker pull store/softwareag/zementis-modeler-zmk:1.38.2 |

For 1.38.2, Please run below command as well
docker pull store/softwareag/zementis-modeler-nginx:1.38.2

**Step 7**: Create a folder in your preferred location
``` 
mkdir MLWORKBENCH
```
- Navigate to that folder 
``` 
cd MLWORKBENCH 
```

**Step 8**: Now copy file(s) from the below link to MLWORKBENCH directory

| Version | github file location| 
| --- | --- |
| 10.5 | https://github.com/SoftwareAG/MLW/blob/master/docker/10.5/Windows/install.ps1  | 
| 1.38.2 | https://github.com/SoftwareAG/MLW/tree/1.38.2/Docker/Windows | 


**Step 9**: Now run the script install.ps1 from Powershell for 10.5
``` 
install.ps1 
mlw up
``` 

**Step 10**: This will launch the Cumulocity IOT ML Workbench. If you are running version 1.38.2, type "localhost" in your browser.

**Step 11**: To stop Cumulocity IOT ML Workbench application, type the below command from the directory where mlw up was typed
```
mlw down
```

# Linux

## Steps to install Cumulocity IOT ML Workbench in Linux Machine with docker :

### if you have standalone Linux system, please proceed to step 4

### If you do not have a standalone Linux system, than it can be installed using your preferred virtualization software [VMWare](https://my.vmware.com/en/web/vmware/info/slug/desktop_end_user_computing/vmware_workstation_pro/15_0) or [Virtual Box](https://www.virtualbox.org/). Start from Step 1

**Step 1**: The first step is to download the VM player. You could do that in the below link

- https://www.vmware.com/products/workstation-player/workstation-player-evaluation.html

**Step 2**: Next step is to download the latest Ubuntu image from the below website 

- https://www.ubuntu.com/#download

**Step 3**: Next, open the VM player and create a new virtual machine for Ubuntu. The installation might take some time. 
- An example of installing Ubuntu using VMWare [here](https://websiteforstudents.com/how-to-install-ubuntu-16-04-17-10-18-04-on-vmware-workstation-guest-machines/)

**Step 4**: The next step is to install the Docker CE and Docker Compose. Below are the links to do so

- https://docs.docker.com/install/linux/docker-ce/ubuntu/
- https://docs.docker.com/compose/install/


**Step 5**: Now Docker is successfully installed in your system

**Step 6**: Open command prompt and say 
```
sudo docker login
```
Supply your docker username and password to log in. After the login is successful, please type the below commands to pull the Cumulocity IOT ML Workbench docker images 

| Version | Command to pull zmm| Command to pull zmk |
| --- | --- | --- |
| 10.5 | docker pull store/softwareag/mlw-zmm:10.5  | docker pull store/softwareag/mlw-zmk:10.5 |
| 1.38.2 | docker pull store/softwareag/zementis-modeler-zmm:1.38.2 | docker pull store/softwareag/zementis-modeler-zmk:1.38.2 |

For 1.38.2, Please run below command as well
docker pull store/softwareag/zementis-modeler-nginx:1.38.2

**Step 7**: Create a folder in your preferred location
``` 
mkdir MLWORKBENCH
```
- Navigate to that folder 
``` 
cd MLWORKBENCH 
```

**Step 8**: Now copy all the files from the below link to MLWORKBENCH directory

| Version | github file location| 
| --- | --- |
| 10.5 | https://github.com/SoftwareAG/MLW/tree/10.5/docker/10.5/Linux  | 
| 1.38.2 | https://github.com/SoftwareAG/MLW/tree/1.38.2/Docker/Linux | 


**Step 9**: Run startup.sh to setup mlw command 
```
bash startup.sh

source ~/.bashrc
```

**Step 10**: You can start Cumulocity IOT ML Workbench application from any directory by typing the below command
```
mlw up
```

**Step 11**: Open your browser type "localhost:7007". This will launch the Cumulocity IOT ML Workbench. If you are running version 1.38.2, type "localhost" in your browser.

**Step 12**: To stop Cumulocity IOT ML Workbench application, type the below command from the directory where mlw up was typed
```
mlw down
```

# MacOS

## Steps to install Cumulocity IOT ML Workbench in MacOS Machine with docker :

**Step 1**: The first step is to install the Docker and Docker Compose. Below are the links to do so
* https://docs.docker.com/docker-for-mac/
* https://docs.docker.com/compose/install/

**Step 2**: Now Docker is successfully installed in your system

**Step 3**: Open command prompt and say 
```
sudo docker login
```
Supply your docker username and password to log in. After the login is successful, please type the below commands to pull the Cumulocity IOT ML Workbench docker images 

| Version | Command to pull zmm| Command to pull zmk |
| --- | --- | --- |
| 10.5 | docker pull store/softwareag/mlw-zmm:10.5  | docker pull store/softwareag/mlw-zmk:10.5 |
| 1.38.2 | docker pull store/softwareag/zementis-modeler-zmm:1.38.2 | docker pull store/softwareag/zementis-modeler-zmk:1.38.2 |

For 1.38.2, Please run below command as well
docker pull store/softwareag/zementis-modeler-nginx:1.38.2

**Step 4**: Create a folder in your preferred location
``` 
mkdir MLWORKBENCH
```
- Navigate to that folder 
``` 
cd MLWORKBENCH 
```

**Step 5**: Now copy all the files from the below link to MLWORKBENCH directory

| Version | github file location| 
| --- | --- |
| 10.5 | https://github.com/SoftwareAG/MLW/tree/10.5/docker/10.5/Mac  | 
| 1.38.2 | https://github.com/SoftwareAG/MLW/tree/1.38.2/Docker/Mac | 


**Step 6**: Run startup.sh to setup Cumulocity IOT ML Workbench command 
```
bash startupMacOS.sh

source ~/.bashrc
```

**Step 7**: You can start Cumulocity IOT ML Workbench application from any directory by typing the below command
```
mlw up
```

**Step 8**: Open your browser type "localhost:7007". This will launch the Cumulocity IOT ML Workbench. If you are running version 1.38.2, type "localhost" in your browser.

**Step 9**: To stop Cumulocity IOT ML Workbench application, type the below command from the directory where Cumulocity IOT ML Workbench up was typed
```
mlw down
```