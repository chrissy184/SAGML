# from django.test import TestCase
import unittest
import requests, os, json, sys, logging
from django.http import JsonResponse
from scoring.scoringClass import Scoring,NewScoringView
from trainModel.mergeTrainingV2 import NewModelOperations

class TestScoring(unittest.TestCase):

	@classmethod
	def setUpClass(self):
		logging.info("******* Running Test Cases for Scoring Class *******")

	def test_01_listOfLoadedModels(self):
		logging.info("Test Case : Get list of loaded models.")
		result = Scoring.getListOfModelinMemory()
		# print ('result >>>>>>>>>',result.__dict__)
		self.assertEqual(len(json.loads(result.__dict__['_container'][0])), 0)
		logging.info("PASSED")


	def test_02_loadModelForCorrectness(self):
		logging.info("Test Case : Load a model into memory. (1)")
		filePath = os.path.abspath('testUseCase/supportData2/mpgSKModel.pmml')
		# result = Scoring.loadModelfile(filePath)
		result= NewModelOperations().loadExecutionModel(filePath)
		self.assertEqual(result.status_code, 200)
		self.assertEqual(json.loads(result.__dict__['_container'][0])['keytoModel'], 'mpgSKModel')
		self.assertEqual(json.loads(result.__dict__['_container'][0])['message'], 'Model Loaded Successfully')
		result = Scoring.getListOfModelinMemory()
		# print ('>>>>>>','result >>>>>>>>>',result.__dict__['_container'])
		self.assertEqual(len(json.loads(result.__dict__['_container'][0])), 1)
		logging.info("PASSED")


	def test_03_loadModelForError(self):
		logging.info("Test Case : Load a model into memory. (2)")
		filePath = os.path.abspath('testUseCase/supportdata/errorPmml.pmml')
		result= NewModelOperations().loadExecutionModel(filePath)
		self.assertEqual(result.status_code, 500)
		self.assertEqual(json.loads(result.__dict__['_container'][0])['keytoModel'], None)
		self.assertEqual(json.loads(result.__dict__['_container'][0])['message'], 'Model load failed, please connect with admin')
		result = Scoring.getListOfModelinMemory()
		self.assertEqual(len(json.loads(result.__dict__['_container'][0])), 1)
		logging.info("PASSED")



	def test_04_unloadModelForError(self):
		logging.info("Test Case : Remove a loaded model from memory. (1)")
		modelName = 'mpgSKModel1'
		result = Scoring().removeModelfromMemory(modelName)
		self.assertEqual(result.status_code, 500)
		self.assertEqual('message' in json.loads(result.__dict__['_container'][0]), True)
		self.assertEqual(json.loads(result.__dict__['_container'][0])['message'], 'Not able to locate, make sure the model was loaded')
		result = Scoring.getListOfModelinMemory()
		self.assertEqual(len(json.loads(result.__dict__['_container'][0])), 1)
		logging.info("PASSED")


	def test_05_unloadModelForCorrectness(self):
		logging.info("Test Case : Remove a loaded model from memory. (2)")
		modelName = 'mpgSKModel'
		result = Scoring().removeModelfromMemory(modelName)
		self.assertEqual(result.status_code, 200)
		self.assertEqual('message' in json.loads(result.__dict__['_container'][0]), True)
		self.assertEqual(json.loads(result.__dict__['_container'][0])['message'], 'Model unloaded successfully, now it will not be available for predictions.')
		result = Scoring.getListOfModelinMemory()
		self.assertEqual(len(json.loads(result.__dict__['_container'][0])), 0)
		logging.info("PASSED")



	def test_06_scoreCsvDataWithSKL(self):
		logging.info("Test Case : Score csv data with NN model.")
		filePath = os.path.abspath('testUseCase/supportData2/mpgSKModel.pmml')
		result= NewModelOperations().loadExecutionModel(filePath)
		modelName = 'mpgSKModel'
		filePath = os.path.abspath('testUseCase/supportData2/mpg_data_exampleTest.csv')
		result = NewScoringView().wrapperForNewLogic(modelName,None,filePath)
		result = json.loads(result.__dict__['_container'][0])
		self.assertEqual('result' in result, True)
		self.assertEqual(result['result'].endswith('.csv'), True)
		logging.info("PASSED")



	def test_07_scoreCsvDataWithNNwithPrepro(self):
		logging.info("Test Case : Score csv data with SKL model.")
		filePath = os.path.abspath('testUseCase/supportData2/nnMPGMOdel10.pmml')
		result= NewModelOperations().loadExecutionModel(filePath)
		modelName = 'nnMPGMOdel10'
		filePath = os.path.abspath('testUseCase/supportData2/mpgFeatureData.csv')
		result = NewScoringView().wrapperForNewLogic(modelName,None,filePath)
		result = json.loads(result.__dict__['_container'][0])
		print (result)
		self.assertEqual('result' in result, True)
		self.assertEqual(result['result'].endswith('.csv'), True)
		logging.info("PASSED")

	def test_08_scoreCsvDataWithWorkFLow(self):
		logging.info("Test Case : Score csv data with WorkFlow model NN and LR.")
		filePath = os.path.abspath('testUseCase/supportData2/wfModelLRNN.pmml')
		result= NewModelOperations().loadExecutionModel(filePath)
		modelName = 'wfModelLRNN'
		filePath = os.path.abspath('testUseCase/supportData2/mpg_data_exampleTest.csv')
		result = NewScoringView().wrapperForNewLogic(modelName,None,filePath)
		result = json.loads(result.__dict__['_container'][0])
		print (result)
		self.assertEqual('result' in result, True)
		self.assertEqual(result['result'].endswith('.csv'), True)
		logging.info("PASSED")


	# def test_08_scoreJsonDataWithSKL(self):
	# 	logging.info("Test Case : Score json data with SKL model.")
	# 	filePath = os.path.abspath('testUseCase/supportdata/irisSKL.pmml')
	# 	result= NewModelOperations().loadExecutionModel(filePath)
	# 	modelName = 'irisSKL'
	# 	filePath = os.path.abspath('testUseCase/supportdata/iris_test.json')
	# 	result = Scoring.predicttestdata(filePath,modelName)
	# 	result = json.loads(result.__dict__['_container'][0])
	# 	self.assertEqual('result' in result, True)
	# 	self.assertEqual(result['result'].endswith('.txt'), True)
	# 	logging.info("PASSED")



	# def test_09_scoreJsonDataWithNN(self):
	# 	logging.info("Test Case : Score json data with NN model.")
	# 	filePath = os.path.abspath('testUseCase/supportdata/irisNN.pmml')
	# 	result= NewModelOperations().loadExecutionModel(filePath)
	# 	modelName = 'irisNN'
	# 	filePath = os.path.abspath('testUseCase/supportdata/iris_test.json')
	# 	result = Scoring.predicttestdata(filePath,modelName)
	# 	result = json.loads(result.__dict__['_container'][0])
	# 	self.assertEqual('result' in result, True)
	# 	self.assertEqual(result['result'].endswith('.txt'), True)
	# 	logging.info("PASSED")
		

	# def test_10_scoreSignleRecordWithNN(self):
	# 	logging.info("Test Case : Score single record with NN model.")
	# 	filePath = os.path.abspath('testUseCase/supportdata/irisNN.pmml')
	# 	result= NewModelOperations().loadExecutionModel(filePath)

	# 	modelName = 'irisNN'
	# 	data = json.loads('{"sepal_length":4,"sepal_width":5,"petal_length":3,"petal_width":5}')
	# 	result = Scoring.predicttestdata(None,modelName,data)
	# 	result = json.loads(result.__dict__['_container'][0])
	# 	self.assertEqual('result' in result, True)
	# 	self.assertEqual(result['result'].endswith('.txt'), True)
	# 	logging.info("PASSED")


	# def test_11_scoreSignleRecordWithSKL(self):
	# 	logging.info("Test Case : Score single record with SKL model.")
	# 	filePath = os.path.abspath('testUseCase/supportdata/irisSKL.pmml')
	# 	result= NewModelOperations().loadExecutionModel(filePath)

	# 	modelName = 'irisSKL'
	# 	data = json.loads('{"sepal_length":4,"sepal_width":5,"petal_length":3,"petal_width":5}')
	# 	result = Scoring.predicttestdata(None,modelName,data)
	# 	result = json.loads(result.__dict__['_container'][0])
	# 	self.assertEqual('result' in result, True)
	# 	self.assertEqual(result['result'].endswith('.txt'), True)
	# 	logging.info("PASSED")


	@classmethod
	def tearDownClass(self):
		logging.info("******* Finished Test Cases for Scoring Class *******\n")

	
