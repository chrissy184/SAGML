from django.shortcuts import render
from keras.models import load_model
from django.http import JsonResponse
import pathlib,datetime
from tensorflow import Graph, Session
import tensorflow as tf
# Create your views here.
class KerasExecution:

    def loadKerasModel(self,filePath):
        model_graph = Graph()
        with model_graph.as_default():
            tf_session = Session()
            with tf_session.as_default():
                seqModel=load_model(filePath)
        return seqModel

    def getDetailsfromKerasModel(self,filePath):
        fO=pathlib.Path(filePath)
        seqModel=self.loadKerasModel(filePath)

        modelLastLayer=seqModel.layers[-1]
        if modelLastLayer.get_config()['activation'] in ['softmax']:
            funcType= 'Classification'
        else:
            funcType='Regression'
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