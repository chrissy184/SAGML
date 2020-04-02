from django.shortcuts import render
from keras.models import load_model
from django.http import JsonResponse
import pathlib,datetime
from tensorflow import Graph, Session
import tensorflow as tf

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

    def scoreOnnxModel(self,modelKey):
        return None