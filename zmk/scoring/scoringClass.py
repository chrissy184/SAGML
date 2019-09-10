
from django.shortcuts import render
from django.http import HttpResponse
from django.conf import settings

from rest_framework import permissions
from rest_framework.views import APIView
from rest_framework.response import Response
import requests
import json
from django.views.decorators.csrf import csrf_exempt
# from rest_framework.decorators import api_view,renderer_classes,
from django.http import JsonResponse
from django.views.decorators.http import require_POST

from rest_framework.decorators import api_view,schema
import os,subprocess,pathlib

from string import ascii_uppercase
from random import choice
import pandas as pd
import skimage

from multiprocessing import Lock, Process
lockForModelLoad = None


def create_lockForModel():
    global lockForModelLoad
    lockForModelLoad = Lock()

# from SwaggerSchema.schemas import (loadModelSwagger,
# 									predictTestDataSwagger,
# 									unloadModelSwagger,
#                                     )
from trainModel import kerasUtilities
from trainModel.kerasUtilities import PMMLMODELSTORAGE
kerasUtilities = kerasUtilities.KerasUtilities()


# global PMMLMODELSTORAGE



class Scoring:
    
	def getListOfModelinMemory():
		global PMMLMODELSTORAGE
		moreDetails=[]
		for j in PMMLMODELSTORAGE:
			temp_dict={}
			temp_dict['modelName']=j
			try:
				temp_dict['inputShape']=PMMLMODELSTORAGE[j]['inputShape']
			except:
				pass
			# temp_dict['predClasses']=[str(cl) for cl in PMMLMODELSTORAGE[j]['predClasses']]
			try:
				temp_dict['status']=PMMLMODELSTORAGE[j]['status']
			except:
				pass
			moreDetails.append(temp_dict)
			# print ('>>>',temp_dict)
		return JsonResponse(moreDetails, safe=False,status=200)


	def loadModelfile(self,filpath, idforData=None):
		# print ('>>>>>',filpath)
		global PMMLMODELSTORAGE
		# filpath=requests.POST.get('filePath')
		# print ('>>>>>>> filepath',filpath)
		# filpath=filpath.replace('.pmml','')
		keyOfGlobalMemory,messNotice,modelType=kerasUtilities.loadPMMLmodel(filpath,idforData)
		# print ('>>>>>>> messnotice',messNotice)
		if messNotice=='Success':
			# modelDetails={j:PMMLMODELSTORAGE[j] for j in PMMLMODELSTORAGE}		
			# print ('>>>>>>>>>>>>>>>>>>>>>',PMMLMODELSTORAGE )
			# modelDetails={'inputShape':modelDetails[keyOfGlobalMemory]['inputShape'],
			# 			   'predClasses':modelDetails[keyOfGlobalMemory]['predClasses'],
			# 			   'status':modelDetails[keyOfGlobalMemory]['status'],}
			data_details={'message':'Model loaded successfully','keytoModel':keyOfGlobalMemory}#,'modelDetails':modelDetails}
			statusCode = 200
			# print('PMMLMODELSTORAGE',PMMLMODELSTORAGE)
			# return JsonResponse(data_details)
		# elif (messNotice=='Success') & (modelType=='sklearnM'):
		# 	# print ('>>>>>')
		# 	# modelDetails={j:PMMLMODELSTORAGE[j] for j in PMMLMODELSTORAGE}
		# 	data_details={'message':'Model loaded successfully','keytoModel':keyOfGlobalMemory}
		elif messNotice=='Failure':
			data_details={'message':'Model loading failed, please contact Admin','keytoModel':None}
			statusCode = 500
		return JsonResponse(data_details,status= statusCode)


	def removeModelfromMemory(self,modelName):
		# print('>>>>>>>>>>>>>>>>came here')
		global PMMLMODELSTORAGE
		# modelname=param
		modelName=modelName.replace('.pmml','')
		# print('modelname ',modelname)
		try:
			messNotice=kerasUtilities.deleteLoadedModelfromMemory(modelName)
			data_details={'message':'Model unloaded successfully, now it will not be available for predictions.'}
			statusCode = 200
		except:
			data_details={'message':'Not able to locate, make sure the model was loaded'}
			statusCode = 500
		# print(data_details)
		return JsonResponse(data_details,status= statusCode)

	def predicttestdata(self,filpath,modelName,jsonData=None):
		# print('Came Step 1')

		def checkValInPMMLSTO(pmmlstorage,valtoCheck):
			try:
				val=pmmlstorage[valtoCheck]
			except:
				val=None
			return val

		def checkExtensionOfFile(fP):
			return pathlib.Path(fP).suffix


		global PMMLMODELSTORAGE
		pmmlstoragepointer=modelName
		# print ('>>>>',pmmlstoragepointer)
		# print('.,.,.,.',PMMLMODELSTORAGE)
		# print('filepath>>>>>>>>>>>>>>>',filpath)
		pmmlstoragepointer=pmmlstoragepointer.replace('.pmml','')
		pmmlObj=PMMLMODELSTORAGE[pmmlstoragepointer]
		
		modelType=checkValInPMMLSTO(pmmlObj,'modelType')
		preProcessScript=checkValInPMMLSTO(pmmlObj,'preProcessScript')
		postProcessScript=checkValInPMMLSTO(pmmlObj,'postProcessScript')
		scriptOutput=checkValInPMMLSTO(pmmlObj,'scriptOutput')

		# print('Came Step 2',modelType,scriptOutput)
		# print ('preProcessScript',preProcessScript,'postProcessScript',postProcessScript)

		
		if filpath and (modelType != 'MRCNN'):
			extenFile=checkExtensionOfFile(filpath)
			PMMLMODELSTORAGE[pmmlstoragepointer]['extenFile']=extenFile
			import pandas as pd
			if (preProcessScript == None) & (postProcessScript == None):
				if extenFile in ['.jpg','.JPG','.png','.PNG']:
					outputModel=kerasUtilities.predictImagedata(pmmlstoragepointer,filpath)
					resulFile=outputModel
				elif os.path.isdir(filpath):
					numFiles=os.listdir(filpath+'/test')
					if len(numFiles) > 100:
						tempRunMemory=kerasUtilities.predictFolderDataInBatch(pmmlstoragepointer,filpath,len(numFiles))
						tempRunMemory['inTask']=True
						return JsonResponse(tempRunMemory,status=200)
					else:
						resulFile=kerasUtilities.predictFolderdata(pmmlstoragepointer,filpath)
				elif extenFile in ['.json']:
					data=json.load(open(filpath,'r'))
					testData=pd.DataFrame([data])
					resulFile=kerasUtilities.predictFiledata(pmmlstoragepointer,testData,modelType)
				else:
					testData=pd.read_csv(filpath)
					resulFile=kerasUtilities.predictFiledata(pmmlstoragepointer,testData,modelType)

			elif (preProcessScript != None) & (postProcessScript == None):
				if scriptOutput in ['IMAGE','DATA']:
					if modelType=='kerasM':
						# print ('>>>>>>>>>>>>>>>>',scriptOutput)
						resulFile=kerasUtilities.predictCustomCodedata(pmmlstoragepointer,filpath,scriptOutput)
						if resulFile.__class__.__name__ == 'dict':
							resulFile['inTask']=True
							return JsonResponse(resulFile,status=200)
				else:
					pass

			elif (preProcessScript != None) & (postProcessScript != None):
				# print('Came Step 3')
				if scriptOutput in ['IMAGE','DATA']:
					if modelType=='kerasM':
						# print ('>>>>>>>>>>>>>>>>>',scriptOutput)
						resulFile=kerasUtilities.predictDataWithPostScript(pmmlstoragepointer,filpath,scriptOutput)
						if resulFile.__class__.__name__ == 'dict':
							resulFile['inTask']=True
							return JsonResponse(resulFile,status=200)
			elif (preProcessScript == None) & (postProcessScript != None):
				# print('Came Step 4')
				# if scriptOutput in ['IMAGE','DATA']:
				if modelType=='kerasM':
					print ('>>>>>>>>>>>>>>>>>',scriptOutput)
					resulFile=kerasUtilities.predictDataWithOnlyPostScript(pmmlstoragepointer,filpath,extenFile)

		elif filpath and (modelType == 'MRCNN'):
			# print ('Came to MRCNN model >>>>>>')
			extenFile=checkExtensionOfFile(filpath)
			if extenFile in ['.jpg','.JPG','.png','.PNG']:
				resulFile=kerasUtilities.detectObject(filpath, modelName)


		else:
			import pandas as pd
			testData=pd.DataFrame([jsonData])
			PMMLMODELSTORAGE[pmmlstoragepointer]['extenFile']='.json'
			resulFile=kerasUtilities.predictFiledata(pmmlstoragepointer,testData,modelType)

		data_details={'result':resulFile}
		return JsonResponse(data_details,status=202)

	def predicttestdataReturnJson(self,filpath,modelName,jsonData=None):
		# print('Came Step 1')

		def checkValInPMMLSTO(pmmlstorage,valtoCheck):
			try:
				val=pmmlstorage[valtoCheck]
			except:
				val=None
			return val

		def checkExtensionOfFile(fP):
			return pathlib.Path(fP).suffix


		global PMMLMODELSTORAGE
		pmmlstoragepointer=modelName
		# print ('>>>>',pmmlstoragepointer)
		# print('.,.,.,.',PMMLMODELSTORAGE)
		# print('filepath>>>>>>>>>>>>>>>',filpath)
		pmmlstoragepointer=pmmlstoragepointer.replace('.pmml','')
		pmmlObj=PMMLMODELSTORAGE[pmmlstoragepointer]
		
		modelType=checkValInPMMLSTO(pmmlObj,'modelType')
		preProcessScript=checkValInPMMLSTO(pmmlObj,'preProcessScript')
		postProcessScript=checkValInPMMLSTO(pmmlObj,'postProcessScript')
		scriptOutput=checkValInPMMLSTO(pmmlObj,'scriptOutput')

		# print('Came Step 2',modelType,scriptOutput)
		# print ('preProcessScript',preProcessScript,'postProcessScript',postProcessScript)

		
		if filpath and (modelType != 'MRCNN'):
			extenFile=checkExtensionOfFile(filpath)
			PMMLMODELSTORAGE[pmmlstoragepointer]['extenFile']=extenFile
			import pandas as pd
			if (preProcessScript == None) & (postProcessScript == None):
				if extenFile in ['.jpg','.JPG','.png','.PNG']:
					outputModel=kerasUtilities.predictImagedata(pmmlstoragepointer,filpath)
					resulFile=outputModel
				elif os.path.isdir(filpath):
					numFiles=os.listdir(filpath+'/test')
					if len(numFiles) > 100:
						tempRunMemory=kerasUtilities.predictFolderDataInBatch(pmmlstoragepointer,filpath,len(numFiles))
						tempRunMemory['inTask']=True
						return JsonResponse(tempRunMemory,status=200)
					else:
						resulFile=kerasUtilities.predictFolderdata(pmmlstoragepointer,filpath)
				elif extenFile in ['.json']:
					data=json.load(open(filpath,'r'))
					testData=pd.DataFrame([data])
					resulFile=kerasUtilities.predictFiledata(pmmlstoragepointer,testData,modelType)
				else:
					testData=pd.read_csv(filpath)
					resulFile=kerasUtilities.predictFiledata(pmmlstoragepointer,testData,modelType)

			elif (preProcessScript != None) & (postProcessScript == None):
				if scriptOutput in ['IMAGE','DATA']:
					if modelType=='kerasM':
						# print ('>>>>>>>>>>>>>>>>',scriptOutput)
						resulFile=kerasUtilities.predictCustomCodedata(pmmlstoragepointer,filpath,scriptOutput)
						if resulFile.__class__.__name__ == 'dict':
							resulFile['inTask']=True
							return JsonResponse(resulFile,status=200)
				else:
					pass

			elif (preProcessScript != None) & (postProcessScript != None):
				# print('Came Step 3')
				if scriptOutput in ['IMAGE','DATA']:
					if modelType=='kerasM':
						# print ('>>>>>>>>>>>>>>>>>',scriptOutput)
						resulFile=kerasUtilities.predictDataWithPostScript(pmmlstoragepointer,filpath,scriptOutput)
						if resulFile.__class__.__name__ == 'dict':
							resulFile['inTask']=True
							return JsonResponse(resulFile,status=200)
			elif (preProcessScript == None) & (postProcessScript != None):
				# print('Came Step 4')
				# if scriptOutput in ['IMAGE','DATA']:
				if modelType=='kerasM':
					print ('>>>>>>>>>>>>>>>>>',scriptOutput)
					resulFile=kerasUtilities.predictDataWithOnlyPostScript(pmmlstoragepointer,filpath,extenFile)

		elif filpath and (modelType == 'MRCNN'):
			# print ('Came to MRCNN model >>>>>>')
			extenFile=checkExtensionOfFile(filpath)
			if extenFile in ['.jpg','.JPG','.png','.PNG']:
				resulFile=kerasUtilities.detectObject(filpath, modelName)


		else:
			import pandas as pd
			testData=pd.DataFrame([jsonData])
			PMMLMODELSTORAGE[pmmlstoragepointer]['extenFile']='.json'
			resulFile=kerasUtilities.predictFiledataReturnJson(pmmlstoragepointer,testData,modelType)

		data_details={'result':resulFile}
		print (data_details)
		return JsonResponse(data_details,status=202)

	def predictTestDataWithModification(self,filpath,modelName,jsonData=None):
		# print ('ModelName',modelName)
		global PMMLMODELSTORAGE
		# print (list(PMMLMODELSTORAGE.keys()))
		if modelName in PMMLMODELSTORAGE:
			print ('Came here model found')
			scoredOutput=self.predicttestdata(filpath,modelName,jsonData)
			return scoredOutput
		else:
			lockForModelLoad.acquire()
			if modelName in PMMLMODELSTORAGE:
				# print ('Came here model found 1.1')
				scoredOutput=self.predicttestdata(filpath,modelName,jsonData)
			else:
				# print ('Came here model Not found')
				modelNameFilePath='../ZMOD/Models/'+modelName+'.pmml'

				self.loadModelfile(modelNameFilePath, idforData=None)
				scoredOutput=self.predicttestdata(filpath,modelName,jsonData)
			lockForModelLoad.release()
			return scoredOutput
		
	def predictTestDataWithModificationReturnJson(self,filpath,modelName,jsonData=None):
		# print ('ModelName',modelName)
		global PMMLMODELSTORAGE
		# print (list(PMMLMODELSTORAGE.keys()))
		if modelName in PMMLMODELSTORAGE:
			# print ('Came here model found')
			scoredOutput=self.predicttestdataReturnJson(filpath,modelName,jsonData)
			return scoredOutput
		else:
			lockForModelLoad.acquire()
			if modelName in PMMLMODELSTORAGE:
				# print ('Came here model found 1.1')
				scoredOutput=self.predicttestdataReturnJson(filpath,modelName,jsonData)
			else:
				# print ('Came here model Not found')
				modelNameFilePath='../ZMOD/Models/'+modelName+'.pmml'

				self.loadModelfile(modelNameFilePath, idforData=None)
				scoredOutput=self.predicttestdataReturnJson(filpath,modelName,jsonData)
			lockForModelLoad.release()
			return scoredOutput
			