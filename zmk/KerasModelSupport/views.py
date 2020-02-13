from django.shortcuts import render
from keras.models import load_model

# Create your views here.
class KerasExecution:

    def loadKerasModel(self,filePath):
        seqModel=load_model(filePath)
        return seqModel

    def getDetailsfromKerasModel(self,filePath):
        seqModel=self.loadKerasModel(filePath)
        allInfo={}
        allInfo['modelGeneratedFrom']='Keras'
        allInfo['deployableToZAD']=False
        allInfo['Version']='1.15.0'
        allInfo['information']=[]
        tempDict={}
        tempDict['property']='No. of layers'
        tempDict['value']=len(seqModel.layers)
        allInfo['information'].append(tempDict)
        return allInfo