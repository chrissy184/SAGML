from django.shortcuts import render
from keras.models import load_model
from django.http import JsonResponse
import pathlib,datetime
from tensorflow import Graph, Session
import tensorflow as tf
from string import ascii_uppercase
from random import choice
import os,json
import numpy as np
from keras.preprocessing import image
import pandas as pd

from trainModel.kerasUtilities import PMMLMODELSTORAGE
global PMMLMODELSTORAGE
# Create your views here.


class KerasExecution:

    def loadKerasModel(self,filePath):
        global PMMLMODELSTORAGE
        fO=pathlib.Path(filePath)
        keyToModel=fO.name.replace(fO.suffix,'')
        # print (PMMLMODELSTORAGE)
        try:
            model_graph = Graph()
            with model_graph.as_default():
                tf_session = Session()
                with tf_session.as_default():
                    seqModel=load_model(filePath)

            tempDictModel={'modelObj':seqModel,
                            'model_graph':model_graph,
                            'modelGeneratedFrom':'Keras',
                            'tf_session':tf_session,
                            'inputShape':seqModel.input_shape,
                            }
            PMMLMODELSTORAGE[keyToModel]=tempDictModel
            messageToWorld= "Model Loaded Successfully"
            reStat=200
        except:
            messageToWorld="Model load failed, please connect with admin"
            keyToModel=None
            reStat=500
        resultResp={'message':messageToWorld,'keytoModel':keyToModel}
        return JsonResponse(resultResp,status=reStat)

    def getDetailsfromKerasModel(self,filePath):
        fO=pathlib.Path(filePath)
        keyToModel=fO.name.replace(fO.suffix,'')
        self.loadKerasModel(filePath)
        global PMMLMODELSTORAGE
        # print (PMMLMODELSTORAGE)
        seqModel=PMMLMODELSTORAGE[keyToModel]['modelObj']

        modelLastLayer=seqModel.layers[-1]
        try:
            if modelLastLayer.get_config()['activation'] in ['softmax','sigmoid']:
                funcType= 'Classification'
            else:
                funcType='Regression'
        except:
            funcType='None'
        allInfo={}
        allInfo['modelGeneratedFrom']='Keras'
        allInfo['deployableToZAD']=False
        # allInfo['Version']='1.15.0'
        allInfo['information']=[]

        tempDict={}
        tempDict['property']='Version'
        tempDict['value']='1.15.0'
        allInfo['information'].append(tempDict)


        tempDict={}
        tempDict['property']='Number Of Layers'
        tempDict['value']=len(seqModel.layers)
        allInfo['information'].append(tempDict)

        tempDict={}
        tempDict['property']='Time File Created'
        tempDict['value']=str(datetime.datetime.fromtimestamp(fO.lstat().st_atime))
        allInfo['information'].append(tempDict)

        tempDict={}
        tempDict['property']='Description'
        tempDict['value']='Keras Model'
        allInfo['information'].append(tempDict)

        tempDict={}
        tempDict['property']='Function Type'
        tempDict['value']=funcType
        allInfo['information'].append(tempDict)

        tempDict={}
        tempDict['property']='Model Name'
        tempDict['value']=fO.name.replace(fO.suffix,'')
        allInfo['information'].append(tempDict)
        return JsonResponse(allInfo)

class ONNXExecution:

    def checkCreatePath(self,folderPath):
        try:
            if os.path.exists(folderPath):
                return ('path exist')
            else:
                os.makedirs(folderPath)
                return ('path created')
        except:
            os.makedirs(folderPath)
            return ('path created')

    def loadOnnxModel(self,filePath):
        print ('Came here')
        global PMMLMODELSTORAGE
        fO=pathlib.Path(filePath)
        keyToModel=fO.name.replace(fO.suffix,'')
        # print (PMMLMODELSTORAGE)
        try:
            import onnxruntime,onnx
            onnx_model = onnx.load(filePath)
            serCont= onnx_model.SerializeToString()
            sess = onnxruntime.InferenceSession(serCont)
            # inputForModel=[(ob.name,x[en]) for en,ob in enumerate()]
            # model_graph = Graph()
            # with model_graph.as_default():
            #     tf_session = Session()
            #     with tf_session.as_default():
            #         seqModel=load_model(filePath)

            tempDictModel={#'modelObj':seqModel,
                            # 'model_graph':model_graph,
                            'modelGeneratedFrom':'ONNX',
                            'model_session':sess,
                            'inputShape':[i.shape for i in sess.get_inputs()],
                            }
            PMMLMODELSTORAGE[keyToModel]=tempDictModel
            messageToWorld= "Model Loaded Successfully"
            reStat=200
        except:
            messageToWorld="Model load failed, please connect with admin"
            keyToModel=None
            reStat=500
        resultResp={'message':messageToWorld,'keytoModel':keyToModel}
        return JsonResponse(resultResp,status=reStat)

    def getDetailsfromOnnxModel(self,filePath):
        fO=pathlib.Path(filePath)
        keyToModel=fO.name.replace(fO.suffix,'')
        self.loadOnnxModel(filePath)
        global PMMLMODELSTORAGE
        # print (PMMLMODELSTORAGE)
        # seqModel=PMMLMODELSTORAGE[keyToModel]['modelObj']

        # modelLastLayer=seqModel.layers[-1]
        # try:
        #     if modelLastLayer.get_config()['activation'] in ['softmax','sigmoid']:
        #         funcType= 'Classification'
        #     else:
        #         funcType='Regression'
        # except:
        funcType='None'
        allInfo={}
        allInfo['modelGeneratedFrom']='ONNX'
        allInfo['deployableToZAD']=True
        # allInfo['Version']='1.15.0'
        allInfo['information']=[]

        tempDict={}
        tempDict['property']='Version'
        tempDict['value']=None
        allInfo['information'].append(tempDict)


        tempDict={}
        tempDict['property']='Number Of Layers'
        tempDict['value']=None
        allInfo['information'].append(tempDict)

        tempDict={}
        tempDict['property']='Time File Created'
        tempDict['value']=str(datetime.datetime.fromtimestamp(fO.lstat().st_atime))
        allInfo['information'].append(tempDict)

        tempDict={}
        tempDict['property']='Description'
        tempDict['value']='ONNX Model'
        allInfo['information'].append(tempDict)

        tempDict={}
        tempDict['property']='Function Type'
        tempDict['value']=funcType
        allInfo['information'].append(tempDict)

        tempDict={}
        tempDict['property']='Model Name'
        tempDict['value']=fO.name.replace(fO.suffix,'')
        allInfo['information'].append(tempDict)
        return JsonResponse(allInfo)

    def scoreOnnxModel(self,modelKey,filePath):
        import numpy as np
        print ('came here')
        print ('came here',modelKey,filePath)
        target_path='./logs/'+''.join(choice(ascii_uppercase) for i in range(12))+'/'
        self.checkCreatePath(target_path)
        global PMMLMODELSTORAGE
        # print (PMMLMODELSTORAGE[modelName])
        modelInformation =PMMLMODELSTORAGE[modelKey]
        print (modelInformation)
        modelObjs=list(modelInformation.keys())
        print (modelObjs)
        sess=modelInformation['model_session']
        output_name = sess.get_outputs()[0].name

        if pathlib.Path(filePath).suffix =='.csv':
            testData=pd.read_csv(filePath)
        elif pathlib.Path(filePath).suffix in ['.jpg','.JPG','.png','.PNG']:
            inputShapevals=modelInformation['inputShape'][0]
            testimage=filePath
            img_height, img_width=inputShapevals[1:3]
            img = image.load_img(testimage, target_size=(img_height, img_width))
            testData = image.img_to_array(img)
            testData = np.expand_dims(testData, axis=0)
            testData=testData/255
            testData = testData if isinstance(testData, list) else [testData]
            inputForModel=[(ob.name,testData[en]) for en,ob in enumerate(sess.get_inputs())]
            res = sess.run([output_name], dict(inputForModel))
            predi=res[0]

            if pathlib.Path(filePath).suffix =='.csv':
                testData['predicted_Score']=predi
                print (testData.shape)
                resafile=target_path+'result.csv'
                testData.to_csv(resafile, index=False)
            elif pathlib.Path(filePath).suffix in ['.jpg','.JPG','.png','.PNG']:
                import numpy as np
                predClasses=['class_'+str(i) for i in range(len(np.ravel(predi)))]
                targetResult= {j:str(float(k)) for j,k in zip(predClasses,list(predi[0]))}
                resafile=target_path+'result.txt'
                with open(resafile,'w') as f:
                    f.write(json.dumps(targetResult))

        resultResp={'result':resafile}
        return JsonResponse(resultResp,status=200)