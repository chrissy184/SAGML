from django.shortcuts import render
from django.http import HttpResponse
from django.conf import settings

from rest_framework import permissions
from rest_framework.views import APIView
from rest_framework.response import Response
import requests, json
from django.views.decorators.csrf import csrf_exempt
# from rest_framework.decorators import api_view,renderer_classes,
from django.http import JsonResponse
from django.views.decorators.http import require_POST
from rest_framework.decorators import api_view,schema
import subprocess
from django.utils.encoding import smart_str
import os,ast, signal,re,pathlib
from string import ascii_uppercase
from random import choice
# from trainModel.training import RUNNING_TASK_MEMORY
from trainModel import autoMLutilities
from operator import itemgetter
global RUNNING_TASK_MEMORY

RUNNING_TASK_MEMORY=[]

autoMLutilities = autoMLutilities.AutoMLUtilities()
# from SwaggerSchema.schemas import ( removeTaskSwagger, downloadPMMLSwaager )
# global RUNNING_TASK_MEMORY

class Utility:

	def convertZMKtoZS(inputFile,outputFile=None):
		with open(inputFile,'r') as ff:
			zmkFile = ff.read()

		zmkFile=re.sub(r'architectureName=\"[A-Za-z\s]+\"','architectureName="mobilenet"',zmkFile)
		zmkFile=re.sub(r'max_value=\"[0-9\.]+\"','',zmkFile)
		zmkFile=zmkFile.replace('paddingType','pad')
		zmkFile=re.sub(r'trainable=\"(true|false)\"','',zmkFile)
		zmkFile=re.sub(r'units=\"[0-9]+\"','',zmkFile)

		if not outputFile:
			outputFile=inputFile
		with open(outputFile,'w') as ff:
			ff.write(zmkFile)
		return JsonResponse({'filePath':outputFile},status=201)

	def downloadPMML(filePath):
		fileGot=filePath
		# userInput=json.loads(userInput)
		# fileGot=userInput['filepath']
		# print ('KKKKKKKKKKKKKKKKKKKKK',fileGot)
		file_path=os.path.dirname(fileGot)
		file_name=os.path.basename(fileGot)
		extension=pathlib.Path(fileGot).suffix
		print ('extension >>>>>>',extension)
		content_type = ''
		# extension = file_name.split('.')[-1]
		if extension == '.pmml':
			content_type = 'application/xml'
		elif extension == '.json':
			content_type = 'application/json'
		elif extension == '.csv':
			content_type = 'text/csv'
		elif extension in ['.jpg','.JPG']:
			content_type = 'image/jpeg'
		elif extension in ['.png','.PNG']:
			content_type = 'image/png'
		elif extension in ['.mp4','.MP4']:
			print ('Came Video')
			content_type = 'video/mp4'
		else:
			content_type = 'text/plain'
		print (content_type,' >>>>>>>>>>>>>>>>> content_type')
		with open(fileGot, 'rb') as filedata:
			response = HttpResponse(filedata.read())
			response['Content-type'] = content_type
			response['Content-Disposition'] = 'attachment; filename=%s' % smart_str(file_name)
			response['X-Sendfile'] = smart_str(file_path)
			return response

	def deleteTaskfromMemory(idforData):
		global RUNNING_TASK_MEMORY
		taskId = idforData
		try:
			indexToDelete = -1
			for idx, task in enumerate(RUNNING_TASK_MEMORY):
				if task['idforData'] == taskId:
					try:
						os.kill(task['pid'], signal.SIGTERM)
					except:
						pass
					indexToDelete = idx
					break
			if indexToDelete != -1:
				del RUNNING_TASK_MEMORY[indexToDelete]
				return JsonResponse({'idforData': taskId, 'message':'Deleted successfully'},status=200)
			else:
				return JsonResponse({'idforData': taskId, 'message':'Something went wrong. Please contact Admin'},status=500)
		except:
			return JsonResponse({'idforData': taskId, 'message':'Something went wrong. Please contact Admin'},status=500)


	def runningTaskList(self):
		print ('RUNNING_TASK_MEMORY RUNNING_TASK_MEMORY',RUNNING_TASK_MEMORY)
		for num,tempRS in enumerate(RUNNING_TASK_MEMORY):
			tempStat=RUNNING_TASK_MEMORY[num]
			data_details=autoMLutilities.readStatusFile(tempRS['idforData'])
			try:
				projectName=tempRS['idforData']
				logFolder='logs/'
				data_details['generationInfo']=autoMLutilities.progressOfModel(logFolder,projectName)
				tempStat['generationInfo']=data_details['generationInfo']
				try:
					data_details=autoMLutilities.readStatusFile(tempRS['idforData'])
					if data_details['listOfModelAccuracy'] != []:
						tempStat['generationInfo']=data_details['listOfModelAccuracy']
				except:
					pass
			except:
				pass

			statusOfProject=data_details['status']
			tempStat['status']=statusOfProject

			try:
				print ('ppppppppppp >>>>>>>>',1)
				if tempStat['type']=='NNProject':
					tempStat['url']=data_details['tensorboardUrl']
				print ('ppppppppppp >>>>>>>>',3)
				if data_details['errorMessage']:
					tempStat['errorMessage']=data_details['errorMessage']
				print ('ppppppppppp >>>>>>>>',4)
				if tempStat['errorTraceback']:
					tempStat['errorTraceback']=data_details['errorTraceback']
				print ('ppppppppppp >>>>>>>>',5)
				RUNNING_TASK_MEMORY[num]=tempStat
				print ('RUNNING_TASK_MEMORY hhaha',RUNNING_TASK_MEMORY)
			except:
				pass
		runTaskListSorted=sorted(RUNNING_TASK_MEMORY, key=itemgetter('createdOn'),reverse=True)
		runningTask={'runningTask':runTaskListSorted}
		return JsonResponse(runningTask,status=200)

	def taskUpdateByTaskName(self,taskName):
		self.runningTaskList()
		print ('taskName >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> ',taskName)
		allTaskList=RUNNING_TASK_MEMORY
		filtListofTask=[i for i in allTaskList if i['taskName']==taskName]
		runningTask={'runningTask':filtListofTask}
		return JsonResponse(runningTask,status=200)

	def taskUpdateByTaskNameIdForData(self,taskName,idForData):
		self.runningTaskList()
		print ('taskName >>>>>> >>>>>>>>>>>>>>>>>>>>>>>>>> ',taskName)
		print ('taskName >>>>>> >>>>>>>>>>>>>>>>>>>>>>>>>> ',idForData)
		allTaskList=RUNNING_TASK_MEMORY
		filtListofTask=[i for i in allTaskList if i['taskName']==taskName]
		filtListofTaskId=[i for i in allTaskList if i['idforData']==idForData]
		runningTask={'runningTask':filtListofTaskId}
		return JsonResponse(runningTask,status=200)


		