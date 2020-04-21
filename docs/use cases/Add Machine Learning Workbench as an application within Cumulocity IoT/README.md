# Add Machine Learning Workbench as an application within Cumulocity IoT

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

<img width="100%" src="https://github.com/SoftwareAG/MLW/blob/master/docs/use%20cases/Add%20Machine%20Learning%20Workbench%20as%20an%20application%20within%20Cumulocity%20IoT/snap-shots/quick-snaps.gif">

# Troubleshooting #

**ERROR: Pool overlaps with another one on this address space**. 
  * If you see the above error, make sure you stop the Cumulocity IOT ML Workbench application from the directory where it was started or remove the docker network by typing:
  * docker network rm ***network name***
  * If envsubst command does not exist in mac. Please follow instructions below to get the package:               https://github.com/tardate/LittleCodingKata/tree/master/tools/envsubst
**ERROR: for zmm  Cannot start service zmm: driver failed programming external connectivity on endpoint: Error starting userland proxy: mkdir /port/tcp:0.0.0.0:port:tcp: ip :port: input/output error
  * Stop all the running containers docker stop $(docker ps -a -q) then
  * Stop the Docker on your machine & restart it.

# Documentation #

The full documentation is available from the website: https://www.mlw.ai

# License #

License terms for this product can be found here: http://www.softwareag.com/license -> Limited Use License Agreement for Software AG Docker Images

# Base Image #

This product references the official [ubuntu](https://hub.docker.com/_/ubuntu) image as its base image. Software AG is not responsible for the contents of this base image.

This product references the official [debian9](https://hub.docker.com/_/debian) image as its base image. Software AG is not responsible for the contents of this base image.
