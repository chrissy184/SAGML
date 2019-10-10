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
            # print (taskT)
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
        # print (sFileText)
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

    def setOptimizer(self, optimizerName, learningRate):

        if optimizerName == 'Adadelta':
            opti=optimizers.Adadelta(lr=learningRate, rho=0.95, epsilon=None, decay=0.0)
        elif optimizerName == 'Adagrad':
            opti=optimizers.Adagrad(lr=learningRate, epsilon=None, decay=0.0)
        elif optimizerName == 'Adam':
            opti=optimizers.Adam(lr=learningRate, beta_1=0.9, beta_2=0.999, epsilon=None, decay=1e-6, amsgrad=False)
        elif optimizerName == 'Adamax':
            opti=optimizers.Adamax(lr=learningRate, beta_1=0.9, beta_2=0.999, epsilon=None, decay=0.0)
        elif optimizerName == 'Nadam':
            opti=optimizers.Nadam(lr=learningRate, beta_1=0.9, beta_2=0.999, epsilon=None, schedule_decay=0.004)
        elif optimizerName == 'Rmsprop':
            opti=optimizers.RMSprop(lr=learningRate, rho=0.9, epsilon=None, decay=0.0)
        elif optimizerName == 'Sgd':
            opti=optimizers.SGD(lr=learningRate, momentum=0.0, decay=0.0, nesterov=False)
        else:
            opti=optimizers.Adam(lr=learningRate, beta_1=0.9, beta_2=0.999, epsilon=None, decay=1e-6, amsgrad=False)
        return opti

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

    def f1(self,y_true, y_pred):
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


    def generateAndCompileModel(self,modelObjZG, lossType, optimizerName, learningRate, listOfMetrics, compileTestOnly=False):

       
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

        try:
            model_graph = modelObjZG['modelObj']['model_graph']
            tf_session = modelObjZG['modelObj']['tf_session']
            # print ('model_graph,tf_session',model_graph,tf_session)
            with model_graph.as_default():
                with tf_session.as_default():
                    model=modelObjZG['modelObj']['recoModelObj'].model
                    if 'f1' in listOfMetrics:
                        listOfMetrics.remove('f1')
                        model.compile(optimizer=optiMi, loss=lossType, metrics=listOfMetrics+[self.f1])
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
        return model,model_graph,tf_session

    def startTensorBoard(self, tensorboardLogFolder):
        # print ('tensorboardLogFolder >>>>>>',tensorboardLogFolder)
        tensor_board=keras.callbacks.TensorBoard(log_dir=tensorboardLogFolder, histogram_freq=0,write_graph=True, write_images=False)
        return tensor_board
    def kerasDataPrep(self,dataFolder,batch_size,img_height, img_width):
        train_data_dir=dataFolder+'/'+'train/'
        val_data_dir=dataFolder+'/'+'validation/'

        train_datagen = ImageDataGenerator(rescale=1. / 255,shear_range=0.2,zoom_range=0.2,horizontal_flip=True)
        test_datagen = ImageDataGenerator(rescale=1. / 255)

        train_generator = train_datagen.flow_from_directory(train_data_dir,target_size=(img_height, img_width),
        batch_size=batch_size,class_mode='categorical')
        if os.path.exists(val_data_dir)==True:
            validation_generator = test_datagen.flow_from_directory(val_data_dir,target_size=(img_height, img_width),
            batch_size=batch_size,class_mode='categorical')
            return train_generator,validation_generator,len(train_generator.class_indices)
        else:
            return train_generator,None,len(train_generator.class_indices)

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
        # print ('modelObj???????????????????',modelObj)
        try:
            dataObj=modelObj['Data']
        except:
            dataObj=None
        modelArch=modelObj['modelObj']['modelArchType']
        try:
            scriptOutputPrepro=modelObj['scriptOutput']
        except:
            scriptOutputPrepro=None
        
        # print ('scriptOutputPrepro',scriptOutputPrepro,dataObj,modelArch)

        datHyperPara=modelObj['modelObj']['hyperparameters']
        if modelArch == 'NNModel':
            checkVal=self.verifyHyperparameters(datHyperPara)
            if checkVal == 'done':
                pass
            else:
                data_details=self.upDateStatus()
                self.updateStatusWithError(data_details,'Training Failed',"Some issue with hyperparameters >> ",'No info',self.statusFile)
                return -1
		
        if (dataObj == None) & (scriptOutputPrepro != None):
            print ('To complicated 1')
            modelObjTrained=self.trainComplicatedDNNObj(modelObj,tensorboardLogFolder,scriptOutputPrepro)
        elif (os.path.isdir(dataObj)) & (scriptOutputPrepro==None):
            print ('Came to Image classifier')
            modelObjTrained=self.trainImageClassifierNN(modelObj,tensorboardLogFolder)
        elif (pathlib.Path(dataObj).suffix == '.csv') & (scriptOutputPrepro==None) :
            print('Simple DNN')
            dataObjPd=pd.read_csv(modelObj['Data'])
            print (dataObjPd.shape)
            modelObjTrained=self.trainSimpleDNNObj(modelObj,tensorboardLogFolder,dataObjPd)
        elif (pathlib.Path(dataObj).suffix == '.csv') & (scriptOutputPrepro!=None) :
            dataObjPd=pd.read_csv(modelObj['Data'])
            print (dataObjPd.shape)
            modelObjTrained=self.trainSimpleDNNObj(modelObj,tensorboardLogFolder,dataObjPd)
            print('Simple DNN with preprocessing')
        else:
            data_details=self.upDateStatus()
            self.updateStatusWithError(data_details,'Training Failed',"Not supported >> ",'No traceback',self.statusFile)
            return -1
        return modelObjTrained


    def trainSimpleDNNObj(self,modelObj,tensorboardLogFolder,dataObj):
        dataFolder=modelObj['Data']
        # modelToCompile=modelObj['modelObj']['recoModelObj']
        datHyperPara=modelObj['modelObj']['hyperparameters']
        listOfMetrics=datHyperPara['listOfMetrics']
        modelV1=modelObj['modelObj']['recoModelObj'].model
        print(">>>>>>>>>>>>>>SimpleDNN")
        print('pathofdata>>>>>',dataFolder)
        predictedClass=None
        targetColumnName = 'target'
        df = dataObj
        indevar=list(df.columns)
        indevar.remove('target')
        targetCol = df[targetColumnName]
        if datHyperPara['problemType']=='classification':
            lb=preprocessing.LabelBinarizer()
            y=lb.fit_transform(targetCol)
            predictedClass = list(targetCol.unique())
        else:
            y=df[targetColumnName]
            predictedClass=None
        ##### Split data into test and validation set for training#################################
        trainDataX,testDataX,trainDataY,testDataY=model_selection.train_test_split(df[indevar],y,
                                                        test_size=datHyperPara['testSize'])
        stepsPerEpochT=int(len(trainDataX)/datHyperPara['batchSize'])
        stepsPerEpochV=int(len(testDataX)/datHyperPara['batchSize'])
        kerasUtilities.updateStatusOfTraining(self.statusFile,'Data split in Train validation part')

        # modelObj = self.generateAndCompileModel(datHyperPara['lossType'],datHyperPara['optimizerName'],datHyperPara['learningRate'],listOfMetrics)
        # if modelObj.__class__.__name__ == 'dict':
        #     return
        # model = modelObj.model
        tensor_board = self.startTensorBoard(tensorboardLogFolder)
        try:
            # print ('Came here 1'*5 )
            model_graph = modelObj['modelObj']['model_graph']
            tf_session = modelObj['modelObj']['tf_session']
            with model_graph.as_default():
                with tf_session.as_default():
                    
                    if 'f1' in listOfMetrics:
                        listOfMetrics.remove('f1')
                        optiMi=self.setOptimizer(datHyperPara['optimizerName'],datHyperPara['learningRate'])
                        modelV1.compile(optimizer=optiMi, loss=datHyperPara['lossType'],metrics=listOfMetrics+[self.f1])
                        import tensorflow as tf
                        kerasUtilities.updateStatusOfTraining(self.statusFile,'Training Started')
                        with tf.device(gpuCPUSelect(selDev)):
                            modelV1.fit(x=trainDataX, y=trainDataY, epochs=datHyperPara['epoch'], callbacks=[tensor_board],\
                                        validation_data=(testDataX, testDataY), steps_per_epoch=stepsPerEpochT, validation_steps=stepsPerEpochV)
                    else:
                        optiMi=self.setOptimizer(datHyperPara['optimizerName'],datHyperPara['learningRate'])
                        modelV1.compile(optimizer=optiMi, loss=datHyperPara['lossType'], metrics=listOfMetrics)
                        import tensorflow as tf
                        kerasUtilities.updateStatusOfTraining(self.statusFile,'Training Started')
                        with tf.device(gpuCPUSelect(selDev)):
                           modelV1.fit(x=trainDataX, y=trainDataY, epochs=datHyperPara['epoch'], callbacks=[tensor_board],\
                                        validation_data=(testDataX, testDataY), steps_per_epoch=stepsPerEpochT, validation_steps=stepsPerEpochV)

                        print ('9'*500)
        except Exception as e:
            print ('Came here 2'*5 )
            data_details=self.upDateStatus()
            self.updateStatusWithError(data_details,'Training Failed','Error while fitting data to Keras Model >> '+ str(e),traceback.format_exc(),self.statusFile)

        kerasUtilities.updateStatusOfTraining(self.statusFile,'Training Completed')
        modelObj['modelObj']['recoModelObj'].model=modelV1
        return modelObj

    def trainImageClassifierNN(self,modelObj,tensorboardLogFolder):
        print ('Enter image classifier')
        dataFolder=modelObj['Data']
        # modelToCompile=modelObj['modelObj']['recoModelObj']
        datHyperPara=modelObj['modelObj']['hyperparameters']
        listOfMetrics=datHyperPara['listOfMetrics']
        modelV1=modelObj['modelObj']['recoModelObj'].model

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

        try:
            img_height, img_width=modelV1.input_shape[1:3]
        except Exception as e:
            data_details=self.upDateStatus()
            self.updateStatusWithError(data_details,'Training Failed','Model input_shape is invalid >> '+ str(e),traceback.format_exc(),self.statusFile)
            return -1

        try:
            tGen,vGen,nClass=self.kerasDataPrep(dataFolder,datHyperPara['batchSize'],img_height,img_width)
            stepsPerEpochT=tGen.n/tGen.batch_size
            stepsPerEpochV=vGen.n/vGen.batch_size
        except Exception as e:
            data_details=self.upDateStatus()
            self.updateStatusWithError(data_details,'Training Failed','Error while generating data for Keras >> '+ str(e),traceback.format_exc(),self.statusFile)
            return -1

        tensor_board = self.startTensorBoard(tensorboardLogFolder)

        try:
            # print ('Came here 1'*5 )
            model_graph = modelObj['modelObj']['model_graph']
            tf_session = modelObj['modelObj']['tf_session']
            with model_graph.as_default():
                with tf_session.as_default():
                    
                    if 'f1' in listOfMetrics:
                        listOfMetrics.remove('f1')
                        optiMi=self.setOptimizer(datHyperPara['optimizerName'],datHyperPara['learningRate'])
                        modelV1.compile(optimizer=optiMi, loss=datHyperPara['lossType'], metrics=listOfMetrics+[self.f1])
                        import tensorflow as tf
                        kerasUtilities.updateStatusOfTraining(self.statusFile,'Training Started')
                        with tf.device(gpuCPUSelect(selDev)):
                            modelV1.fit_generator(tGen,steps_per_epoch=stepsPerEpochT,validation_steps=stepsPerEpochV,epochs=datHyperPara['epoch'],validation_data=vGen,callbacks=[tensor_board])
                    else:
                        optiMi=self.setOptimizer(datHyperPara['optimizerName'],datHyperPara['learningRate'])
                        modelV1.compile(optimizer=optiMi, loss=datHyperPara['lossType'], metrics=listOfMetrics)
                        import tensorflow as tf
                        kerasUtilities.updateStatusOfTraining(self.statusFile,'Training Started')
                        with tf.device(gpuCPUSelect(selDev)):
                            modelV1.fit_generator(tGen,steps_per_epoch=stepsPerEpochT,validation_steps=stepsPerEpochV,epochs=datHyperPara['epoch'],validation_data=vGen,callbacks=[tensor_board])
        except Exception as e:
            print ('Came here 2'*5 )
            data_details=self.upDateStatus()
            self.updateStatusWithError(data_details,'Training Failed','Error while compiling Model >> '+ str(e),traceback.format_exc(),self.statusFile)

        kerasUtilities.updateStatusOfTraining(self.statusFile,'Training Completed')

        predictedClass=list(tGen.class_indices.keys())
        modelObj['modelObj']['recoModelObj'].model=modelV1
        return modelObj

    def getSKLMOdelObjtoFit(self,modelV1):
        from sklearn import ensemble
        # print (str(modelV1))
        if 'RandomForestRegressor' in str(modelV1):
            return  ensemble.RandomForestRegressor()
        return modelV1
    def trainComplicatedDNNObj(self,modelObj,tensorboardLogFolder,scriptOutputPrepro):
        # print ('*'*500)
        # print ('Came to complicated part',modelObj)
        # print ('modelObj',modelObj.keys())

        if scriptOutputPrepro=='DATA':
            dataObj,tar=modelObj['preprocessing']()
            # print (dataObj.shape)
        else:
            pass

        predictedClass=None
        df = dataObj
        datHyperPara=modelObj['modelObj']['hyperparameters']
        
        if modelObj['modelObj']['modelArchType']=='SKLModel':
            # print('P'*200)
            modelV1=modelObj['modelObj']['recoModelObj']
            modelV1=self.getSKLMOdelObjtoFit(modelV1)
            modelV1.fit(df,tar)
            modelObj['modelObj']['recoModelObj']=modelV1

        else:
            listOfMetrics=datHyperPara['listOfMetrics']
            targetCol = tar
            # print(">>>>>>>>>>>>>>SimpleDNN")
            modelV1=modelObj['modelObj']['recoModelObj'].model
            if datHyperPara['problemType']=='classification':
                lb=preprocessing.LabelBinarizer()
                y=lb.fit_transform(targetCol)
                predictedClass = list(targetCol.unique())
            else:
                y=tar
                predictedClass=None
            # print(df.shape,y.shape,datHyperPara['testSize'])
            trainDataX,testDataX,trainDataY,testDataY=model_selection.train_test_split(df,tar,test_size=datHyperPara['testSize'])
            stepsPerEpochT=int(len(trainDataX)/datHyperPara['batchSize'])
            stepsPerEpochV=int(len(testDataX)/datHyperPara['batchSize'])
            kerasUtilities.updateStatusOfTraining(self.statusFile,'Data split in Train validation part')

            # modelObj = self.generateAndCompileModel(datHyperPara['lossType'],datHyperPara['optimizerName'],datHyperPara['learningRate'],listOfMetrics)
            # if modelObj.__class__.__name__ == 'dict':
            #     return
            # model = modelObj.model
            tensor_board = self.startTensorBoard(tensorboardLogFolder)
            try:
                # print ('Came here 1'*5 )
                model_graph = modelObj['modelObj']['model_graph']
                tf_session = modelObj['modelObj']['tf_session']
                with model_graph.as_default():
                    with tf_session.as_default():
                        
                        if 'f1' in listOfMetrics:
                            listOfMetrics.remove('f1')
                            optiMi=self.setOptimizer(datHyperPara['optimizerName'],datHyperPara['learningRate'])
                            modelV1.compile(optimizer=optiMi, loss=datHyperPara['lossType'],metrics=listOfMetrics+[self.f1])
                            import tensorflow as tf
                            kerasUtilities.updateStatusOfTraining(self.statusFile,'Training Started')
                            with tf.device(gpuCPUSelect(selDev)):
                                modelV1.fit(x=trainDataX, y=trainDataY, epochs=datHyperPara['epoch'], callbacks=[tensor_board],\
                                            validation_data=(testDataX, testDataY), steps_per_epoch=stepsPerEpochT, validation_steps=stepsPerEpochV)
                        else:
                            optiMi=self.setOptimizer(datHyperPara['optimizerName'],datHyperPara['learningRate'])
                            modelV1.compile(optimizer=optiMi, loss=datHyperPara['lossType'], metrics=listOfMetrics)
                            import tensorflow as tf
                            kerasUtilities.updateStatusOfTraining(self.statusFile,'Training Started')
                            with tf.device(gpuCPUSelect(selDev)):
                                modelV1.fit(x=trainDataX, y=trainDataY, epochs=datHyperPara['epoch'], callbacks=[tensor_board],\
                                                validation_data=(testDataX, testDataY), steps_per_epoch=stepsPerEpochT, validation_steps=stepsPerEpochV)
                            # print ('9'*500)
                            modelObj['modelObj']['recoModelObj'].model=modelV1
            except Exception as e:
                print ('Came here 2'*5 )
                data_details=self.upDateStatus()
                self.updateStatusWithError(data_details,'Training Failed','Error while fitting data to Keras Model >> '+ str(e),traceback.format_exc(),self.statusFile)
        
        kerasUtilities.updateStatusOfTraining(self.statusFile,'Training 1st part completed')
        
        return modelObj

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
            # print ('>>>>>>>>>>>>>>>                 ',self.statusFile)
            print('Came in model 1')
            modeScope=modelInformation['train'][modelObjsTrain[0]]
            # print ('modeScope>>>>>>>>>>> ',modeScope)
            kerasUtilities.updateStatusofProcess(self.statusFile,'Training Model Loaded')

            modeScope=self.trainModelObjectDict(modeScope,idforData,tensorboardLogFolder)
            modelInformation['train'][modelObjsTrain[0]]=modeScope

        if modeScope == -1:
            return -1

        # print ('trainModelObjectDict modeScope  >>>>>>>>> ',modeScope)
        
        tempDict=modelInformation
        listOfModelNames=set([k for j in tempDict for k in tempDict[j]])
        # print ('listOfModelNames',listOfModelNames)

        toExportDict={}
        for objDet in listOfModelNames:
            toExportDict[objDet]={'hyperparameters':None,
                                'data':None,
                                'preProcessingScript':{'scripts':[], 'scriptpurpose':[],'scriptOutput':[]},
                                'modelObj':None,
                                'pipelineObj':None,
                                'featuresUsed':None,
                                'targetName':None,
                                'postProcessingScript':{'scripts':[], 'scriptpurpose':[],'scriptOutput':[]},
                                'taskType': None}

        # print ('tempDict>>>>>>>>>>>>>>>>>>>>>>>',tempDict)

        for modObjeCom in tempDict:
            if modObjeCom == 'train':
                for echMod in toExportDict:
                    if echMod in tempDict[modObjeCom]:
                        # print ('>>>>>',echMod)
                        # print ('8'*100)
                        # print (tempDict[modObjeCom][echMod]['modelObj']['model_graph'])
                        if tempDict[modObjeCom][echMod]['modelObj']['modelArchType']=='NNModel':
                        #                     print (tempDict[modObjeCom][echMod]['modelObj'])
                            toExportDict[echMod]['modelObj']=tempDict[modObjeCom][echMod]['modelObj']['recoModelObj'].model
                            toExportDict[echMod]['model_graph']=tempDict[modObjeCom][echMod]['modelObj']['model_graph']
                            toExportDict[echMod]['tf_session']=tempDict[modObjeCom][echMod]['modelObj']['tf_session']
                        else:
                            toExportDict[echMod]['modelObj']=tempDict[modObjeCom][echMod]['modelObj']['recoModelObj']
                        if 'preprocessing_code' in tempDict[modObjeCom][echMod]:
                            toExportDict[echMod]['preProcessingScript']['scripts'].append(tempDict[modObjeCom][echMod]['preprocessing_code'])
                            toExportDict[echMod]['preProcessingScript']['scriptpurpose'].append(modObjeCom)
                            toExportDict[echMod]['preProcessingScript']['scriptOutput'].append(tempDict[modObjeCom][echMod]['scriptOutput'])
                        if 'postprocessing_code' in tempDict[modObjeCom][echMod]:
                            toExportDict[echMod]['postProcessingScript']['scripts'].append(tempDict[modObjeCom][echMod]['postprocessing_code'])
                            toExportDict[echMod]['postProcessingScript']['scriptpurpose'].append(modObjeCom)
                            toExportDict[echMod]['postProcessingScript']['scriptOutput'].append(tempDict[modObjeCom][echMod]['scriptOutput'])
                        toExportDict[echMod]['taskType']=modObjeCom
                        toExportDict[echMod]['featuresUsed']=tempDict[modObjeCom][echMod]['modelObj']['listOFColumns']
                        toExportDict[echMod]['targetName']=tempDict[modObjeCom][echMod]['modelObj']['targetCol']
                        toExportDict[echMod]['hyperparameters']=tempDict[modObjeCom][echMod]['modelObj']['hyperparameters']

                        if 'Data' in tempDict[modObjeCom][echMod]:
                            toExportDict[echMod]['data']=tempDict[modObjeCom][echMod]['Data']
            if modObjeCom == 'score':
                for echMod in toExportDict:
                    if echMod in tempDict[modObjeCom]:
                        # print ('>>>>',echMod)
                        if tempDict[modObjeCom][echMod]['modelObj']['modelArchType']=='NNModel':
        #                     print (tempDict[modObjeCom][echMod]['modelObj'])
                            toExportDict[echMod]['modelObj']=tempDict[modObjeCom][echMod]['modelObj']['recoModelObj'].model
                            toExportDict[echMod]['model_graph']=tempDict[modObjeCom][echMod]['modelObj']['model_graph']
                            toExportDict[echMod]['tf_session']=tempDict[modObjeCom][echMod]['modelObj']['tf_session']
                        else:
                            toExportDict[echMod]['modelObj']=tempDict[modObjeCom][echMod]['modelObj']['recoModelObj']
                        if 'preprocessing_code' in tempDict[modObjeCom][echMod]:
                            toExportDict[echMod]['preProcessingScript']['scripts'].append(tempDict[modObjeCom][echMod]['preprocessing_code'])
                            toExportDict[echMod]['preProcessingScript']['scriptpurpose'].append(modObjeCom)
                            toExportDict[echMod]['preProcessingScript']['scriptOutput'].append(tempDict[modObjeCom][echMod]['scriptOutput'])
                        if 'postprocessing_code' in tempDict[modObjeCom][echMod]:
                            toExportDict[echMod]['postProcessingScript']['scripts'].append(tempDict[modObjeCom][echMod]['postprocessing_code'])
                            toExportDict[echMod]['postProcessingScript']['scriptpurpose'].append(modObjeCom)
                            toExportDict[echMod]['postProcessingScript']['scriptOutput'].append(tempDict[modObjeCom][echMod]['scriptOutput'])
                        toExportDict[echMod]['taskType']=modObjeCom
                        toExportDict[echMod]['featuresUsed']=tempDict[modObjeCom][echMod]['modelObj']['listOFColumns']
                        toExportDict[echMod]['targetName']=tempDict[modObjeCom][echMod]['modelObj']['targetCol']
                        toExportDict[echMod]['hyperparameters']=tempDict[modObjeCom][echMod]['modelObj']['hyperparameters']
                        if 'Data' in tempDict[modObjeCom][echMod]:
                            toExportDict[echMod]['data']=tempDict[modObjeCom][echMod]['Data']
      
        for modNa in listOfModelNames:
            if (modNa in tempDict['train']) & (modNa in tempDict['score']):
                toExportDict[modNa]['taskType']='trainAndscore'
                # print (tempDict['train'][modNa].keys())
                # print (tempDict['score'][modNa]['preprocessing_code'][0])
                if (tempDict['train'][modNa]['scriptOutput']=='DATA') & (tempDict['score'][modNa]['scriptOutput'] == 'DATA'):
                    # print ('Came here >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>')
                    toExportDict[modNa]['preProcessingScript']['scriptOutput']=['DATA']
                    toExportDict[modNa]['preProcessingScript']['scriptpurpose']=['trainAndscore']
                    # print( '>>>>>>>>',tempDict['score'][modNa]['preprocessing_code'])
                    toExportDict[modNa]['preProcessingScript']['scripts']=[tempDict['score'][modNa]['preprocessing_code']]
            # print ("toExportDict[modNa]['preProcessingScript']['script']",toExportDict[modNa]['preProcessingScript']['script'])
            if ((modNa in tempDict['train'] )== False) & (modNa in tempDict['score']):
                toExportDict[modNa]['taskType']=['score']

        tempTa2=list(toExportDict.keys())
        tempTa2.sort()
        toExportReOrdered={}
        for taTTemp2 in tempTa2:
            toExportReOrdered[taTTemp2]=toExportDict[taTTemp2]

        toExportDict=toExportReOrdered.copy()

        # print ('8'*100)
        # print (toExportDict['model1'].keys(),listOfModelNames)
        # print ('8'*100)

        # print('*'*100)

        # print (toExportDict)
        # print('*'*100)
        fN=pathlib.Path(pmmlFile).name
        orgfName='../ZMOD/Models/'+fN#+'.pmml'
        fN=fN.replace('.pmml','')
        copyOrgFName='../ZMOD/Models/'+self.increName(fN)+'.pmml'
        import shutil
        from nyokaBase.skl.skl_to_pmml import model_to_pmml
        shutil.copy2(orgfName,copyOrgFName)
        model_to_pmml(toExportDict, PMMLFileName=copyOrgFName)
        NewModelOperations().loadExecutionModel(orgfName)
        # data_details['status']='Model Saved in different Version'
        kerasUtilities.updateStatusOfTraining(self.statusFile,'Model Saved in different Version')
        data_details=self.upDateStatus()

        return JsonResponse(data_details)



        # try:
        #     toExportDict={
        #     'model1':{'data':dataFolder,'hyperparameters':datHyperPara,'preProcessingScript':None,
        #     'pipelineObj':None,'modelObj':model,'featuresUsed':None,'targetName':None,'postProcessingScript':None,
        #     'taskType': 'trainAndscore','predictedClasses':predictedClass,'dataSet':'image'}
        #                 }
        #     from nyokaBase.skl.skl_to_pmml import model_to_pmml
        #     model_to_pmml(toExportDict, PMMLFileName=fileName)
        #     kerasUtilities.updateStatusOfTraining(self.statusFile,'Complete')
        #     return 'Success'
        # except Exception as e:
        #     data_details=self.upDateStatus()
        #     self.updateStatusWithError(data_details,'Saving File Failed',' '+ str(e),traceback.format_exc(),self.statusFile)
        #     return -1