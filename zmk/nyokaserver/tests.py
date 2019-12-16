# from django.test import TestCase
import unittest
import requests, os, json, sys, logging
from django.http import JsonResponse
from nyokaserver.nyokaServerClass import NyokaServer

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



# 	@classmethod
# 	def tearDownClass(self):
# 		logging.info("******* Finished Test Cases for Nyoka Server Class *******\n")
