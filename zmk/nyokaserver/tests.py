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


    def createSamplePMML(self):
        filePathToWrite = os.path.abspath('testUseCase/supportData2/sampNNMOdel.pmml')
        filePathToRead = os.path.abspath('testUseCase/supportdata/sampNNMOdel.pmml')
        toWrite=None
        with open(filePathToRead,'r') as fp:
            toWrite = fp.read()

        with open(filePathToWrite,'w') as fp:
            fp.write(toWrite)

    def test_1_getDetailsOfPmml(self):
        self.createSamplePMML()
        logging.info("Test Case : Get details of pmml.")
        filePath = os.path.abspath('testUseCase/supportData2/sampNNMOdel.pmml')
        result = NyokaServer.getDetailsOfPMML(filePath)
        result = json.loads(result.__dict__['_container'][0])
        self.assertEqual('modelGeneratedFrom' in result, True)
        self.assertEqual(result['modelGeneratedFrom'], 'DeepNetwork')
        logging.info("PASSED")


    def test_2_getGlobalMemory(self):
        logging.info("Test Case : Get global memory.")
        result = NyokaServer.getGlobalObject()
        result = json.loads(result.__dict__['_container'][0])
        self.assertEqual(result,{})
        logging.info("PASSED")

    def test_3_addArchitecture(self):
        self.createSamplePMML()
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
        self.createSamplePMML()
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

    def test_4_1_removeArch(self):
        self.createSamplePMML()
        logging.info("Test Case : Mobilenet.")
        projectID = 'xyz1'
        result = NyokaServer.removeArch(projectID)
        result = json.loads(result.__dict__['_container'][0])
        self.assertEqual(result['status'],'removed')
        logging.info("PASSED")

    # def testCreateDNNaddDatalayer1(self):
    #     self.createSamplePMML()
    #     self.test_3_addArchitecture()
    #     projectID = 'xyz1'
    #     payLoad={"name":"Data","icon":"mdi mdi-database-plus","itemType":"DATA","layerId":"mpgFeatureData.csv","trainable":True,
    #     "id":"K49UR4EBABJZL","layerIndex":0,"url":"/api/data/preview/mpgFeatureData.csv",
    #     "filePath":"C:\\Users\\swsh\\Desktop\\ZMODGit\\ZMOD\\ZMOD\\Data\\mpgFeatureData.csv"}
    #     result = NyokaServer.updatetoArchitecture(payLoad, projectID)
    #     result = json.loads(result.__dict__['_container'][0])
    #     # print ("result",result)
    #     # self.assertEqual(result['filePath'],filePath)
    #     self.assertEqual(result['projectID'],'xyz1')
    #     self.assertEqual(result['layerUpdated']['name'],'Data')
    #     self.assertEqual(result['layerUpdated']['url'],'/api/data/preview/mpgFeatureData.csv')
    #     logging.info("PASSED")

    # def testCreateDNNaddDatalayer2(self):
    #     projectID = 'xyz1'
    #     payLoad={"connectionLayerId":"mpgFeatureData.csv","itemType":"LAYER","layerId":"Input_2","layerIndex":1,"layerType":"Input",
    #     "trainable":True,"name":"Input","properties":[{"dataType":"array","hint":"Input Shape of the Data",
    #     "id":"inputDimension","label":"Input Dimension","options":[],"value":[7,1]},
    #     {"dataType":"array","hint":"Output Dimension","id":"outputDimension","label":"Output Dimension","value":[0]}],
    #     "id":"K49V01OTUQ60S"}
    #     result = NyokaServer.updatetoArchitecture(payLoad, projectID)
    #     result = json.loads(result.__dict__['_container'][0])
    #     # print ("result",result)
    #     # self.assertEqual(result['filePath'],filePath)
    #     self.assertEqual(result['projectID'],'xyz1')
    #     self.assertEqual(result['layerUpdated']['properties'][1]['value'],[7,1])
    #     logging.info("PASSED")

    # def testCreateDNNaddDatalayer3(self):
    #     projectID = 'xyz1'

    #     payLoad={"connectionLayerId":"Input_2","itemType":"LAYER","layerId":"Dense_3","layerIndex":2,"layerType":"Dense","trainable":True,
    #     "name":"Dense","properties":[{"dataType":"string","hint":"Activation function for the dense layer","id":"activationFunction",
    #     "label":"Activation Function","options":["linear","tanh","relu","sigmoid","softmax"],"value":"relu"},
    #     {"dataType":"integer","hint":"Neurons for fully connected layer","id":"units","label":"Units","options":[],"value":200},
    #     {"dataType":"array","hint":"only required if dragged first","id":"inputDimension","label":"Input Dimension",
    #     "options":[],"value":[7,1]},{"dataType":"array","hint":"Output dimension of the layer","id":"outputDimension",
    #     "label":"Output Dimension","options":[],"value":[0,1]}],"id":"K49VNPRK4HN3L"}
    #     result = NyokaServer.updatetoArchitecture(payLoad, projectID)
    #     result = json.loads(result.__dict__['_container'][0])
    #     # print ("result",result)
    #     # self.assertEqual(result['filePath'],filePath)
    #     self.assertEqual(result['projectID'],'xyz1')
    #     self.assertEqual(result['layerUpdated']['properties'][3]['value'],[200,1])
    #     logging.info("PASSED")

    # def testCreateDNNaddDatalayer4(self):
    #     projectID = 'xyz1'

    #     payLoad={"connectionLayerId":"Dense_3","itemType":"LAYER","layerId":"Dense_4","layerIndex":3,"layerType":"Dense","trainable":True,
    #     "name":"Dense","properties":[{"dataType":"string","hint":"Activation function for the dense layer","id":"activationFunction","label":"Activation Function",
    #     "options":["linear","tanh","relu","sigmoid","softmax"],"value":"relu"},{"dataType":"integer","hint":"Neurons for fully connected layer","id":"units","label":"Units","options":[],"value":50},
    #     {"dataType":"array","hint":"only required if dragged first","id":"inputDimension","label":"Input Dimension","options":[],"value":[200,1]},
    #     {"dataType":"array","hint":"Output dimension of the layer","id":"outputDimension","label":"Output Dimension","options":[],"value":[0,1]}],"id":"K49VUNV0SKVXR"}
    #     result = NyokaServer.updatetoArchitecture(payLoad, projectID)
    #     result = json.loads(result.__dict__['_container'][0])
    #     # print ("result",result)
    #     # self.assertEqual(result['filePath'],filePath)
    #     self.assertEqual(result['projectID'],'xyz1')
    #     self.assertEqual(result['layerUpdated']['properties'][3]['value'],[50,1])
    #     logging.info("PASSED")
    
    # def testCreateDNNaddDatalayer5(self):
    #     projectID = 'xyz1'

    #     payLoad={"connectionLayerId":"Dense_3","itemType":"LAYER","layerId":"Dense_4","layerIndex":3,"layerType":"Dense","trainable":True,
    #     "name":"Dense","properties":[{"dataType":"string","hint":"Activation function for the dense layer","id":"activationFunction","label":"Activation Function",
    #     "options":["linear","tanh","relu","sigmoid","softmax"],"value":"relu"},{"dataType":"integer","hint":"Neurons for fully connected layer","id":"units","label":"Units","options":[],"value":50},
    #     {"dataType":"array","hint":"only required if dragged first","id":"inputDimension","label":"Input Dimension","options":[],"value":[200,1]},
    #     {"dataType":"array","hint":"Output dimension of the layer","id":"outputDimension","label":"Output Dimension","options":[],"value":[0,1]}],"id":"K49VUNV0SKVXR"}
    #     result = NyokaServer.updatetoArchitecture(payLoad, projectID)
    #     result = json.loads(result.__dict__['_container'][0])
    #     # print ("result",result)
    #     # self.assertEqual(result['filePath'],filePath)
    #     self.assertEqual(result['projectID'],'xyz1')
    #     self.assertEqual(result['layerUpdated']['properties'][3]['value'],[50,1])
    #     logging.info("PASSED")


    # def testCreateDNNaddDatalayer5(self):
    #     projectID = 'xyz1'

    #     payLoad={"connectionLayerId":"Dense_4","itemType":"LAYER","layerId":"Dense_5","layerIndex":4,"layerType":"Dense","trainable":True,"name":"Dense","properties":[{"dataType":"string","hint":"Activation function for the dense layer","id":"activationFunction","label":"Activation Function","options":["linear","tanh","relu","sigmoid","softmax"],"value":"linear"},{"dataType":"integer","hint":"Neurons for fully connected layer","id":"units","label":"Units","options":[],"value":1},{"dataType":"array","hint":"only required if dragged first","id":"inputDimension","label":"Input Dimension","options":[],"value":[50,1]},{"dataType":"array","hint":"Output dimension of the layer","id":"outputDimension","label":"Output Dimension","options":[],"value":[0,1]}],"id":"K49VXHBWU2Y4D"}
    #     result = NyokaServer.updatetoArchitecture(payLoad, projectID)
    #     result = json.loads(result.__dict__['_container'][0])
    #     # print ("result",result)
    #     # self.assertEqual(result['filePath'],filePath)
    #     self.assertEqual(result['projectID'],'xyz1')
    #     self.assertEqual(result['layerUpdated']['properties'][3]['value'],[1,1])
    #     logging.info("PASSED")

    # def testRemoveFiles(self):
    #     filePathToWrite = os.path.abspath('testUseCase/supportData2/sampNNMOdel.pmml')
        
    #     os.remove(filePathToWrite)


    # def testtrainDNN(self):
    #     projectID = 'xyz1'

    #     payLoad={"batchSize":15,"epoch":100,"stepPerEpoch":10,"learningRate":0.001,"loss":"MAE",
    #     "metrics":["MAE"],"optimizer":"Adam","testSize":0.3,"scriptOutput":"NA","problemType":"regression"}
    #     # result = Training().trainNeuralNetworkModels(payLoad, projectID)
    #     nntrainer = mergeTrainingNN.NeuralNetworkModelTrainer()

    #     idforData='12345'
    #     tensorboardLogFolder=''
    #     hyperParaUser=payLoad
    #     pmmlFile=''


	# 	nntrainer.train(idforData,pmmlFile,tensorboardLogFolder,hyperParaUser,pmmlFile)
    #     result = json.loads(result.__dict__['_container'][0])
    #     # print ("result",result)
    #     # self.assertEqual(result['filePath'],filePath)
    #     self.assertEqual(result['status'],'In Progress')
    #     self.assertEqual(result['taskName'],'In Progress')
    #     self.assertEqual(result['type'],'NNProject')
    #     logging.info("PASSED")


# 	@classmethod
# 	def tearDownClass(self):
# 		logging.info("******* Finished Test Cases for Nyoka Server Class *******\n")
