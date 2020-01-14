# Cumulocity IOT ML Workbench
[<img src="https://demo.zmod.org/assets/logo-aw.png" alt="zmod_logo" height="80" style="float:right"/>](https://www.zmod.org/)

Making and (re-)training machine learning and deep learning models.

[![Build status](https://dev.azure.com/zementis-ai/Cumulocity%20IOT%20ML%20Workbench/_apis/build/status/CI-Pipeline%20(ZMOD))](https://dev.azure.com/zementis-ai/Cumulocity%20IOT%20ML%20Workbench/_build/latest?definitionId=6)

<p align="left">
  <img width="100%" src="https://github.com/SoftwareAG/ZMOD/blob/master/docs/quick-snaps.gif">
</p>

**Cumulocity IOT ML Workbench** is an open source machine learning and artificial intelligence platform for Data Scientist to solve business problems faster and quicker, build prototypes and convert them to actual project. The modeler helps from data preparation to model building and deployment, the tool supports a large variety of algorithms that can be run without a single line of code. The web based tool has various components which help Data Scientist of different skill levels to perfrom several model building tasks and provides deployment ready PMML files which can be hosted as a REST services.

Cumulocity IOT ML Workbench allows it's user to cover a wide variety of algorithms and Deep Neural Network architectures, with minimal or No code enviornment. It is also one of the few deep-learning platforms to support the Predictive Model Markup Languaue (PMML) format, PMML allows for different statistical and data mining tools to speak the same language.
* **AutoML** : Automatically train Machine learning models on data supports huge space of algorithms and hyper parameters tuning.
* **Model Editor** : Create Deep Neural Network models using drag and drop functionality, which supports wide variety of model layers, once model architecture is ready, train your model. Editor also comes with pre-trained architcture templates that helps in quick model building and training.
* **Jupyter Notebook** : It has integrated Jupyter Notebook (For R and Python).
* **Tensorboard** : It provides Tensorboard dashboard to show the progress of models.
* **Code Execution** : It provides support of executing Python script files for more advance requirements.
* **REST API Support** :  It has REST APIs to use as deployment tool.

## Getting Started

## Feature(s)

## Getting familiar with Cumulocity IOT ML Workbench :
It has 6 sections : 
1. Data
2. Model
3. Code
4. Tasks
5. Repo
6. Assets

**Data** : In this section, user uploads the data and can manage the data by performing all the CRUD operations. The data section supports, zip folders, image files, mp4 files, csv files.

**Model** : In this section, all the models are being managed, all the models will be PMML format only. The model files can be of machine learning based models or Neural Network based models, the models can have only structure or can also have weights with the model structures.

**Code** : Code section supports Python script files (.py files) and Jupyter Notebook files (.ipynb files). A user can upload python script files, notebook files and execute them in the Zementis Modeler. There are no options of creating new files in the UI, a user has to upload his files and then only they will be ready for executions. A user can edit files once uploaded to the Zmenentis Modeler.

**Tasks** : This section provides information about all the running tasks in the Zementis Modeler System, it gives info about running or finished AutoML tasks, Deep Neural Network model training status, and if any code execution process status, and any jupyter notebook instance running, a user can delete a running task from this section.

**Repo** : Data, Code and Models are critical resource(s) for ai project. Repo helps to do versioning these resources and manage their dependencies centrally so that ML open source communities get access with UMOYA CLI for cloud and on-premises Cumulocity IOT ML Workbench environment.

**Assets** : Assets section provide more info regarding the available hardware systems, used ports and components of Zementis Modeler. if any process needs to be killed or resource to be freed can be controlled from this section.

## Submitting patches
1. Fork and create branch.
2. Commit your changes.
3. Submit a PR, DevOps pipeline will run all tests.
4. Address issues in the review and build failures.
5. Before merge rebase on master `git rebase -i master` and possibly squash some of the commits.

## Issues ?
If you have an idea or found a bug, open an issue to discuss it.

## Quick Walk-Throught on couple of Use Case(s) :

[Demo 1 : Distracted Driver Use Case](https://www.zmod.org/learn/0SZHvRSJwyc)<br/>
[Demo 2 : AutoML](https://www.zmod.org/learn/6RUJ0Nk52u8)<br/>
[Demo 3 : Import data from Cumulocity](https://www.zmod.org/learn/_YCGPhMETF4)<br/>
[Demo 4 : Jupyter Notebook](https://www.zmod.org/learn/dm98Rjb8l6w)<br/>
[Demo 5 : Python Execution](https://www.zmod.org/learn/Hnu7lvcc0kc)<br/>


## Support
You can ask questions at
*	[https://stackoverflow.com](https://stackoverflow.com) by tagging your questions with #pmml, #zmod
*	You can also post bug reports in [GitHub issues](https://github.com/SoftwareAG/ZMOD/issues) 
______________________
These tools are provided as-is and without warranty or support. They do not constitute part of the Software AG product suite. Users are free to use, fork and modify them, subject to the license agreement. While Software AG welcomes contributions, we cannot guarantee to include every contribution in the master project.	

Contact us at [TECHcommunity](mailto:technologycommunity@softwareag.com?subject=Github/SoftwareAG) if you have any questions.

## Credits to open source projects used as reference
