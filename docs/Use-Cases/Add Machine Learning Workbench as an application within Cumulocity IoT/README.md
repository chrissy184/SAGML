# **Steps to install Cumulocity IOT ML Workbench in Windows 10 / Linux / MacOS through docker**

<br>

# Feature tags
<br>
10.5 (Latest)

- docker pull store/softwareag/mlw-zmm:10.5
- docker pull store/softwareag/mlw-zmk:10.5

1.38.2
- docker pull store/softwareag/zementis-modeler-zmm:1.38.2
- docker pull store/softwareag/zementis-modeler-zmk:1.38.2
- docker pull store/softwareag/zementis-modeler-nginx:1.38.2

<br>

# Pre-requisites
Please pick your installation instructions in the following according to the OS you are using.

- Keep 30 GB memory space for Cumulocity IOT ML Workbench application.
- Docker 18.09 CE onwards
- Docker-compose version 1.23.2

## Note :
Installation time may take up to 20 mins depending on the machine type and/or internet connectivity.
you can extract or pull images in parallel by opening multiple commands prompts tab.

<br>

# [Windows 10](#Windows-10) | [Linux](#Linux) | [MacOS](#MacOS)

<br>

# Windows 10

## Steps to install Cumulocity IOT ML Workbench in Windows 10 machine through docker :

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

- For 1.38.2 in addition to the above commands, the below command also needs to be run

  - docker pull store/softwareag/zementis-modeler-nginx:1.38.2


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
| 10.5 | https://github.com/SoftwareAG/MLW/tree/10.5/docker/10.5/Windows  | 
| 1.38.2 | https://github.com/SoftwareAG/MLW/tree/1.38.2/Docker/Windows | 


**Step 9**: Now run the script mlw_up.ps1 from Powershell
``` 
mlw_up.ps1 
``` 

**Step 10**: Open your browser type "localhost:7007". This will launch the Cumulocity IOT ML Workbench. If you are running version 1.38.2, type "localhost" in your browser.

**Step 11**: To stop Cumulocity IOT ML Workbench application, type the below command from the directory where mlw up was typed
```
mlw_down.ps1 
```

<br>

# Linux

## Steps to install Cumulocity IOT ML Workbench in Linux machine through docker :

### if you have standalone Linux system, please proceed to step 4

### If you do not have a standalone Linux system, than it can be installed using your preferred virtualization software [VMWare](https://my.vmware.com/en/web/vmware/info/slug/desktop_end_user_computing/vmware_workstation_pro/15_0) or [Virtual Box](https://www.virtualbox.org/). Start from Step 1

<br>

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

- For 1.38.2 in addition to the above commands, the below command also needs to be run

  - docker pull store/softwareag/zementis-modeler-nginx:1.38.2



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

<br>

# MacOS

## Steps to install Cumulocity IOT ML Workbench in MacOS machine through docker :

**Step 1**: The first step is to install the Docker and Docker Compose. Below are the links to do so

- https://docs.docker.com/docker-for-mac/
- https://docs.docker.com/compose/install/

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

- For 1.38.2 in addition to the above commands, the below command also needs to be run

  - docker pull store/softwareag/zementis-modeler-nginx:1.38.2

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

<br>

# Adding Machine Learning Workbench as an application within Cumulocity IoT

**Step 1**: Make sure the MLW is up and running in the Docker environment

**Step 2**: Login to your Cumulocity IoT instance and select 'Administration' from the App Switcher 

**Step 3**: Navigate to Applications -> Own applications -> Add application

**Step 4**: Choose 'External Application' and add in the following details and click on 'Save'

  - Name: MLW
  - Application Key: NA
  - External URL: http://localhost:7007

**Step 5**: Refresh the browser and 'MLW' app would be now part of the Cumulocity IoT App Switcher

**Step 6**: Login to the Machine Learning Workbench and navigate to the settings page. Update your tenant id, username, password, url and set the selected flag to "true" under "Cumulocity" section. Save the settings page

**Step 7**: We could now navigate to our Cumulocity IoT instance using the App Switcher within MLW 

<br>

# Troubleshooting #

  * **ERROR: Pool overlaps with another one on this address space**. 
    * If you see the above error, make sure you stop the Cumulocity IOT ML Workbench application from the directory where it was started or remove the docker network by typing:
       * docker network rm ***network name***

  * If envsubst command does not exist in mac. Please follow instructions below to get the package:
https://github.com/tardate/LittleCodingKata/tree/master/tools/envsubst


  * ERROR: for zmm  Cannot start service zmm: driver failed programming external connectivity on endpoint: Error starting userland proxy: mkdir /port/tcp:0.0.0.0:port:tcp: ip :port: input/output error

    * Stop all the running containers docker stop $(docker ps -a -q) then
    * Stop the Docker on your machine & restart it.

<br>

# Documentation #

The full documentation is available from the website: https://www.mlw.ai

<br>

# License #

License terms for this product can be found here: http://www.softwareag.com/license -> Limited Use License Agreement for Software AG Docker Images

<br>

# Base Image #

This product references the official [ubuntu](https://hub.docker.com/_/ubuntu) image as its base image. Software AG is not responsible for the contents of this base image.

This product references the official [debian9](https://hub.docker.com/_/debian) image as its base image. Software AG is not responsible for the contents of this base image.
