# ML Workbench
[<img src="https://github.com/SoftwareAG/ZMOD/blob/master/docs/logo.png" alt="zmod_logo" height="50" style="float:right"/>](https://www.zmod.org/)

Making and (re-)training machine learning and deep learning models.

[![Build status](https://dev.azure.com/zementis-ai/ML%20Workbench/_apis/build/status/CI-Pipeline%20(MLW))](https://dev.azure.com/zementis-ai/ML%20Workbench/_build/latest?definitionId=6)

<p align="left">
  <img width="100%" src="https://github.com/SoftwareAG/ZMOD/blob/master/docs/quick-snaps.gif">
</p>

**ML Workbench** is an open source machine learning and artificial intelligence platform for Data Scientist to solve business problems faster and quicker, build prototypes and convert them to actual project. The modeler helps from data preparation to model building and deployment, the tool supports a large variety of algorithms that can be run without a single line of code. The web based tool has various components which help Data Scientist of different skill levels to perfrom several model building tasks and provides deployment ready PMML files which can be hosted as a REST services.

ML Workbench allows it's user to cover a wide variety of algorithms and Deep Neural Network architectures, with minimal or No code enviornment. It is also one of the few deep-learning platforms to support the Predictive Model Markup Languaue (PMML) format, PMML allows for different statistical and data mining tools to speak the same language.

## Getting Started
1. Visit live instance of [ML Workbench](https://demo.mlw.ai/)
2. Use your github account or register with us and login.
3. Explore Use case(s) and product features from [YouTube video guide](https://www.mlw.ai/learn)<br/>
   <b>Couple of popular topics</b>
   1. [AutoML](https://www.mlw.ai/learn/)<br/>
   2. [Distracted Driver Use Case](https://www.mlw.ai/learn/)<br/>
   3. [Import data from Cumulocity](https://www.zmod.org/learn/)<br/>
   4. [Jupyter Notebook](https://www.zmod.org/learn/)<br/>
   5. [Python Execution](https://www.zmod.org/learn/)<br/>
4. For any query, You can visit [help documents](https://github.com/SoftwareAG/MLW/tree/master/docs) 

## Feature(s)
1. AutoML for automated training of Machine Learning models.
2. Anomaly detection training models support.
3. Support for Neural Network and Workflow for AI projects.
4. User Interface to drag and drop, edit, train or load models in memory.
5. One-Click deployment to Cumulocity IoT Predictive Analytics (ZAD).
6. Integration with Cumulocity and DataHub.
7. Integration with Data Scientist's Tools such as Jupyter Notebook and Tensorboard.
8. Integration with [Repo \ UMOYA](https://hub.umoya.ai/) as a model management framework for versioning models and its resource (data and code) dependencies.
9. User-specific configurations to connect various Cumulocity IOT tenants.
10. Swagger Rest API(s) [Interface](https://demo.mlw.ai/swagger/index.html)
11. Docker [support](https://hub.docker.com/search?q=SoftwareAG&type=image)
12. Built in [UMOYA CLI](https://hub.umoya.ai/packages/umoya) in ML Workbench's Docker Container Shell.
13. Enable [DevOps](https://dev.azure.com/zementis-ai/ML%20Workbench/_build/latest?definitionId=6).


## Submitting patches
1. Fork and create branch.
2. Commit your changes; make sure you have unit/integration tests.
3. Submit a PR, DevOps pipeline will run all tests.
4. Address issues in the review and build failures.
5. Before merge rebase on master `git rebase -i master` and possibly squash some of the commits.

## Issues ?
If you have an idea or found a bug, open an issue to discuss it.

## Support
You can ask questions at
*	[https://stackoverflow.com](https://stackoverflow.com) by tagging your questions with #pmml, #zmod, #mlw
*	You can also post bug reports in [GitHub issues](https://github.com/SoftwareAG/ZMOD/issues) 

## License
This project is under [Apache License 2.0](https://github.com/SoftwareAG/MLW/blob/master/LICENSE). It has list of 3rd party components and libraries. Get more details from [here](https://github.com/SoftwareAG/MLW/blob/master/Third-Party%20License(s)%20Terms.pdf) 

This tool is provided as-is and without warranty or support. They do not constitute part of the Software AG product suite. Users are free to use, fork and modify them, subject to the license agreement. While Software AG welcomes contributions, we cannot guarantee to include every contribution in the master project.	

Contact us at [TECHcommunity](mailto:technologycommunity@softwareag.com?subject=Github/SoftwareAG) if you have any questions.
