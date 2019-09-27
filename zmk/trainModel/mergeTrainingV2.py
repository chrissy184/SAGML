import pandas  as pd
from django.http import JsonResponse
import numpy as np
from sklearn import model_selection,preprocessing
from keras.preprocessing.image import ImageDataGenerator
from datetime import datetime
import matplotlib.pyplot as plt
from datetime import datetime
from keras import optimizers
import sys, os,ast,json,copy,traceback,pathlib
from keras import backend as K
import keras
from multiprocessing import Process
from trainModel import kerasUtilities
kerasUtilities = kerasUtilities.KerasUtilities()
from multiprocessing import Lock, Process
from nyokaBase import PMML43Ext as ny
from tensorflow import Graph, Session
import tensorflow as tf

global PMMLMODELSTORAGE

PMMLMODELSTORAGE={}


modelObjectToCheck=['AssociationModel', 'AnomalyDetectionModel', 'BayesianNetworkModel',
                    'BaselineModel', 'ClusteringModel', 'DeepNetwork', 'GaussianProcessModel',
                    'GeneralRegressionModel', 'MiningModel', 'NaiveBayesModel', 'NearestNeighborModel', 
                    'NeuralNetwork', 'RegressionModel', 'RuleSetModel', 'SequenceModel', 'Scorecard', 
                    'SupportVectorMachineModel', 'TextModel', 'TimeSeriesModel', 'TreeModel']

selDev="/device:CPU:0"
def gpuCPUSelect(selDev):
	return selDev

class NewModelOperations:

    def getCode(self,strngVal):
        lines = []
        code = strngVal.lstrip('\n')
        leading_spaces = len(code) - len(code.lstrip(' '))
        for line in code.split('\n'):
            lines.append(line[leading_spaces:])
        code = '\n'.join(lines)
        return code

    def getCodeObjectToProcess(self,codeVal):
        d = {}
        exec(codeVal, None,d)
        objeCode=d[list(d.keys())[0]]
        return objeCode

    def getTargetAndColumnsName(self,modObjToDetect):
        targetCol=None
        listOFColumns=[]
        if modObjToDetect.__dict__['original_tagname_'] in ['MiningModel','DeepNetwork']:
            for minF in modObjToDetect.get_MiningSchema().__dict__['MiningField']:
                if minF.__dict__['usageType'] == 'target':
                    targetCol=minF.__dict__['name']
                else:
                    listOFColumns.append(minF.__dict__['name'])
        else:
            print ('>>>>>>>>>>>>>>>>>>>>>>>>','Add support')
            targetCol=None
            listOFColumns=[]
        return listOFColumns,targetCol

    def nyObjOfModel(self,pmmlObj,singMod):
        import nyokaBase.PMML43Ext as ny
        if singMod['pmmlModelObject'].__dict__['original_tagname_']=='MiningModel':
            nyokaObj=ny.PMML(MiningBuildTask=pmmlObj.MiningBuildTask,DataDictionary=pmmlObj.DataDictionary,MiningModel=[singMod['pmmlModelObject']])
        elif singMod['pmmlModelObject'].__dict__['original_tagname_']=='DeepNetwork':
            nyokaObj=ny.PMML(DataDictionary=pmmlObj.DataDictionary,DeepNetwork=[singMod['pmmlModelObject']])
        else:
            nyokaObj=None
        return nyokaObj

    def loadExecutionModel(self,pmmlFile):
        print ('loadmodel started')
        global PMMLMODELSTORAGE
        pmmlFileObj=pathlib.Path(pmmlFile)
        pmmlFileForKey=pmmlFileObj.name.replace(pmmlFileObj.suffix,'')
        from nyokaBase import PMML43Ext as ny
        pmmlObj=ny.parse(pmmlFile,silence=True)

        modelObj=[]
        for inMod in modelObjectToCheck:
            if len(pmmlObj.__dict__[inMod]) >0:
                modPMMLObj=pmmlObj.__dict__[inMod]
                if inMod == 'DeepNetwork':
                    for ininMod in modPMMLObj:
                        colInfo=self.getTargetAndColumnsName(ininMod)
                        modelObj.append({'modelArchType':'NNModel','pmmlModelObject':ininMod,'recoModelObj':None,'listOFColumns':None,'targetCol':colInfo[1]})
                else:
                    for ininMod in modPMMLObj:
                        colInfo=self.getTargetAndColumnsName(ininMod)
        #                 recoModelObj=generateModelfromPMML(ininMod)
                        modelObj.append({'modelArchType':'SKLModel','pmmlModelObject':ininMod,'recoModelObj':None,'listOFColumns':colInfo[0],'targetCol':colInfo[1]})
        
        
        tempDict={}
        tempDict['train']={}

        tempDict['score']={}

        for singMod in modelObj:
            if singMod['pmmlModelObject'].taskType=='trainAndscore':
                tempDict['train'][singMod['pmmlModelObject'].modelName]={}
                tempDict['train'][singMod['pmmlModelObject'].modelName]['modelObj']=singMod
                tempDict['train'][singMod['pmmlModelObject'].modelName]['modelObj']['pmmlDdicObj']=pmmlObj.DataDictionary
                tempDict['train'][singMod['pmmlModelObject'].modelName]['modelObj']['pmmlNyokaObj']=self.nyObjOfModel(pmmlObj,singMod)
                tempDict['score'][singMod['pmmlModelObject'].modelName]={}
                tempDict['score'][singMod['pmmlModelObject'].modelName]['modelObj']=singMod
                tempDict['score'][singMod['pmmlModelObject'].modelName]['modelObj']['pmmlDdicObj']=pmmlObj.DataDictionary
                tempDict['train'][singMod['pmmlModelObject'].modelName]['modelObj']['pmmlNyokaObj']=self.nyObjOfModel(pmmlObj,singMod)
            else:
                tempDict[singMod['pmmlModelObject'].taskType][singMod['pmmlModelObject'].modelName]={}
                tempDict[singMod['pmmlModelObject'].taskType][singMod['pmmlModelObject'].modelName]['modelObj']=singMod
                tempDict[singMod['pmmlModelObject'].taskType][singMod['pmmlModelObject'].modelName]['modelObj']['pmmlDdicObj']=pmmlObj.DataDictionary
                tempDict[singMod['pmmlModelObject'].taskType][singMod['pmmlModelObject'].modelName]['modelObj']['pmmlNyokaObj']=self.nyObjOfModel(pmmlObj,singMod)
        
        tempDict2={}
        for taType in tempDict:
            tempTa=list(tempDict[taType].keys())
            tempTa.sort()
 
            for taTTemp in tempTa:
                if taType in tempDict2:
                    pass
                else:
                    tempDict2[taType]={}
                tempDict2[taType][taTTemp]=tempDict[taType][taTTemp]

        tempDict=tempDict2.copy()

        for sc1 in pmmlObj.script:
            if sc1.scriptPurpose=='trainAndscore':
                tempDict['train'][sc1.for_][sc1.class_]={}
                tempDict['train'][sc1.for_][sc1.class_+'_code']=self.getCode(sc1.valueOf_)
                tempDict['train'][sc1.for_][sc1.class_]=self.getCodeObjectToProcess(self.getCode(sc1.valueOf_))
                tempDict['train'][sc1.for_]['scriptOutput']=sc1.scriptOutput
                tempDict['score'][sc1.for_][sc1.class_]={}
                tempDict['score'][sc1.for_][sc1.class_+'_code']=self.getCode(sc1.valueOf_)
                tempDict['score'][sc1.for_][sc1.class_]=self.getCodeObjectToProcess(self.getCode(sc1.valueOf_))
                tempDict['score'][sc1.for_]['scriptOutput']=sc1.scriptOutput
            else:
                tempDict[sc1.scriptPurpose][sc1.for_][sc1.class_]={}
                tempDict[sc1.scriptPurpose][sc1.for_][sc1.class_+'_code']=self.getCode(sc1.valueOf_)
                tempDict[sc1.scriptPurpose][sc1.for_][sc1.class_]=self.getCodeObjectToProcess(self.getCode(sc1.valueOf_))
                tempDict[sc1.scriptPurpose][sc1.for_]['scriptOutput']=sc1.scriptOutput


        taskTypesName=list(tempDict.keys())
        listOfModelNames=set([k for j in tempDict for k in tempDict[j]])

        hyperParDict={}
        for extObj in pmmlObj.MiningBuildTask.Extension:
            if extObj.name=='hyperparameters':
                hyperParDict[extObj.for_]=ast.literal_eval(extObj.value)

        try:
            miningBuildTaskList=pmmlObj.MiningBuildTask.__dict__['Extension']
            for bTask in miningBuildTaskList:
                if bTask.__dict__['for_'] in listOfModelNames:
                    for tT in taskTypesName:
                        for modInd in listOfModelNames:
                            tempDict[tT][modInd]['modelObj']['miningExtension']=bTask
        except:
            pass

        for dO in pmmlObj.Data:
            for tT in taskTypesName:
                for modInd in listOfModelNames:
                    print (tT,modInd)
                    if dO.for_ == modInd:
                        try:
                            tempDict[tT][modInd]['Data']=dO.filePath
                        except:
                            pass

        modelLoadStatus=[]

        for taskT in tempDict:
            print (taskT)
            for mO in tempDict[taskT]:
                if tempDict[taskT][mO]['modelObj']['modelArchType']=="NNModel":
                    modelProp=tempDict[taskT][mO]['modelObj']['pmmlNyokaObj']

                    model_graph = Graph()
                    with model_graph.as_default():
                        tf_session = Session()
                        with tf_session.as_default():
                            print ('step 5')
                            from nyokaBase.reconstruct.pmml_to_pipeline_model import generate_skl_model
                            print ('step 5.1')
                            model_net = generate_skl_model(modelProp)
                            print ('step 5.2')
                            model = model_net.model
                            model_graph = tf.get_default_graph()
                            print ('step 6')
                    inputShapevals=[inpuShape.value for inpuShape in list(model.input.shape)]
                    if str(model_net) != 'None':
                        tempDict[taskT][mO]['modelObj']['recoModelObj']=model_net
                        tempDict[taskT][mO]['modelObj']['model_graph']=model_graph
                        tempDict[taskT][mO]['modelObj']['tf_session']=tf_session
                        tempDict[taskT][mO]['modelObj']['inputShape']=inputShapevals
                        modelLoadStatus.append(1)
                    else:
                        modelLoadStatus.append(0)
                    try:
                        tempDict[taskT][mO]['modelObj']['hyperparameters']=hyperParDict[mO]
                    except:
                        tempDict[taskT][mO]['modelObj']['hyperparameters']=None
                elif tempDict[taskT][mO]['modelObj']['modelArchType']=="SKLModel":
                    modelProp=tempDict[taskT][mO]['modelObj']['pmmlNyokaObj']
                    from nyokaBase.reconstruct.pmml_to_pipeline_model import generate_skl_model
                    recoModelObj=generate_skl_model(modelProp)
                    if recoModelObj != None:
                        tempDict[taskT][mO]['modelObj']['recoModelObj']=recoModelObj
                        modelLoadStatus.append(1)
                    else:
                        modelLoadStatus.append(0)
                    try:
                        tempDict[taskT][mO]['modelObj']['hyperparameters']=hyperParDict[mO]
                    except:
                        tempDict[taskT][mO]['modelObj']['hyperparameters']=None

        # print('*'*100)

        # print(tempDict['score']['model2'])
        # print('*'*100)
        
        PMMLMODELSTORAGE[pmmlFileForKey]=tempDict
        if 0 in modelLoadStatus:
            messageToWorld= "Model load failed, please connect with admin"
        else:
            messageToWorld= "Model Loaded Successfully"

        resultResp={'message':messageToWorld,'keytoModel':pmmlFileForKey}
        return JsonResponse(resultResp,status=200)


class NewTrainingView:

    global PMMLMODELSTORAGE

    def upDateStatus(self,statusFile):
        self.lockForStatus.acquire()
        sFile=open(statusFile,'r')
        sFileText=sFile.read()
        print (sFileText)
        data_details=json.loads(sFileText)
        sFile.close()
        self.lockForStatus.release()
        return data_details

    def trainAllModel(self,modelName):

        idforData=''.join(choice(ascii_uppercase) for i in range(12))
        saveStatus=logFolder+idforData+'/'
        kerasUtilities.checkCreatePath(saveStatus)
        statusfileLocation=saveStatus+'status.txt'

        pID=0

        tempRunMemory={'idforData': idforData,
			'status': 'Execution Failed' if pID==-1 else 'In Progress',
			'createdOn': str(datetime.datetime.now()),
			'type': 'NNModel',
			'pid':pID
            }
        tempRunMemory['taskName']=modelName
        RUNNING_TASK_MEMORY.append(tempRunMemory)
        with open(statusfileLocation,'w') as filetosave:
            json.dump(tempRunMemory, filetosave)
        import time
        time.sleep(5)
        
        trainVieClassObj=TrainingViewModels()
        train_prc = Thread(target=trainVieClassObj.trainModel,args=(modelName,statusfileLocation,))
        train_prc.start()
        pID = train_prc.ident
        tempRunMemory={'idforData': idforData,
			'status': 'Execution Failed' if pID==-1 else 'In Progress',
			'createdOn': str(datetime.datetime.now()),
			'type': 'NNModel',
			'pid':pID
            }
        tempRunMemory['taskName']=modelName
        RUNNING_TASK_MEMORY.append(tempRunMemory)
        with open(statusfileLocation,'w') as filetosave:
            json.dump(tempRunMemory, filetosave)

        
        return JsonResponse(tempRunMemory)



class TrainingViewModels:

    def __init__(self):
        self.statusFile = None
        self.logFolder = 'logs/'
        self.lockForStatus = Lock()

    def upDateStatus(self):
        self.lockForStatus.acquire()
        sFile=open(self.statusFile,'r')
        sFileText=sFile.read()
        data_details=json.loads(sFileText)
        sFile.close()
        self.lockForStatus.release()
        return data_details

    def updateStatusWithError(self,data_details,statusOFExe,errorMessage,errorTraceback,statusFile):
        data_details=self.upDateStatus()
        data_details['status']=statusOFExe
        data_details['errorMessage']=errorMessage
        data_details['errorTraceback']=errorTraceback
        with open(statusFile,'w') as filetosave:
            json.dump(data_details, filetosave)
        return ('done')

    def increName(self,nameList):
        import os
        nameList=nameList.split('_')
        if 'V' in nameList[-1]:
            nameExist=True
            while nameExist:
                toInc=int(nameList[-1][1:])+1
                nameList[-1]='V'+str(toInc)
                fName='_'.join(nameList)
                if os.path.exists('../ZMOD/Models/'+fName+'.pmml'):
                    nameExist=True
        else:
            fName='_'.join(nameList)+'_V'+str(1)
            nameList=fName.split('_')
            print ('Step 1 >>>>',fName,nameList)
            nameExist=os.path.exists('../ZMOD/Models/'+fName+'.pmml')
            while nameExist:
                toInc=int(nameList[-1][1:])+1
                nameList[-1]='V'+str(toInc)
                fName='_'.join(nameList)
                if os.path.exists('../ZMOD/Models/'+fName+'.pmml'):
                    nameExist=True
                else:
                    nameExist=False
        return fName


    def generateAndCompileModel(self, lossType, optimizerName, learningRate, listOfMetrics, compileTestOnly=False):

		def f1(y_true, y_pred):
			def recall(y_true, y_pred):
			    true_positives = K.sum(K.round(K.clip(y_true * y_pred, 0, 1)))
			    possible_positives = K.sum(K.round(K.clip(y_true, 0, 1)))
			    recall = true_positives / (possible_positives + K.epsilon())
			    return recall

			def precision(y_true, y_pred):
			    true_positives = K.sum(K.round(K.clip(y_true * y_pred, 0, 1)))
			    predicted_positives = K.sum(K.round(K.clip(y_pred, 0, 1)))
			    precision = true_positives / (predicted_positives + K.epsilon())
			    return precision
			precision = precision(y_true, y_pred)
			recall = recall(y_true, y_pred)
			return 2*((precision*recall)/(precision+recall+K.epsilon()))
		try:
			optiMi=self.setOptimizer(optimizerName,learningRate)
		except Exception as e:
			if not compileTestOnly:
				data_details=self.upDateStatus()
				self.updateStatusWithError(data_details,'Training Failed','Unable to get the optimizer '+optimizerName+' >> '+ str(e),traceback.format_exc(),self.statusFile)
			else:
				data_details = {}
				self.updateStatusWithError(data_details,'Model Compilation Failed','Error while compiling the Model >> '+ str(e),traceback.format_exc(),self.statusFile)
			return data_details

		model=modelObj.model

		try:
		    if 'f1' in listOfMetrics:
		        listOfMetrics.remove('f1')
		        model.compile(optimizer=optiMi, loss=lossType, metrics=listOfMetrics+[f1])
		    else:
		        model.compile(optimizer=optiMi, loss=lossType, metrics=listOfMetrics)
		except Exception as e:
			if not compileTestOnly:
				data_details=self.upDateStatus()
				self.updateStatusWithError(data_details,'Training Failed','Error while compiling Model >> '+ str(e),traceback.format_exc(),self.statusFile)
			else:
				data_details = {}
				self.updateStatusWithError(data_details,'Model Compilation Failed','Error while compiling Model >> '+ str(e),traceback.format_exc(),self.statusFile)
			return data_details
		if not compileTestOnly:
			kerasUtilities.updateStatusOfTraining(self.statusFile,'Model Successfully compiled')
		modelObj.model = model
		return modelObj
        
    def verifyHyperparameters(self,datHyperPara):
        try:
            lossType=datHyperPara['lossType']
        except Exception as e:
            self.pathOfData=None
            data_details=self.upDateStatus()
            self.updateStatusWithError(data_details,'Training Failed',"Couldn't find hyperparameters lossType >> "+ str(e),traceback.format_exc(),self.statusFile)
            return -1
        try:
            listOfMetrics=datHyperPara['listOfMetrics']
        except Exception as e:
            self.pathOfData=None
            data_details=self.upDateStatus()
            self.updateStatusWithError(data_details,'Training Failed',"Couldn't find hyperparameters listOfMetrics >> "+ str(e),traceback.format_exc(),self.statusFile)
            return -1

        try:
            batchSize=datHyperPara['batchSize']
        except Exception as e:
            self.pathOfData=None
            data_details=self.upDateStatus()
            self.updateStatusWithError(data_details,'Training Failed',"Couldn't find hyperparameters batchSize >> "+ str(e),traceback.format_exc(),self.statusFile)
            return -1

        try:
            epoch=datHyperPara['epoch']
        except Exception as e:
            self.pathOfData=None
            data_details=self.upDateStatus()
            self.updateStatusWithError(data_details,'Training Failed',"Couldn't find hyperparameters epoch >> "+ str(e),traceback.format_exc(),self.statusFile)
            return -1

        try:
            problemType=datHyperPara['problemType']
        except Exception as e:
            self.pathOfData=None
            data_details=self.upDateStatus()
            self.updateStatusWithError(data_details,'Training Failed',"Couldn't find hyperparameters problemType >> "+ str(e),traceback.format_exc(),self.statusFile)
            return -1

        try:
            optimizerName=datHyperPara['optimizerName']
        except Exception as e:
            self.pathOfData=None
            data_details=self.upDateStatus()
            self.updateStatusWithError(data_details,'Training Failed',"Couldn't find hyperparameters optimizerName >> "+ str(e),traceback.format_exc(),self.statusFile)
            return -1

        try:
            learningRate=datHyperPara['learningRate']
        except Exception as e:
            self.pathOfData=None
            data_details=self.upDateStatus()
            self.updateStatusWithError(data_details,'Training Failed',"Couldn't find hyperparameters learningRate >> "+ str(e),traceback.format_exc(),self.statusFile)
            return -1

        try:
            testSize=datHyperPara['testSize']
        except Exception as e:
            testSize=None
        return 'done'
    def trainModelObjectDict(self,modelObj,idforData,tensorboardLogFolder):
        dataObj=modelObj['Data']
        modelArch=modelObj['modelObj']['modelArchType']
        try:
            scriptOutputPrepro=modelObj['scriptOutput']
        except:
            scriptOutputPrepro=None

        datHyperPara=modelObj['hyperparameters']
        checkVal=self.verifyHyperparameters(modelObj['hyperparameters'])
        if checkVal == 'done':
            pass
        else:
            data_details=self.upDateStatus()
            self.updateStatusWithError(data_details,'Training Failed',"Some issue with hyperparameters >> ",'No info',self.statusFile)
            return -1
		
        if dataObj == None:
            pass
        elif (os.path.isdir(dataObj)) & (scriptOutputPrepro==None):
            modelObjTrained=self.trainImageClassifierNN(modelObj)
            pass
        elif (pathlib.Path(dataObj).suffix == '.csv') & (scriptOutputPrepro==None) :
            print('Simple DNN')
            pass
        elif (pathlib.Path(dataObj).suffix == '.csv') & (scriptOutputPrepro!=None) :
            print('Simple DNN with preprocessing')
        else:
            data_details=self.upDateStatus()
            self.updateStatusWithError(data_details,'Training Failed',"Not supported >> ",'No traceback',self.statusFile)
            return -1

        # if 'preprocessing' in modeScope:
        #         print ('Finaly came to preprocessing')
        #         try:
        #             trainData=modeScope['preprocessing']()
        #             print (trainData.shape)
        #             print('Preprocess step completed')
        #             kerasUtilities.updateStatusofProcess(self.statusFile,'Preprocessing for Model 1 Completed')
        #         except Exception as e:
        #             data_details=self.upDateStatus()
        #             self.updateStatusWithError(data_details,'Training Failed',"Some issue with preprocessing >> "+ str(e),traceback.format_exc(),self.statusFile)
        #             return -1
        # else:
        #     pass
        # XVar=modeScope['modelObj']['listOFColumns']
        # YVar=modeScope['modelObj']['targetCol']

        # modltToTrain=modeScope['modelObj']['recoModelObj']
        # print('Training step Started')
        # kerasUtilities.updateStatusofProcess(self.statusFile,'Training in Progress')
        # if str(type(modltToTrain))=="<class 'lightgbm.basic.Booster'>":
        #     print('Booster Model started')
        #     import lightgbm as lgb
        #     train_data=lgb.Dataset(trainData[XVar],trainData[YVar])
        #     modelHyParameters=modeScope['modelObj']['hyperparameters']
        #     newmodel1_ = lgb.train(modelHyParameters, train_data,init_model=modltToTrain)
        #     data_details=self.upDateStatus(statusFile)
        #     data_details['status']='Complete'
        #     with open(statusFile,'w') as filetosave:
        #         json.dump(data_details, filetosave)

        # elif 'sklearn.ensemble' in str(type(modltToTrain)):
        #     print('ensemble Model started')
        #     modelHyParameters=modeScope['modelObj']['hyperparameters']
        #     if YVar is None:
        #         modltToTrain.fit(trainData[XVar])
        #     else:
        #         print('XVar  >>>> ',XVar)
        #         modltToTrain.fit(trainData[XVar],trainData[YVar])
        #     newmodel1_=modltToTrain
            
        #     data_details=self.upDateStatus(statusFile)
        #     data_details['status']='Complete'
        #     with open(statusFile,'w') as filetosave:
        #         json.dump(data_details, filetosave)
        # else:
        #     data_details=self.upDateStatus(statusFile)
        #     data_details['status']='Training Failed'
        #     data_details['errorMessage']='Add support for other model '+str(e)
        #     data_details['errorTraceback']=traceback.format_exc()
        #     with open(statusFile,'w') as filetosave:
        #         json.dump(data_details, filetosave)

        # modelInformation['score'][modelObjsTrain[0]]['modelObj']['recoModelObj']=modelInformation['train'][modelObjsTrain[0]]['modelObj']['recoModelObj']=newmodel1_
        # print ('tempDict is ready to be')

        return 'dones'
    def trainModel(self,idforData,pmmlFile,tensorboardLogFolder):
        global PMMLMODELSTORAGE
        pmmlFileObj=pathlib.Path(pmmlFile)
        pmmlFileForKey=pmmlFileObj.name.replace(pmmlFileObj.suffix,'')
        saveStatus=self.logFolder+idforData+'/'
        self.statusFile=saveStatus+'status.txt'
        global PMMLMODELSTORAGE

        try:
            NewModelOperations().loadExecutionModel(pmmlFile)
        except Exception as e:
            print ('Came here')
            data_details=self.upDateStatus()
            self.updateStatusWithError(data_details,'Training Failed',"Couldn't load the PMML >> "+ str(e),traceback.format_exc(),self.statusFile)
            return -1
        # self.statusFile=statusFile
        modelInformation =PMMLMODELSTORAGE[pmmlFileForKey]
        
        modelObjsTrain=list(modelInformation['train'].keys())
        print('model object loaded')

        if len(modelObjsTrain)==0:
            data_details=self.upDateStatus()
            self.updateStatusWithError(data_details,'Training Failed',"Error while selecting model to Train >> "+ str(e),traceback.format_exc(),self.statusFile)
            return -1
        elif len(modelObjsTrain) ==2:
            data_details=self.upDateStatus()
            self.updateStatusWithError(data_details,'Training Failed',"Add support for training multiple model >> "+ str(e),traceback.format_exc(),self.statusFile)
            return -1
        elif len(modelObjsTrain) ==1:
            print ('>>>>>>>>>>>>>>>                 ',self.statusFile)
            print('Came in model 1')
            modeScope=modelInformation['train'][modelObjsTrain[0]]
            print ('modeScope>>>>>>>>>>> ',modeScope)
            kerasUtilities.updateStatusofProcess(self.statusFile,'Training Model Loaded')

            modeScope=self.trainModelObjectDict(modeScope,idforData,tensorboardLogFolder)
        
        tempDict=modelInformation
        listOfModelNames=set([k for j in tempDict for k in tempDict[j]])
        print ('listOfModelNames',listOfModelNames)

        toExportDict={}
        for objDet in listOfModelNames:
            toExportDict[objDet]={'hyperparameters':None,
                'preProcessingScript':{'scripts':[], 'scriptpurpose':[]},
                'modelObj':None,
                'pipelineObj':None,
                'featuresUsed':None,
                'targetName':None,
                'postProcessingScript':{'scripts':[], 'scriptpurpose':[]},
                'taskType': None}

        for modObjeCom in tempDict:
            if modObjeCom == 'train':
                for echMod in toExportDict:
                    if echMod in tempDict[modObjeCom]:
                        print ('>>>>>',echMod)
                        if tempDict[modObjeCom][echMod]['modelObj']['modelArchType']=='NNModel':
                            toExportDict[echMod]['modelObj']=tempDict[modObjeCom][echMod]['modelObj']['recoModelObj']
                            toExportDict[echMod]['modelObj']['model_graph']=tempDict[modObjeCom][echMod]['modelObj']['model_graph']
                            toExportDict[echMod]['modelObj']['tf_session']=tempDict[modObjeCom][echMod]['modelObj']['tf_session']
                        else:
                            toExportDict[echMod]['modelObj']=tempDict[modObjeCom][echMod]['modelObj']['recoModelObj']
                        if 'preprocessing_code' in tempDict[modObjeCom][echMod]:
                            toExportDict[echMod]['preProcessingScript']['scripts'].append(tempDict[modObjeCom][echMod]['preprocessing_code'])
                            toExportDict[echMod]['preProcessingScript']['scriptpurpose'].append(modObjeCom)
                        if 'postprocessing_code' in tempDict[modObjeCom][echMod]:
                            toExportDict[echMod]['postProcessingScript']['scripts'].append(tempDict[modObjeCom][echMod]['postprocessing_code'])
                            toExportDict[echMod]['postProcessingScript']['scriptpurpose'].append(modObjeCom)
                        toExportDict[echMod]['taskType']=modObjeCom
                        toExportDict[echMod]['featuresUsed']=tempDict[modObjeCom][echMod]['modelObj']['listOFColumns']
                        toExportDict[echMod]['targetName']=tempDict[modObjeCom][echMod]['modelObj']['targetCol']
                        toExportDict[echMod]['hyperparameters']=tempDict[modObjeCom][echMod]['modelObj']['hyperparameters']
            if modObjeCom == 'score':
                for echMod in toExportDict:
                    if echMod in tempDict[modObjeCom]:
                        print ('>>>>',echMod)
                        if tempDict[modObjeCom][echMod]['modelObj']['modelArchType']=='NNModel':
        #                     print (tempDict[modObjeCom][echMod]['modelObj'])
                            toExportDict[echMod]['modelObj']=tempDict[modObjeCom][echMod]['modelObj']['recoModelObj']
                            toExportDict[echMod]['model_graph']=tempDict[modObjeCom][echMod]['modelObj']['model_graph']
                            toExportDict[echMod]['tf_session']=tempDict[modObjeCom][echMod]['modelObj']['tf_session']
                        else:
                            toExportDict[echMod]['modelObj']=tempDict[modObjeCom][echMod]['modelObj']['recoModelObj']
                        if 'preprocessing_code' in tempDict[modObjeCom][echMod]:
                            toExportDict[echMod]['preProcessingScript']['scripts'].append(tempDict[modObjeCom][echMod]['preprocessing_code'])
                            toExportDict[echMod]['preProcessingScript']['scriptpurpose'].append(modObjeCom)
                        if 'postprocessing_code' in tempDict[modObjeCom][echMod]:
                            toExportDict[echMod]['postProcessingScript']['scripts'].append(tempDict[modObjeCom][echMod]['postprocessing_code'])
                            toExportDict[echMod]['postProcessingScript']['scriptpurpose'].append(modObjeCom)
                        toExportDict[echMod]['taskType']=modObjeCom
                        toExportDict[echMod]['featuresUsed']=tempDict[modObjeCom][echMod]['modelObj']['listOFColumns']
                        toExportDict[echMod]['targetName']=tempDict[modObjeCom][echMod]['modelObj']['targetCol']
                        toExportDict[echMod]['hyperparameters']=tempDict[modObjeCom][echMod]['modelObj']['hyperparameters']
                        
        for modNa in listOfModelNames:
            if (modNa in tempDict['train']) & (modNa in tempDict['score']):
                toExportDict[modNa]['taskType']='trainAndscore'
            if ((modNa in tempDict['train'] )== False) & (modNa in tempDict['score']):
                toExportDict[modNa]['taskType']='score'

        tempTa2=list(toExportDict.keys())
        tempTa2.sort()
        toExportReOrdered={}
        for taTTemp2 in tempTa2:
            toExportReOrdered[taTTemp2]=toExportDict[taTTemp2]

        toExportDict=toExportReOrdered.copy()

        # print ('8'*100)
        # print (modelInformation['score']['model2'])
        # print ('8'*100)

        print('*'*100)

        print (toExportDict)
        print('*'*100)
        orgfName='../ZMOD/Models/'+modelName+'.pmml'
        copyOrgFName='../ZMOD/Models/'+self.increName(modelName)+'.pmml'

        shutil.copy2(orgfName,copyOrgFName)
        model_to_pmml(toExportDict, pmml_f_name=copyOrgFName)
        NewModelOperations().loadExecutionModel(orgfName)
        data_details=self.upDateStatus(statusFile)
        data_details['status']='Model Saved in different Version'
        # data_details['errorMessage']='Add support for other model '+str(e)
        # data_details['errorTraceback']=traceback.format_exc()
        with open(statusFile,'w') as filetosave:
            json.dump(data_details, filetosave)

        return JsonResponse(data_details)


    def trainImageClassifierNN(self,modelObj):
        dataFolder=modelObj['Data']
        datHyperPara=modelObj['hyperparameters']

        print ('Classification data folder at',dataFolder)
        try:
            self.trainFolder=dataFolder+'/'+'train/'
            self.validationFolder=dataFolder+'/'+'validation/'
            kerasUtilities.checkCreatePath(self.trainFolder)
            kerasUtilities.checkCreatePath(self.validationFolder)
        except Exception as e:
            data_details=self.upDateStatus()
            self.updateStatusWithError(data_details,'Training Failed','Unable to find train and validation folder >> '+ str(e),traceback.format_exc(),self.statusFile)
            return -1

        modelObj = self.generateAndCompileModel(datHyperPara['lossType'],datHyperPara['optimizerName'],datHyperPara['learningRate'],datHyperPara['listOfMetrics'])
        if modelObj.__class__.__name__ == 'dict':
            return
        model = modelObj.model

        try:
            img_height, img_width=modelObj.image_input.shape.as_list()[1:3]
        except Exception as e:
            data_details=self.upDateStatus()
            self.updateStatusWithError(data_details,'Training Failed','Model input_shape is invalid >> '+ str(e),traceback.format_exc(),self.statusFile)
            return -1

        try:
            tGen,vGen,nClass=self.kerasDataPrep(dataFolder,batchSize,img_height,img_width)
            stepsPerEpochT=tGen.n/tGen.batch_size
            stepsPerEpochV=vGen.n/vGen.batch_size
        except Exception as e:
            data_details=self.upDateStatus()
            self.updateStatusWithError(data_details,'Training Failed','Error while generating data for Keras >> '+ str(e),traceback.format_exc(),self.statusFile)
            return -1

        tensor_board = self.startTensorBoard(tensorboardLogFolder)

        kerasUtilities.updateStatusOfTraining(self.statusFile,'Training Started')
        try:
            import tensorflow as tf
            print ('started Trianing')
            with tf.device(gpuCPUSelect(selDev)):
                model.fit_generator(tGen,steps_per_epoch=stepsPerEpochT,validation_steps=stepsPerEpochV,epochs=epoch,validation_data=vGen,callbacks=[tensor_board])
        except Exception as e:
            data_details=self.upDateStatus()
            self.updateStatusWithError(data_details,'Training Failed','There is a problem with training parameters >> '+ str(e),traceback.format_exc(),self.statusFile)
            return -1

        kerasUtilities.updateStatusOfTraining(self.statusFile,'Training Completed')

        predictedClass=list(tGen.class_indices.keys())
        try:
            toExportDict={
            'model1':{'data':dataFolder,'hyperparameters':datHyperPara,'preProcessingScript':None,
            'pipelineObj':None,'modelObj':model,'featuresUsed':None,'targetName':None,'postProcessingScript':None,
            'taskType': 'trainAndscore','predictedClasses':predictedClass,'dataSet':'image'}
                        }
            from nyokaBase.skl.skl_to_pmml import model_to_pmml
            model_to_pmml(toExportDict, PMMLFileName=fileName)
            kerasUtilities.updateStatusOfTraining(self.statusFile,'PMML file Successfully Saved')
            return 'Success'
        except Exception as e:
            data_details=self.upDateStatus()
            self.updateStatusWithError(data_details,'Saving File Failed',' '+ str(e),traceback.format_exc(),self.statusFile)
            return -1