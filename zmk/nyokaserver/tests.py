# from django.test import TestCase
import unittest
import requests, os, json, sys, logging
from django.http import JsonResponse
from nyokaserver.nyokaServerClass import NyokaServer
from trainModel.training import Training
from trainModel import kerasUtilities,mergeTrainingNN

class TestNyokaServer(unittest.TestCase):

    @classmethod
    def setUpClass(self):
        logging.info("******* Running Test Cases for Nyoka Server Class *******")


    def createSamplePMML(self,sampNNMOdel,newfileName):
        filePathToWrite = os.path.abspath('testUseCase/supportData2/'+newfileName+'.pmml')
        filePathToRead = os.path.abspath('testUseCase/supportdata/'+sampNNMOdel+'.pmml')
        toWrite=None
        with open(filePathToRead,'r') as fp:
            toWrite = fp.read()

        with open(filePathToWrite,'w') as fp:
            fp.write(toWrite)

    def test_1_getDetailsOfPmml(self):
        self.createSamplePMML('sampNNMOdel','sampNNMOdel')
        logging.info("Test Case : Get details of pmml.")
        filePath = os.path.abspath('testUseCase/supportData2/sampNNMOdel.pmml')
        result = NyokaServer.getDetailsOfPMML(filePath)
        result = json.loads(result.__dict__['_container'][0])
        self.assertEqual('modelGeneratedFrom' in result, True)
        self.assertEqual(result['modelGeneratedFrom'], 'DeepNetwork')
        logging.info("PASSED")


    # def test_2_getGlobalMemory(self):
    #     logging.info("Test Case : Get global memory.")
    #     result = NyokaServer.getGlobalObject()
    #     result = json.loads(result.__dict__['_container'][0])
    #     self.assertEqual(result,{})
    #     logging.info("PASSED")

    def test_3_addArchitecture(self):
        self.createSamplePMML('sampNNMOdel','sampNNMOdel')
        logging.info("Test Case : Add pmml to global memory.")
        projectID = 'xyz1'
        filePath = os.path.abspath('testUseCase/supportData2/sampNNMOdel.pmml')
        result = NyokaServer.addArchitectureToGlobalMemoryDict(projectID,filePath)
        result = json.loads(result.__dict__['_container'][0])
        print ("result",result)
        self.assertEqual(result['filePath'],filePath)
        self.assertEqual(result['projectID'],'xyz1')
        self.assertEqual(result['architecture'],[])
        logging.info("PASSED")


    def test_4_addMobileNet(self):
        self.createSamplePMML('sampNNMOdel','sampNNMOdel')
        logging.info("Test Case : Mobilenet.")
        projectID = 'xyz1'
        filePath = os.path.abspath('testUseCase/supportData2/sampNNMOdel.pmml')
        payLoad={"itemType":"TEMPLATE","layerId":"Mobilenet_1","layerIndex":0,"templateId":"mobilenetArch","name":"Mobilenet","id":"K4899FIXEJDNR"}
        result = NyokaServer.updatetoArchitecture(payLoad, projectID)
        result = json.loads(result.__dict__['_container'][0])
        # print ("result",result)
        # self.assertEqual(result['filePath'],filePath)
        self.assertEqual(result['projectID'],'xyz1')
        self.assertEqual(len(result['architecture']),38)
        logging.info("PASSED")

    # def test_4_1_removeArch(self):
    #     logging.info("Test Case : Mobilenet.")
    #     projectID = 'xyz1'
    #     result = NyokaServer.removeArch(projectID)
    #     result = json.loads(result.__dict__['_container'][0])
    #     self.assertEqual(result['status'],'removed')
    #     logging.info("PASSED")

        

    def testCreateDNNaddDatalayer1(self):
        # self.createSamplePMML()
        # self.test_3_addArchitecture()
        self.createSamplePMML('sampNNMOdel','sampNNMOdel2')
        logging.info("Test Case : Add pmml to global memory.")
        projectID = 'xyz2'
        filePath = os.path.abspath('testUseCase/supportData2/sampNNMOdel2.pmml')
        result = NyokaServer.addArchitectureToGlobalMemoryDict(projectID,filePath)
        result = json.loads(result.__dict__['_container'][0])
        print ("result",result)
        self.assertEqual(result['filePath'],filePath)
        self.assertEqual(result['projectID'],'xyz2')
        self.assertEqual(result['architecture'],[])
        logging.info("PASSED")


        payLoad={"name":"Data","icon":"mdi mdi-database-plus","itemType":"DATA","layerId":"mpgFeatureData.csv","trainable":True,
        "id":"K49UR4EBABJZL","layerIndex":0,"url":"/api/data/preview/mpgFeatureData.csv",
        "filePath":"C:\\Users\\swsh\\Desktop\\ZMODGit\\ZMOD\\ZMOD\\Data\\mpgFeatureData.csv"}
        result = NyokaServer.updatetoArchitecture(payLoad, projectID)
        result = json.loads(result.__dict__['_container'][0])
        # print ("result",result)
        # self.assertEqual(result['filePath'],filePath)
        self.assertEqual(result['projectID'],'xyz2')
        self.assertEqual(result['layerUpdated']['name'],'Data')
        self.assertEqual(result['layerUpdated']['url'],'/api/data/preview/mpgFeatureData.csv')
        logging.info("PASSED")

        payLoad={"connectionLayerId":"mpgFeatureData.csv","itemType":"LAYER","layerId":"Input_2","layerIndex":1,"layerType":"Input",
        "trainable":True,"name":"Input","properties":[{"dataType":"array","hint":"Input Shape of the Data",
        "id":"inputDimension","label":"Input Dimension","options":[],"value":[7,1]},
        {"dataType":"array","hint":"Output Dimension","id":"outputDimension","label":"Output Dimension","value":[0]}],
        "id":"K49V01OTUQ60S"}
        result = NyokaServer.updatetoArchitecture(payLoad, projectID)
        result = json.loads(result.__dict__['_container'][0])
        # print ("result",result)
        # self.assertEqual(result['filePath'],filePath)
        self.assertEqual(result['projectID'],'xyz2')
        self.assertEqual(result['layerUpdated']['properties'][1]['value'],[7,1])
        logging.info("PASSED")

        payLoad={"connectionLayerId":"Input_2","itemType":"LAYER","layerId":"Dense_3","layerIndex":2,"layerType":"Dense","trainable":True,
        "name":"Dense","properties":[{"dataType":"string","hint":"Activation function for the dense layer","id":"activationFunction",
        "label":"Activation Function","options":["linear","tanh","relu","sigmoid","softmax"],"value":"relu"},
        {"dataType":"integer","hint":"Neurons for fully connected layer","id":"units","label":"Units","options":[],"value":200},
        {"dataType":"array","hint":"only required if dragged first","id":"inputDimension","label":"Input Dimension",
        "options":[],"value":[7,1]},{"dataType":"array","hint":"Output dimension of the layer","id":"outputDimension",
        "label":"Output Dimension","options":[],"value":[0,1]}],"id":"K49VNPRK4HN3L"}
        result = NyokaServer.updatetoArchitecture(payLoad, projectID)
        result = json.loads(result.__dict__['_container'][0])
        # print ("result",result)
        # self.assertEqual(result['filePath'],filePath)
        self.assertEqual(result['projectID'],'xyz2')
        self.assertEqual(result['layerUpdated']['properties'][3]['value'],[200,1])
        logging.info("PASSED")

        payLoad={"connectionLayerId":"Dense_3","itemType":"LAYER","layerId":"Dense_4","layerIndex":3,"layerType":"Dense","trainable":True,
        "name":"Dense","properties":[{"dataType":"string","hint":"Activation function for the dense layer","id":"activationFunction","label":"Activation Function",
        "options":["linear","tanh","relu","sigmoid","softmax"],"value":"relu"},{"dataType":"integer","hint":"Neurons for fully connected layer","id":"units","label":"Units","options":[],"value":50},
        {"dataType":"array","hint":"only required if dragged first","id":"inputDimension","label":"Input Dimension","options":[],"value":[200,1]},
        {"dataType":"array","hint":"Output dimension of the layer","id":"outputDimension","label":"Output Dimension","options":[],"value":[0,1]}],"id":"K49VUNV0SKVXR"}
        result = NyokaServer.updatetoArchitecture(payLoad, projectID)
        result = json.loads(result.__dict__['_container'][0])
        # print ("result",result)
        # self.assertEqual(result['filePath'],filePath)
        self.assertEqual(result['projectID'],'xyz2')
        self.assertEqual(result['layerUpdated']['properties'][3]['value'],[50,1])
        logging.info("PASSED")

        payLoad={"connectionLayerId":"Dense_3","itemType":"LAYER","layerId":"Dense_4","layerIndex":3,"layerType":"Dense","trainable":True,
        "name":"Dense","properties":[{"dataType":"string","hint":"Activation function for the dense layer","id":"activationFunction","label":"Activation Function",
        "options":["linear","tanh","relu","sigmoid","softmax"],"value":"relu"},{"dataType":"integer","hint":"Neurons for fully connected layer","id":"units","label":"Units","options":[],"value":50},
        {"dataType":"array","hint":"only required if dragged first","id":"inputDimension","label":"Input Dimension","options":[],"value":[200,1]},
        {"dataType":"array","hint":"Output dimension of the layer","id":"outputDimension","label":"Output Dimension","options":[],"value":[0,1]}],"id":"K49VUNV0SKVXR"}
        result = NyokaServer.updatetoArchitecture(payLoad, projectID)
        result = json.loads(result.__dict__['_container'][0])
        # print ("result",result)
        # self.assertEqual(result['filePath'],filePath)
        self.assertEqual(result['projectID'],'xyz2')
        self.assertEqual(result['layerUpdated']['properties'][3]['value'],[50,1])
        logging.info("PASSED")

        payLoad={"connectionLayerId":"Dense_4","itemType":"LAYER","layerId":"Dense_5","layerIndex":4,"layerType":"Dense","trainable":True,"name":"Dense","properties":[{"dataType":"string","hint":"Activation function for the dense layer","id":"activationFunction","label":"Activation Function","options":["linear","tanh","relu","sigmoid","softmax"],"value":"linear"},{"dataType":"integer","hint":"Neurons for fully connected layer","id":"units","label":"Units","options":[],"value":1},{"dataType":"array","hint":"only required if dragged first","id":"inputDimension","label":"Input Dimension","options":[],"value":[50,1]},{"dataType":"array","hint":"Output dimension of the layer","id":"outputDimension","label":"Output Dimension","options":[],"value":[0,1]}],"id":"K49VXHBWU2Y4D"}
        result = NyokaServer.updatetoArchitecture(payLoad, projectID)
        result = json.loads(result.__dict__['_container'][0])
        # print ("result",result)
        # self.assertEqual(result['filePath'],filePath)
        self.assertEqual(result['projectID'],'xyz2')
        self.assertEqual(result['layerUpdated']['properties'][3]['value'],[1,1])
        logging.info("PASSED")

    def test_5_addWorkflow(self):
        self.createSamplePMML('sampleWorkFlow','sampleWorkFlow')
        logging.info("Test Case : Workflow.")
        projectID = 'xyz3'
        filePath = os.path.abspath('testUseCase/supportData2/sampleWorkFlow.pmml')
        result = NyokaServer.addArchitectureToGlobalMemoryDict(projectID,filePath)
        result = json.loads(result.__dict__['_container'][0])
        print ("result",result)
        self.assertEqual(result['filePath'],filePath)
        self.assertEqual(result['projectID'],'xyz3')
        self.assertEqual(result['architecture'],[])
        logging.info("PASSED")

        payLoad={"name":"Section","layerId":"model1","children":[],"itemType":"FOLDING","icon":"mdi mdi-group","class":"wide","modelType":"Workflow","id":"K4DPDAXBYOSFD","sectionId":"K4DPDAXBESIKY","layerIndex":0}
        result = NyokaServer.updatetoWorkflow(payLoad, projectID)
        result = json.loads(result.__dict__['_container'][0])
        self.assertEqual(result['projectID'],'xyz3')
        logging.info("PASSED")

        payLoad={"name":"Section","layerId":"model2","children":[],"itemType":"FOLDING","icon":"mdi mdi-group","class":"wide","modelType":"Workflow","id":"K4DPFUIG9RLNI","sectionId":"K4DPFUIG4CEJ9","layerIndex":1,"connectionLayerId":"model1"}
        result = NyokaServer.updatetoWorkflow(payLoad, projectID)
        result = json.loads(result.__dict__['_container'][0])
        self.assertEqual(result['projectID'],'xyz3')
        logging.info("PASSED")

        payLoad={"name":"Model","icon":"mdi mdi-xml","itemType":"MODEL","layerId":"mpgSKModel.pmml","trainable":True,"modelType":"Workflow","id":"K4DPJVKITJOT4","sectionId":"K4DPDAXBESIKY","layerIndex":0,"url":"/model/mpgSKModel.pmml","filePath":"testUseCase/supportData2/mpgLRModel2.pmml","taskType":"score"}
        result = NyokaServer.updatetoWorkflow(payLoad, projectID)
        result = json.loads(result.__dict__['_container'][0])
        self.assertEqual(result['projectID'],'xyz3')
        logging.info("PASSED")

        payLoad={"name":"Code","icon":"mdi mdi-code-braces","itemType":"CODE","layerId":"postproLinearModel.py","trainable":True,"modelType":"Workflow","id":"K4DPL8AMCNQ7V","sectionId":"K4DPDAXBESIKY","layerIndex":1,"url":"/code/postproLinearModel.py","filePath":"testUseCase/supportData2/postproLinearModel.py","taskType":"postprocessing","scriptOutput":"NONE","scriptPurpose":"score"}
        result = NyokaServer.updatetoWorkflow(payLoad, projectID)
        result = json.loads(result.__dict__['_container'][0])
        self.assertEqual(result['projectID'],'xyz3')
        logging.info("PASSED")

        payLoad={"name":"Code","icon":"mdi mdi-code-braces","itemType":"CODE","layerId":"featureExtract.py","trainable":True,"modelType":"Workflow","id":"K4DPMID52XST6","sectionId":"K4DPFUIG4CEJ9","layerIndex":0,"url":"/code/featureExtract.py","filePath":"testUseCase/supportData2/featureExtract.py","taskType":"preprocessing","scriptOutput":"DATA","scriptPurpose":"score"}
        result = NyokaServer.updatetoWorkflow(payLoad, projectID)
        result = json.loads(result.__dict__['_container'][0])
        self.assertEqual(result['projectID'],'xyz3')
        logging.info("PASSED")

        payLoad={"name":"Model","icon":"mdi mdi-xml","itemType":"MODEL","layerId":"nnModel2WF.pmml","trainable":True,"modelType":"Workflow","id":"K4DPNK51ZPONV","sectionId":"K4DPFUIG4CEJ9","layerIndex":1,"connectionLayerId":"model1","url":"/model/nnModel2WF.pmml","filePath":"testUseCase/supportData2/nnModel2WF.pmml","taskType":"score"}
        result = NyokaServer.updatetoWorkflow(payLoad, projectID)
        result = json.loads(result.__dict__['_container'][0])
        self.assertEqual(result['projectID'],'xyz3')
        logging.info("PASSED")

        payLoad={"name":"Code","icon":"mdi mdi-code-braces","itemType":"CODE","layerId":"postNNModel.py","trainable":True,"modelType":"Workflow","id":"K4DPOLYIBPK85","sectionId":"K4DPFUIG4CEJ9","layerIndex":2,"connectionLayerId":"model1","url":"/code/postNNModel.py","filePath":"testUseCase/supportData2/postNNModel.py","taskType":"postprocessing","scriptOutput":"NONE","scriptPurpose":"score"}
        result = NyokaServer.updatetoWorkflow(payLoad, projectID)
        result = json.loads(result.__dict__['_container'][0])
        self.assertEqual(result['projectID'],'xyz3')
        logging.info("PASSED")

        

    @classmethod
    def tearDownClass(self):
        logging.info("******* Finished Test Cases for Nyoka Server Class *******\n")
